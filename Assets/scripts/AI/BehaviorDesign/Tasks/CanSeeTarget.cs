using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy/Perception")]
public class CanSeeTarget : Conditional
{
    [Header("Target")]
    public SharedTransform target;

    [Header("Distance")]
    public float minSenceDistance = 1f;
    public float viewDistance = 5f;

    public float fovAngle = 90f;

    public float eyeHeight = 1.6f;
    public float targetHeight = 1.0f;

    public LayerMask obstacleMask = ~0;

    public bool ignoreTriggers = true;
    [Header("Interval between next attacks")]
    public SharedFloat NextAttack = 0;

    public override TaskStatus OnUpdate()
    {
        if (target == null || target.Value == null) return TaskStatus.Failure;
        if (NextAttack.Value > Time.time)
        {
            return TaskStatus.Failure;
        }

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = target.Value.position + Vector3.up * targetHeight;

        Vector3 toTarget = targetPos - eyePos;
        float dist = toTarget.magnitude;

        // 1) 距离限制
        if (dist < minSenceDistance) return TaskStatus.Success;
        if (dist > viewDistance) return TaskStatus.Failure;
        if (dist <= 0.0001f) return TaskStatus.Success;

        // 2) 角度限制（前方 90°）
        float halfFov = Mathf.Clamp(fovAngle * 0.5f, 0f, 180f);
        float angle = Vector3.Angle(transform.forward, toTarget);
        if (angle > halfFov) return TaskStatus.Failure;

        // 3) 遮挡检测（视线）
        Vector3 dir = toTarget / dist;

        var query = ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;

        // 我们用 obstacleMask 来检测障碍物：
        // - 如果射线先打到障碍物 => 看不见
        // - 如果没有打到任何障碍物 => 认为看得见（或你也可以改成必须命中目标的 collider）
        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, dist, obstacleMask, query))
        {
            // 射线在到达目标前命中障碍物
            return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}