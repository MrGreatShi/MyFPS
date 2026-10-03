using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy/State")]
public class SetPhase : Action
{
    public SharedInt phase;
    public SharedBool allowImmediateDeath;

    public int phaseToSet = 0;

    public override TaskStatus OnUpdate()
    {
        phase.Value = phaseToSet;

        // Ñ²Âß/×·»÷£ºÔÊĞíÁ¢¼´ËÀÍö£»¹¥»÷£º²»ÔÊĞíÁ¢¼´ËÀÍö£¨ÒªµÈ¹¥»÷½áÊø£©
        allowImmediateDeath = (phase.Value == 0 || phase.Value == 1);

        return TaskStatus.Success;
    }
}
