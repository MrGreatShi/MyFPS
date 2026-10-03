using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Foundation;

public class VictorySystem : MonoBehaviour
{
    [SerializeField] private VictoryConfig config;

    public GameStats Stats { get; private set; } = new GameStats();
    public bool IsRunning { get; private set; }
    public bool IsVictory { get; private set; }

    private float _secAccumulator; // 累计 dt，用于按秒更新

    private void OnEnable()
    {
        EventManager.Subscribe<EnemyKilledEvent>(OnEnemyKilled);

        // scaled tick（受 timeScale 影响）
        TimeEventManager.OnTick -= OnTick;
        TimeEventManager.OnTick += OnTick;
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        TimeEventManager.OnTick -= OnTick;
    }

    private void OnTick(float dt)
    {
        if (!IsRunning || IsVictory) return;
        if (dt <= 0f) return;

        _secAccumulator += dt;

        // 每满 1 秒更新一次（最小单位为秒）
        // 用 while 处理卡顿：如果一帧 dt=2.2，也能补两次“秒级更新”
        while (_secAccumulator >= 1f && IsRunning && !IsVictory)
        {
            _secAccumulator -= 1f;

            // 只按秒累加（避免小数）
            Stats.SurviveTime += 1f;

            // 每秒才评估一次胜利条件
            Evaluate();
        }
    }

    private void OnEnemyKilled(EnemyKilledEvent evt)
    {
        if (!IsRunning || IsVictory) return;
        AddKill(evt.Enemy.name, 1);
    }

    public void StartRun()
    {
        Stats.Reset();
        _secAccumulator = 0f;

        IsVictory = false;
        IsRunning = true;

        Evaluate();
    }

    public void SetConfig(VictoryConfig newConfig)
    {
        config = newConfig;
        Evaluate();
    }

    public void Pause() => IsRunning = false;
    public void Resume() { if (!IsVictory) IsRunning = true; }

    public void AddKill(string EnemyName, int delta = 1)
    {
        if (IsVictory) return;

        Stats.AddKill(EnemyName, delta);

        // 击杀属于“事件驱动”，仍然即时评估（不必等到下一秒）
        Evaluate();
    }

    public void AddCounter(string key, int delta = 1) { Stats.AddCounter(key, delta); Evaluate(); }
    public void SetFlag(string key) { Stats.SetFlag(key); Evaluate(); }

    public IReadOnlyList<VictoryConditionBase> GetConditions()
        => config != null ? config.conditions : Array.Empty<VictoryConditionBase>();

    private void Evaluate()
    {

        if (config == null || config.conditions == null || config.conditions.Count == 0)
            return;

        bool win = (config.logic == VictoryConfig.Logic.All);

        foreach (var c in config.conditions)
        {
            if (c == null) continue;
            bool done = c.IsCompleted(Stats);

            if (config.logic == VictoryConfig.Logic.All) win &= done;
            else win |= done;
        }

        if (win && !IsVictory)
        {
            IsVictory = true;
            IsRunning = false;
            EventManager.Publish(new VictoryEvent { Source = this });
        }
        EventManager.Publish(new ProgressUpdateEvent { Condition = null });
    }
}