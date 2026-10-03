using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config", fileName = "LevelConfig_001")]
public class LevelConfig : ScriptableObject
{
    [Header("Basic")]
    public int levelId = 1;
    public string levelName;

    [Header("Scene")]
    public string sceneName; // 场景名称（必须与实际场景名匹配）

    [Header("Victory")]
    public VictoryConfig victory; // 直接引用你的胜利条件配置（也是一个SO）

    [Header("Spawners / Enemies")]
    public List<EnemySpawnConfig> enemySpawns = new();

    [Header("Optional")]
    public float timeLimitSeconds = 0f; // 0=不限时

    [Header("NextLevel")]
    public int nextLevelId = 0; // 0=没有下一关
}
