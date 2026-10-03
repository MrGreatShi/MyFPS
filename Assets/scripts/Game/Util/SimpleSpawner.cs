using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    [Header("Pooling")]
    public SimpleObjectPool objectPool;

    [Header("Spawn Settings")]
    [Min(0.1f)]
    public float spawnInterval = 2f;
    [Tooltip("是否自动刷新")]
    public bool autoSpawn = true;
    public bool autoStart = true;

    [Header("Spawn Point Setting")]
    [Tooltip("刷新点")]
    public List<Vector3> spawnPoints;
    [Tooltip("随机生成范围形状")]
    public bool SpawnInCircle = false; // 是否在 spawnPoints 的基础上随机一个圆形范围内的点
    [Header("Random Offset (Optional)")]
    [Tooltip("圆形范围(半径)")]
    public float spawnRadius = 0.5f; // 圆形范围的半径（仅 SpawnInCircle 为 true 时有效）
    [Tooltip("矩形范围(X)")]
    public float spawnOffsetX = 10f;
    [Tooltip("矩形范围(Y)")]
    public float spawnOffsetY = 0.3f;
    [Tooltip("矩形范围(Z)")]
    public float spawnOffsetZ = 10f;

    [Header("Limit Total (Optional)")]
    public bool limitTotal = false;
    public int totalToSpawn = 30;
    public int maxAlive = 10;

    private GameObject target;

    private readonly List<GameObject> _alive = new List<GameObject>();
    private Coroutine _routine;
    private int _spawnedTotal;

    public int AliveCount => _alive.Count;
    public int SpawnedTotal => _spawnedTotal;

    private void Start()
    {
        if(objectPool != null) objectPool.Apply(this);
        if (autoSpawn && autoStart) StartSpawning();
        if(target == null && objectPool.prefab.CompareTag("Enemy")) target = GameObject.FindGameObjectWithTag("Player");
    }
    public void StartSpawning()
    {
        if (_routine != null) return;

        if (objectPool == null || objectPool.prefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPool 或 enemyPool.prefab 未设置。", this);
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("[EnemySpawner] 没有刷新点。请设置 spawnPoints 或创建子物体作为刷新点。", this);
            return;
        }

        _routine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (_routine == null) return;
        StopCoroutine(_routine);
        _routine = null;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            CleanupInactiveOrNull();

            bool reachAliveLimit = _alive.Count >= maxAlive;
            bool reachTotalLimit = limitTotal && _spawnedTotal >= totalToSpawn;

            if (!reachAliveLimit && !reachTotalLimit)
            {
                SpawnOne();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnOne()
    {
        Vector3 spawnPos = GetRandomSpawnPoint();
        Vector3 spawnAngle = GetRandomSpawnAngle();

        GameObject go = objectPool.Get(spawnPos, Quaternion.Euler(spawnAngle));
        if (go == null) return;
        go.transform.SetParent(transform, true);
        if(target == null && go.CompareTag("Enemy")) target = GameObject.FindGameObjectWithTag("Player");
        var _bt = go.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (_bt != null)
        {
            if(target != null) _bt.SetVariableValue("target", target.transform);
            _bt.SetVariableValue("health", go.GetComponent<Health>());
            _bt.SetVariableValue("animator", go.GetComponent<Animator>());
        }
            
            
        _alive.Add(go);
        _spawnedTotal++;
    }
    public void SpawnOne(Vector3 pos, Quaternion rot)
    {
        GameObject go = objectPool.Get(pos, rot);
        go.transform.SetParent(transform, true);
        _alive.Add(go);
        _spawnedTotal++;    
    }
    public void DespawnAllAndStop()
    {
        StopSpawning();
        foreach(GameObject gameObject in _alive)
        {
            gameObject.GetComponent<PooledObject>()?.Despawn();
        }
        _alive.Clear();
        _spawnedTotal = 0;
        
    }
    private Vector3 GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Count);
        if (SpawnInCircle)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            float x = randomCircle.x;
            float z = randomCircle.y;
            return spawnPoints[index] + new Vector3(x, 0, z);
        }
        else
        {
            float offsetX = Random.Range(-spawnOffsetX, spawnOffsetX);
            float offsetY = Random.Range(-spawnOffsetY, spawnOffsetY);
            float offsetZ = Random.Range(-spawnOffsetZ, spawnOffsetZ);
            return spawnPoints[index] + new Vector3(offsetX, offsetY, offsetZ);
        }
    }
    private Vector3 GetRandomSpawnAngle()
    {
        float y = Random.Range(0f, 360f);
        return new Vector3(0f, y, 0f);
    }
    private void CleanupInactiveOrNull()
    {
        for (int i = _alive.Count - 1; i >= 0; i--)
        {
            var e = _alive[i];
            if (e == null || !e.activeInHierarchy)
                _alive.RemoveAt(i);
        }
    }

    // 回收/销毁时调用都可以
    public void NotifyEnemyDestroyed(GameObject enemy)
    {
        _alive.Remove(enemy);
    }

    private void OnDestroy()
    {
        objectPool.Release(this);
    }
}
