using System;
using System.Collections.Generic;

namespace Game.Foundation
{
    public enum TimeDomain
    {
        Scaled,    // 使用 Time.deltaTime（受 timeScale 影响）
        Unscaled,  // 使用 Time.unscaledDeltaTime（不受 timeScale 影响）
    }

    public readonly struct TimerHandle : IEquatable<TimerHandle>
    {
        public readonly int Id;
        public TimerHandle(int id) => Id = id;
        public bool Equals(TimerHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is TimerHandle other && Equals(other);
        public override int GetHashCode() => Id;
        public static readonly TimerHandle None = new TimerHandle(0);
        public override string ToString() => $"TimerHandle({Id})";
    }

    /// <summary>
    /// 时间事件分发：
    /// - Update 驱动：TimeEventManager.Update(scaledDt, unscaledDt)
    /// - 订阅 Tick：OnTick / OnUnscaledTick
    /// - 定时：Schedule / ScheduleRepeating / NextFrame
    /// </summary>
    public static class TimeEventManager
    {
        // Tick 事件（也可以通过 EventManager.Publish 分发，这里直接用 Action 更轻）
        public static event Action<float> OnTick;          // scaled dt
        public static event Action<float> OnUnscaledTick;  // unscaled dt

        private class Timer
        {
            public int id;
            public TimeDomain domain;
            public float due;          // 距离触发还有多久
            public float interval;     // 循环间隔；<=0 表示一次性
            public int repeat;         // -1 无限；>0 次数；1 表示只触发一次（等价一次性）
            public Action callback;
            public bool cancelled;
        }

        private static int _nextId = 1;

        private static readonly List<Timer> _scaledTimers = new();
        private static readonly List<Timer> _unscaledTimers = new();

        // 避免遍历时增删问题：新增先放队列，Update 开头合并
        private static readonly List<Timer> _pendingAddScaled = new();
        private static readonly List<Timer> _pendingAddUnscaled = new();

        // 下一帧队列（分 scaled/unscaled 无所谓，这里统一下一帧执行）
        private static readonly List<Action> _nextFrame = new();
        private static readonly List<Action> _nextFramePending = new();

        public static void Update(float scaledDeltaTime, float unscaledDeltaTime)
        {
            // 1) 合并新增 timer
            if (_pendingAddScaled.Count > 0)
            {
                _scaledTimers.AddRange(_pendingAddScaled);
                _pendingAddScaled.Clear();
            }
            if (_pendingAddUnscaled.Count > 0)
            {
                _unscaledTimers.AddRange(_pendingAddUnscaled);
                _pendingAddUnscaled.Clear();
            }

            // 2) 先执行下一帧任务（上一帧安排的）
            if (_nextFrame.Count > 0)
            {
                for (int i = 0; i < _nextFrame.Count; i++)
                {
                    try { _nextFrame[i]?.Invoke(); }
                    catch (Exception e) { UnityEngine.Debug.LogException(e); }
                }
                _nextFrame.Clear();
            }
            // 把本帧安排的 nextFrame 放到队列里，保证严格“下一帧”
            if (_nextFramePending.Count > 0)
            {
                _nextFrame.AddRange(_nextFramePending);
                _nextFramePending.Clear();
            }

            // 3) Tick 分发
            OnTick?.Invoke(scaledDeltaTime);
            OnUnscaledTick?.Invoke(unscaledDeltaTime);

            // 4) Timer 驱动
            StepTimers(_scaledTimers, scaledDeltaTime);
            StepTimers(_unscaledTimers, unscaledDeltaTime);

            // 5) 清理取消的 timer（可选：每帧都清，简单）
            Cleanup(_scaledTimers);
            Cleanup(_unscaledTimers);
        }

        public static TimerHandle NextFrame(Action callback)
        {
            if (callback == null) return TimerHandle.None;

            // nextFrame 的取消也用 TimerHandle 统一管理：用 timer 模拟下一帧一次性触发
            // 这里直接走 Schedule(0) 也行，但会在“同帧”触发（看实现）。我们严格下一帧：
            var id = _nextId++;
            var wrapped = WrapWithHandleCancel(id, callback);
            _nextFramePending.Add(wrapped);

            // 记录到一个“伪 timer”表以支持 Cancel（这里用字典存取消标记）
            _nextFrameCancelFlags[id] = false;
            return new TimerHandle(id);
        }

        // --- Timer API ---

        public static TimerHandle Schedule(float delaySeconds, Action callback, TimeDomain domain = TimeDomain.Scaled)
            => ScheduleInternal(delaySeconds, 0f, 1, callback, domain);

        public static TimerHandle ScheduleRepeating(float delaySeconds, float intervalSeconds, Action callback, TimeDomain domain = TimeDomain.Scaled, int repeatCount = -1)
            => ScheduleInternal(delaySeconds, intervalSeconds, repeatCount, callback, domain);

        public static bool Cancel(TimerHandle handle)
        {
            if (handle.Id == 0) return false;

            // 先处理 nextFrame 的取消
            if (_nextFrameCancelFlags.TryGetValue(handle.Id, out var flag))
            {
                _nextFrameCancelFlags[handle.Id] = true;
                return true;
            }

            bool cancelled = CancelInList(_scaledTimers, handle.Id) | CancelInList(_unscaledTimers, handle.Id);
            cancelled |= CancelInList(_pendingAddScaled, handle.Id) | CancelInList(_pendingAddUnscaled, handle.Id);
            return cancelled;
        }

        public static void CancelAll()
        {
            foreach (var t in _scaledTimers) t.cancelled = true;
            foreach (var t in _unscaledTimers) t.cancelled = true;
            foreach (var t in _pendingAddScaled) t.cancelled = true;
            foreach (var t in _pendingAddUnscaled) t.cancelled = true;

            _nextFrame.Clear();
            _nextFramePending.Clear();
            _nextFrameCancelFlags.Clear();
        }

        // --- internal ---

        private static TimerHandle ScheduleInternal(float delay, float interval, int repeat, Action callback, TimeDomain domain)
        {
            if (callback == null) return TimerHandle.None;
            if (delay < 0f) delay = 0f;
            if (interval < 0f) interval = 0f;

            var timer = new Timer
            {
                id = _nextId++,
                domain = domain,
                due = delay,
                interval = interval,
                repeat = (interval <= 0f) ? 1 : repeat, // 一次性强制 repeat=1
                callback = callback,
            };

            if (domain == TimeDomain.Scaled) _pendingAddScaled.Add(timer);
            else _pendingAddUnscaled.Add(timer);

            return new TimerHandle(timer.id);
        }

        private static void StepTimers(List<Timer> list, float dt)
        {
            if (dt <= 0f) return;
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                if (t.cancelled) continue;

                t.due -= dt;
                if (t.due > 0f) continue;

                // 触发
                try { t.callback?.Invoke(); }
                catch (Exception e) { UnityEngine.Debug.LogException(e); }

                if (t.interval <= 0f)
                {
                    t.cancelled = true;
                    continue;
                }

                if (t.repeat > 0)
                {
                    t.repeat--;
                    if (t.repeat <= 0)
                    {
                        t.cancelled = true;
                        continue;
                    }
                }

                // 重新计时（如果 due<0，允许累积误差不补偿；需要补偿可改为 while）
                t.due += t.interval;
            }
        }

        private static void Cleanup(List<Timer> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].cancelled) list.RemoveAt(i);
            }
        }

        private static bool CancelInList(List<Timer> list, int id)
        {
            bool found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].id == id)
                {
                    list[i].cancelled = true;
                    found = true;
                }
            }
            return found;
        }

        // ---- NextFrame 取消支持 ----
        private static readonly Dictionary<int, bool> _nextFrameCancelFlags = new();

        private static Action WrapWithHandleCancel(int id, Action cb)
        {
            return () =>
            {
                if (_nextFrameCancelFlags.TryGetValue(id, out var cancelled) && cancelled)
                {
                    _nextFrameCancelFlags.Remove(id);
                    return;
                }
                _nextFrameCancelFlags.Remove(id);
                cb?.Invoke();
            };
        }
    }
}
