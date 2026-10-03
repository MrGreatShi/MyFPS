using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [Header("Runtime Spawners (optional preset)")]
    [SerializeField] private List<SimpleSpawner> spawners = new();

    [Header("Addressables Loader")]
    [SerializeField] public AddressablesLoader<GameObject> Loader = new();


    private void Awake()
    {
        RefreshSpawnersFromChildren();
    }

    /// <summary>
    /// 从子物体刷新现有 spawner，并注入 gameManager 引用
    /// </summary>
    public void RefreshSpawnersFromChildren()
    {
        if (spawners == null) spawners = new List<SimpleSpawner>();

        if (spawners.Count == 0)
            spawners.AddRange(GetComponentsInChildren<SimpleSpawner>(true));

    }

    /// <summary>
    /// 应用关卡配置：根据 enemySpawns 创建/更新 spawner（异步：会加载 Addressables prefab）
    /// </summary>
    public async Task ApplyLevelConfigAsync(LevelConfig levelConfig)
    {
        if (levelConfig == null)
        {
            Debug.LogError("SpawnerController.ApplyLevelConfigAsync: levelConfig is null", this);
            return;
        }

        // 逐条应用 enemy spawn 配置
        foreach (var cfg in levelConfig.enemySpawns)
        {
            await UpsertSpawnerAsync(cfg);
        }
    }

    /// <summary>开始所有允许 autoSpawn 的 spawner</summary>
    public void StartAll()
    {
        foreach (var s in spawners)
        {
            if (s != null && s.autoSpawn && s.autoStart)
                s.StartSpawning();
        }
    }

    /// <summary>停止所有 spawner 并清场（需要 SimpleSpawner 提供 DespawnAllAndStop）</summary>
    public void StopAllAndDespawn()
    {
        foreach (var s in spawners)
        {
            if (s != null)
                s.DespawnAllAndStop();
        }
    }

    /// <summary>
    /// 创建或更新一个 spawner
    /// </summary>
    private async Task UpsertSpawnerAsync(EnemySpawnConfig cfg)
    {
        if (cfg == null)
            return;

        // 1) 加载 prefab
        var handle = await Loader.LoadPrefabHandleAsync(cfg.AddressablesKey);
        var prefab = handle.Result;
        if (prefab == null)
        {
            Debug.LogError($"SpawnerController: prefab load result is null: {cfg.AddressablesKey}", this);
            return;
        }

        // 2) 查找是否已有同 prefab 的 spawner
        for (int i = 0; i < spawners.Count; i++)
        {
            var s = spawners[i];
            if (s == null) continue;

            if (s.objectPool != null && s.objectPool.prefab == prefab)
            {
                ApplyConfigToSpawner(s, cfg);
                return;
            }
        }

        // 3) 没有则创建新的 spawner
        var spawnerName = prefab.name + "Spawner";
        var newSpawner = new GameObject(spawnerName).AddComponent<SimpleSpawner>();
        newSpawner.transform.SetParent(transform, false);

        // 对象池：用 PoolManager.Instance.GetPoolOfObject(prefab, s.maxAlive)
        var pool = PoolManager.Instance.GetPool(prefab, cfg.maxAlive);

        newSpawner.objectPool = pool;

        ApplyConfigToSpawner(newSpawner, cfg);
        spawners.Add(newSpawner);
    }

    private void ApplyConfigToSpawner(SimpleSpawner spawner, EnemySpawnConfig cfg)
    {
        spawner.autoSpawn = cfg.autoSpawn;
        spawner.autoStart = cfg.autoStart;
        spawner.spawnInterval = cfg.spawnInterval;

        spawner.spawnPoints = cfg.spawnPoints;

        spawner.SpawnInCircle = cfg.spawnInCircle;
        spawner.spawnRadius = cfg.spawnRadius;

        spawner.spawnOffsetX = cfg.spawnOffsetX;
        spawner.spawnOffsetY = cfg.spawnOffsetY;
        spawner.spawnOffsetZ = cfg.spawnOffsetZ;

        spawner.limitTotal = cfg.limitTotal;
        spawner.totalToSpawn = cfg.totalToSpawn;

        spawner.maxAlive = cfg.maxAlive;
    }

    public void Clear()
    {
        StopAllAndDespawn();
        spawners.Clear();
    }
}
