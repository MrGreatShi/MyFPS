using System;
using System.Collections.Generic;

[Serializable]
public class GameStats
{
    public float SurviveTime;
    // 记录每种敌人的击杀数
    public Dictionary<string, int> EnemyKillCounts = new(); 
    // 通用计数：收集物品、完成事件次数等
    private Dictionary<string, int> _intCounters = new();

    // 通用标记：BOSS是否击杀、关键事件是否发生等
    private HashSet<string> _flags = new();

    public void Reset()
    {
        SurviveTime = 0f;
        EnemyKillCounts.Clear();
        _intCounters.Clear();
        _flags.Clear();
    }

    public void AddKill(string enemyType)
    {
        if (!string.IsNullOrEmpty(enemyType))
        {
            if (EnemyKillCounts.ContainsKey(enemyType))
                EnemyKillCounts[enemyType]++;
            else
                EnemyKillCounts[enemyType] = 1;
        }
    }
    public void AddKill(string enemyType, int count)
    {
        if (!string.IsNullOrEmpty(enemyType))
        {
            if (EnemyKillCounts.ContainsKey(enemyType))
                EnemyKillCounts[enemyType] += count;
            else
                EnemyKillCounts[enemyType] = count;
        }
    }
    public int GetKillCount(string enemyType)
    {
        if (string.IsNullOrEmpty(enemyType)) return 0;
        return EnemyKillCounts.TryGetValue(enemyType, out var count) ? count : 0;
    }

    public void AddCounter(string key, int delta = 1)
    {
        if (string.IsNullOrEmpty(key)) return;
        _intCounters.TryGetValue(key, out var v);
        _intCounters[key] = v + delta;
    }

    public int GetCounter(string key)
    {
        if (string.IsNullOrEmpty(key)) return 0;
        return _intCounters.TryGetValue(key, out var v) ? v : 0;
    }

    public void SetFlag(string key)
    {
        if (!string.IsNullOrEmpty(key)) _flags.Add(key);
    }

    public bool HasFlag(string key)
    {
        if (string.IsNullOrEmpty(key)) return false;
        return _flags.Contains(key);
    }
}