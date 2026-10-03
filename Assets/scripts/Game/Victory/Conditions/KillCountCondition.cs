using System;
using UnityEngine;

[Serializable]
public class KillCountCondition : VictoryConditionBase
{
    [SerializeField] private int targetKills = 10;
    public override float Target => targetKills;
    public override float Current(GameStats stats) => stats.GetKillCount(DisplayName);
}