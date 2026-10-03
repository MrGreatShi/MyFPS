using System;
using UnityEngine;

[Serializable]
public class SurviveTimeCondition : VictoryConditionBase
{
    [SerializeField] private float targetSeconds = 60f;
    public override float Target => targetSeconds;
    public override float Current(GameStats stats) => stats.SurviveTime;

    public override string GetProgressText(GameStats stats)
        => $"{DisplayName}: {stats.SurviveTime:0.0}s/{Target:0.0}s";
}