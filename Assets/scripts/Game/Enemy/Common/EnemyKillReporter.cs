using UnityEngine;
using Game.Foundation;

public class EnemyKillReporter : MonoBehaviour
{
    [SerializeField] private Health health;
    private bool _reported;

    private void Awake()
    {
        if (health == null) health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _reported = false;
        // 监听全局死亡事件后过滤
        EventManager.Subscribe<DiedEvent>(OnAnyDied);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<DiedEvent>(OnAnyDied);
    }

    private void OnAnyDied(DiedEvent evt)
    {
        if (_reported) return;
        if (evt.Health != health) return; // 只关心自己的死亡

        _reported = true;
        EventManager.Publish(new EnemyKilledEvent(this, gameObject));
    }
}