using UnityEngine;
using UnityEngine.UI;
using Game.Foundation;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        if (health == null) health = GetComponentInParent<Health>();
    }

    private void OnEnable()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        EventManager.Subscribe<HealthChangedEvent>(OnHealthChanged);
        Refresh();
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
    }

    private void LateUpdate()
    {
        if (!faceCamera) return;
        if (targetCamera == null) return;
        transform.forward = targetCamera.transform.forward;
    }

    private void OnHealthChanged(HealthChangedEvent evt)
    {
        if (evt.Health != health) return; // 过滤：只响应自己的 Health
        Refresh();
    }

    private void Refresh()
    {
        if (health == null || fillImage == null) return;
        fillImage.fillAmount = health.Normalized;
    }
}