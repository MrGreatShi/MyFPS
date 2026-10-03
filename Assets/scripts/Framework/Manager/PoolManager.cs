using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance = null;

    private List<SimpleObjectPool> pools = new List<SimpleObjectPool>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    // 获取与指定 prefab 关联的 SimpleObjectPool（不存在则创建）
    public SimpleObjectPool GetPool(GameObject prefab, int preload = 10)
    {
        foreach (var pool in pools)
        {
            if (pool != null && pool.prefab == prefab)
            {
                return pool;
            }
        }

        // 创建一个专用的子物体作为该池的容器,在容器上添加 SimpleObjectPool 组件并初始化,挂载到 PoolManager 下
        var containerName = $"Pool_{(prefab != null ? prefab.name : "Unknown")}";
        var newPool = new GameObject(containerName).AddComponent<SimpleObjectPool>();
        newPool.prefab = prefab;
        newPool.transform.SetParent(transform, false);

        if (preload < 0)
        {
            preload = 10;
        }
        newPool.preloadCount = preload;

        pools.Add(newPool);
        return newPool;
    }
    public void ClearAllPools()
    {
        foreach (var pool in pools)
        {
            if (pool != null)
            {
                pool.Clear();
            }
        }
    }

    public void DestoryFreePools()
    {
        foreach (var pool in pools)
        {
            if (pool != null && pool._count == 0)
            {
                pool.Clear();
                Destroy(pool.gameObject);
            }
        }
    }
}
