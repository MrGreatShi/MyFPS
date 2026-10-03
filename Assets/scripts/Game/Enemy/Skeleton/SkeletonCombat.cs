using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCombat : EnemyCombat
{
    public override void TryAttack()
    {
        //判断范围攻击距离内是否有玩家，有玩家则造成伤害
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AttackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 这里你可以调用玩家的受伤方法，例如 hitCollider.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
                hitCollider.GetComponent<Health>()?.TryTakeDamage(damageAmount);
            }
        }
    }
}
