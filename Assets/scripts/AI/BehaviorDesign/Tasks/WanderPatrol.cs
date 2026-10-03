using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Enemy/Move")]
public class WanderPatrol : Action
{
    public SharedGameObject motorObject; // 可不填，默认 GetComponent<EnemyMotor>()
    public float wanderRadius = 10f;
    public float repathDelay = 0.5f;     // 到点后等待再换点
    public SharedInt areaMask = NavMesh.AllAreas;

    private EnemyMotor _motor;
    private float _waitUntil;

    public override void OnStart()
    {
        _motor = ResolveMotor();
        _waitUntil = 0f;

        if (_motor != null)
        {
            _motor.SetPatrolMode();
            TryPickNewPoint();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_motor == null) return TaskStatus.Failure;

        // 到点了：等待一下再换点
        if (_motor.HasReachedDestination())
        {
            if (_waitUntil <= 0f)
                _waitUntil = Time.time + Mathf.Max(0f, repathDelay);

            if (Time.time >= _waitUntil)
            {
                _waitUntil = 0f;
                if (!TryPickNewPoint())
                    return TaskStatus.Failure;
            }
        }

        return TaskStatus.Running;
    }

    private EnemyMotor ResolveMotor()
    {
        if (motorObject != null && motorObject.Value != null)
            return motorObject.Value.GetComponent<EnemyMotor>();
        return GetComponent<EnemyMotor>();
    }

    private bool TryPickNewPoint()
    {
        float radius = wanderRadius > 0 ? wanderRadius : (_motor != null ? _motor.wanderRadius : 10f);
        return _motor.TrySetRandomDestination(radius, areaMask.Value);
    }
}