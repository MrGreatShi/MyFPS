using System;
using UnityEngine;
using Game.Foundation;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [Min(1)]
    public int maxHP = 20;

    [Tooltip("重用(从对象池取出)时是否恢复满血")]
    public bool resetOnEnable = true;

    [Header("Invincibility (Optional)")]
    [Min(0f)]
    public float invincibleSecondsAfterHit = 0f;

    public int CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    float _invincibleUntil;

    private void OnEnable()
    {
        if (resetOnEnable) ResetToFull();
    }

    public void ResetToFull()
    {
        IsDead = false;
        CurrentHP = maxHP;
        _invincibleUntil = 0f;

        EventManager.Publish(new HealthChangedEvent(this, CurrentHP, maxHP));
    }

    public bool CanTakeDamage()
    {
        if (IsDead) return false;
        if (invincibleSecondsAfterHit <= 0f) return true;
        return Time.time >= _invincibleUntil;
    }

    public bool TryTakeDamage(int damage)
    {
        if (damage <= 0) return false;
        if (!CanTakeDamage()) return false;

        int before = CurrentHP;
        CurrentHP = Mathf.Max(0, CurrentHP - damage);

        if (invincibleSecondsAfterHit > 0f)
            _invincibleUntil = Time.time + invincibleSecondsAfterHit;

        EventManager.Publish(new DamagedEvent(this, damage));

        if (CurrentHP != before)
        {
            EventManager.Publish(new HealthChangedEvent(this, CurrentHP, maxHP));
        }

        if (CurrentHP == 0 && !IsDead)
        {
            IsDead = true;
            EventManager.Publish(new DiedEvent(this));
        }
        return true;
    }

    public float Normalized => maxHP <= 0 ? 0 : (float)CurrentHP / maxHP;
}
