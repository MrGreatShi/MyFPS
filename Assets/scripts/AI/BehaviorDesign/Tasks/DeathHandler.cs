using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy/Lifecycle")]
public class DeathHandler : Action
{
    public SharedTransform target;
    public string dieTrigger = "Die";
    public SharedFloat despawnDelay = 0.4f;

    public override void OnStart()
    {
        transform.LookAt(target.Value);
        var animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger(dieTrigger);

        var pooled = GetComponent<PooledObject>();
        if (pooled != null)
            pooled.Despawn(despawnDelay.Value);

    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
