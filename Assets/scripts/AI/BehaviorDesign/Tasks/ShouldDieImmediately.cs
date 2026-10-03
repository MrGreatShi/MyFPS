using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Enemy/State")]
public class ShouldDieImmediately : Conditional
{
    public SharedBehaviour Health;
    public SharedInt phase;
    public SharedBool allowImmediateDeath;
    public SharedBool deathRequested;

    public override TaskStatus OnUpdate()
    { 
        var health = Health.Value as Health;
        if(health != null)
        {
            deathRequested.Value = health.IsDead;
        }
        allowImmediateDeath.Value = phase.Value != 2;
        return (deathRequested.Value && allowImmediateDeath.Value)
            ? TaskStatus.Success
            : TaskStatus.Failure;
    }
}
