using UnityEngine;

namespace Game.Foundation
{
    public readonly struct HealthChangedEvent
    {
        public readonly Health Health;
        public readonly int Current;
        public readonly int Max;

        public HealthChangedEvent(Health health, int current, int max)
        {
            Health = health;
            Current = current;
            Max = max;
        }
    }

    public readonly struct DamagedEvent
    {
        public readonly Health Health;
        public readonly int Damage;

        public DamagedEvent(Health health, int damage)
        {
            Health = health;
            Damage = damage;
        }
    }

    public readonly struct DiedEvent
    {
        public readonly Health Health;
        public readonly GameObject Target;

        public DiedEvent(Health health)
        {
            Health = health;
            Target = health != null ? health.gameObject : null;
        }
    }

    public readonly struct EnemyKilledEvent
    {
        public readonly EnemyKillReporter Reporter;
        public readonly GameObject Enemy;

        public EnemyKilledEvent(EnemyKillReporter reporter, GameObject enemy)
        {
            Reporter = reporter;
            Enemy = enemy;
        }
    }
}
