using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy/Combat")]
public class DoAttack : Action
{
    private EnemyCombat _combat;

    public SharedFloat NextAttack;
    public float attackCooldown = 3f;

    public override void OnStart()
    {
        _combat = GetComponent<EnemyCombat>();
        _combat?.StartAttack();
        NextAttack.Value = Time.time + attackCooldown;
    }

    public override TaskStatus OnUpdate()
    {
        if (_combat == null) return TaskStatus.Failure;
        return _combat.IsAttacking ? TaskStatus.Running : TaskStatus.Success;
    }
}
