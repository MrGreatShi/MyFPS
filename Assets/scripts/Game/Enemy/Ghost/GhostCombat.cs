using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCombat : EnemyCombat
{
    public override void TryAttack()
    {
        //向前方发射一个射线，长度为攻击范围，检测是否击中玩家
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, AttackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // 这里你可以调用玩家的受伤方法，例如 hit.collider.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
                hit.collider.GetComponent<Health>()?.TryTakeDamage(damageAmount);
            }
        }
    }
}
