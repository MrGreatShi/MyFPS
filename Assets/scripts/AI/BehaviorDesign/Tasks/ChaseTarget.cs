using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy/Move")]
public class ChaseTarget : Action
{
    public SharedTransform target;
    public SharedGameObject motorObject; // 可不填，默认 GetComponent<EnemyMotor>()

    public SharedFloat stoppingDistance = 1f;
    public SharedFloat repathInterval = 0.2f;

    private EnemyMotor _motor;
    private float _nextRepathTime;

    public override void OnStart()
    {
        _motor = ResolveMotor();
        _nextRepathTime = 0f;

        if (_motor != null)
            _motor.SetChaseMode(stoppingDistance.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (_motor == null) return TaskStatus.Failure;
        if (target == null || target.Value == null) return TaskStatus.Failure;

        // 到时间才更新一次路径
        if (Time.time >= _nextRepathTime)
        {
            _nextRepathTime = Time.time + Mathf.Max(0.05f, repathInterval.Value);
            _motor.SetDestination(target.Value.position);
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        if (_motor == null) return;

        var agent = _motor.Agent;
        if (agent == null) return;
        if (!agent.enabled) return;
        if (!agent.isOnNavMesh) return;

        _motor.Stop();
    }

    private EnemyMotor ResolveMotor()
    {
        if (motorObject != null && motorObject.Value != null)
            return motorObject.Value.GetComponent<EnemyMotor>();
        return GetComponent<EnemyMotor>();
    }
}