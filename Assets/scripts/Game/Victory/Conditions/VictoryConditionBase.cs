using System;
using UnityEngine;

[Serializable]
public abstract class VictoryConditionBase
{
    [SerializeField] private string displayName;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? GetType().Name : displayName;

    public abstract float Target { get; }
    public abstract float Current(GameStats stats);

    public virtual bool IsCompleted(GameStats stats) => Current(stats) >= Target;

    public virtual float Progress01(GameStats stats)
    {
        if (Target <= 0) return 1f;
        return Mathf.Clamp01(Current(stats) / Target);
    }

    public virtual string GetProgressText(GameStats stats)
        => $"{DisplayName}: {Current(stats):0.##}/{Target:0.##}";
}