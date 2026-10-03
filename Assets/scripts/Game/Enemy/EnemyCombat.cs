using System;
using UnityEngine;

public abstract class EnemyCombat : MonoBehaviour
{
    public float windupSeconds = 1f;  // 蓄力
    public float recoverSeconds = 0.5f; // 收招
    public string attackTrigger = "Attack";
    public int damageAmount = 1; // 攻击伤害

    public bool IsAttacking { get; private set; }
    public event Action OnAttackFinished;

    public float AttackRange = 1.2f; // 攻击范围

    private float _t;
    private Animator _anim;
    private bool _hitApplied = false; // 确保只执行一次命中判定

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void StartAttack()
    {
        if (IsAttacking) return;
        IsAttacking = true;
        _t = 0f;
        _anim.SetTrigger(attackTrigger);
    }

    private void Update()
    {
        if (!IsAttacking) return;

        _t += Time.deltaTime;

        // 在 windupSeconds 时刻执行命中判定（你可接近战判定/发射子弹等）
        // if (_t >= windupSeconds && !_hitApplied) ApplyHit();

        if (_t >= windupSeconds + recoverSeconds)
        {
            _hitApplied = false; // 重置命中标志位，为下一次攻击做准备
            IsAttacking = false;
            OnAttackFinished?.Invoke();
        }
        else if(_t >= windupSeconds && !_hitApplied)
        {
            _hitApplied = true;
            TryAttack();
        }
    }
    public virtual void TryAttack(){}
}