using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SimpleObjectPool : MonoBehaviour
{
    [Header("Pool")]
    public GameObject prefab;
    [Min(0)]
    public int preloadCount = 10;

    [Tooltip("池里不够时是否允许自动扩容")]
    public bool canGrow = true;

    readonly Queue<GameObject> _inactive = new Queue<GameObject>();

    readonly List<SimpleSpawner> _applied = new List<SimpleSpawner>();

    public int _count => _applied.Count;

    void Start()
    {
        Preload(preloadCount);
    }

    public void Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var go = CreateNew();
            Return(go);
        }
    }

    GameObject CreateNew()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        var po = go.GetComponent<PooledObject>();
        if (po == null) po = go.AddComponent<PooledObject>();
        if (go.CompareTag("Enemy"))//敌人需要上报击杀事件
        {
            var ekr = go.GetComponent<EnemyKillReporter>();
            if(ekr == null) ekr = go.AddComponent<EnemyKillReporter>();
        }
        po.pool = this;

        go.SetActive(false);
        return go;
    }

    public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject go = null;

        while (_inactive.Count > 0 && go == null)
        {
            go = _inactive.Dequeue();
        }

        if (go == null)
        {
            if (!canGrow) return null;
            go = CreateNew();
        }

        var t = go.transform;
        t.SetParent(parent, false);
        t.SetPositionAndRotation(position, rotation);

        go.SetActive(true);
        return go;
    }

    public void Return(GameObject go)
    {
        if (go == null) return;

        go.SetActive(false);
        go.transform.SetParent(transform, false);
        _inactive.Enqueue(go);
    }

    public void Clear()
    {
        while (_inactive.Count > 0)
        {
            var go = _inactive.Dequeue();
            if (go != null)
            {
                Destroy(go);
            }
        }
    }

    public void Apply(SimpleSpawner spawner) 
    {
        if(_applied.Contains(spawner)) return;
        _applied.Add(spawner);
    }
    public void Release(SimpleSpawner spawner)
    {
        if(!_applied.Contains(spawner)) return;
        _applied.Remove(spawner);
    }
}
