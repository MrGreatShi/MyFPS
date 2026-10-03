using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnConfig
{
    [Header("Enemy")]
    public string AddressablesKey;                 //  Addressables key£¨¼ûÏÂ£©

    [Header("Spawn Rules")]
    public float spawnInterval = 2f;
    public bool autoSpawn = true;
    public bool autoStart = true;

    public bool limitTotal = false;
    public int totalToSpawn = 30;

    public int maxAlive = 10;

    [Header("Spawn Points")]
    public List<Vector3> spawnPoints = new();
    public bool spawnInCircle = false;
    public float spawnRadius = 0.5f;

    [Header("Random Offset (Optional)")]
    public float spawnOffsetX = 10f;
    public float spawnOffsetY = 0.3f;
    public float spawnOffsetZ = 10f;
}
