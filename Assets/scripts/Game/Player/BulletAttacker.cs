using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttacker : Attacker
{
    protected override bool isAttackable(GameObject go)
    {
        return go.CompareTag("Enemy");
    }
}
