using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy/Combat")]
public class IsInAttackRange : Conditional
{
    public SharedTransform target;
    public float attackRange = 2f;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null) return TaskStatus.Failure;
        return Vector3.Distance(transform.position, target.Value.position) <= attackRange
            ? TaskStatus.Success : TaskStatus.Failure;
    }
}
