using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMotor : MonoBehaviour
{
    [Header("Speed")]
    public float patrolSpeed = 1.0f;
    public float chaseSpeed = 2.0f;

    [Header("Stopping")]
    public float defaultStoppingDistance = 0.2f;

    [Header("Wander")]
    public float wanderRadius = 10f;

    private NavMeshAgent _agent;
    private Animator _animator;

    public NavMeshAgent Agent => _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        if (_agent != null)
        {
            if (_agent.stoppingDistance <= 0f)
                _agent.stoppingDistance = defaultStoppingDistance;
        }
    }
    private void Update()
    {
        // 可选：根据速度切换动画
        if (_animator != null && _agent != null)
        {
            _animator.SetFloat("Speed", _agent.velocity.magnitude);
        }
    }

    public void SetPatrolMode()
    {
        if (_agent == null) return;
        _agent.isStopped = false;
        _agent.speed = patrolSpeed;
        if (_agent.stoppingDistance != defaultStoppingDistance)
            _agent.stoppingDistance = defaultStoppingDistance;
    }

    public void SetChaseMode(float stoppingDistance)
    {
        if (_agent == null) return;
        _agent.isStopped = false;
        _agent.speed = chaseSpeed;
        _agent.stoppingDistance = Mathf.Max(0f, stoppingDistance);
    }

    public void Stop()
    {
        if (_agent == null) return;
        if (!_agent.enabled) return;              // 关键：agent 被禁用时不能操作
        if (!isActiveAndEnabled) return;          // 组件/物体未激活
        if (!_agent.isOnNavMesh) return;          // 不在 NavMesh 上会报错

        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public bool SetDestination(Vector3 position)
    {
        if (_agent == null) return false;
        if (!_agent.isOnNavMesh) return false;
        _agent.isStopped = false;
        return _agent.SetDestination(position);
    }

    public bool HasReachedDestination(float extraTolerance = 0.05f)
    {
        if (_agent == null) return true;

        if (_agent.pathPending) return false;
        if (!_agent.hasPath) return true;

        // remainingDistance 可能在某些情况下为 Mathf.Infinity
        if (float.IsInfinity(_agent.remainingDistance)) return false;

        float threshold = _agent.stoppingDistance + Mathf.Max(0f, extraTolerance);
        return _agent.remainingDistance <= threshold;
    }

    /// <summary>
    /// 在当前位置附近随机游走：随机点 -> NavMesh.SamplePosition -> SetDestination
    /// </summary>
    public bool TrySetRandomDestination(float radius, int areaMask = NavMesh.AllAreas, int sampleTries = 10, float sampleMaxDistance = 2f)
    {
        if (_agent == null) return false;
        if (!_agent.isOnNavMesh) return false;

        Vector3 origin = transform.position;

        for (int i = 0; i < sampleTries; i++)
        {
            Vector2 rand = Random.insideUnitCircle * radius;
            Vector3 candidate = origin + new Vector3(rand.x, 0f, rand.y);

            if (NavMesh.SamplePosition(candidate, out var hit, sampleMaxDistance, areaMask))
            {
                return SetDestination(hit.position);
            }
        }
        return false;
    }
}
