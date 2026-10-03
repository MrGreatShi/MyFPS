using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Foundation;

public class GameWindow : UIWindow
{
    public override string Key => "UI/GameWindow";
    public override UILayer Layer => UILayer.Normal;

    public TextMeshProUGUI StatsTxt;
    public Slider HealthSlider;
    public TextMeshProUGUI HealthTxt;
    public Image HitImage;
    public Button PauseButton;

    private Health _health;
    private VictorySystem _victory;

    public override void OnCreate()
    {
        // 只做UI自身初始化
        if (HitImage != null)
            HitImage.gameObject.SetActive(false);
    }

    public override void OnShow(object args)
    {
        if (HitImage != null)
            HitImage.gameObject.SetActive(false);

        // 1) 解析注入参数
        var payload = args as GameWindowArgs;
        if (payload == null)
        {
            Debug.LogError("[GameWindow] OnShow requires GameWindowArgs payload.");
            return;
        }

        _health = payload.PlayerHealth;
        _victory = payload.Victory;

        // 2) 首次刷新
        UpdateHealthUI();
        UpdateGameStats(new ProgressUpdateEvent { Condition = null });

        // 3) 订阅事件（先移除再添加，避免重复订阅）
        EventManager.Subscribe<ProgressUpdateEvent>(UpdateGameStats);

        EventManager.Subscribe<HealthChangedEvent>(HandleHpChanged);

        // 4) 按钮事件
        PauseButton.onClick.AddListener(OnPauseClick);
    }

    public override void OnHide()
    {   
        // 解绑，避免重开一局订阅旧对象
        EventManager.Unsubscribe<ProgressUpdateEvent>(UpdateGameStats);

        EventManager.Unsubscribe<HealthChangedEvent>(HandleHpChanged);

        _victory = null;
        _health = null;

        PauseButton.onClick.RemoveListener(OnPauseClick);
    }

    private async void OnPauseClick()
    {
        await GUIManager.Instance.ShowAsync<PauseWindow>("UI/PauseWindow");
    }

    public void ShowHitEffect()
    {
        if (HitImage == null) return;
        HitImage.gameObject.SetActive(true);
        // 可以添加淡出动画等效果，这里简单显示一下
        Invoke(nameof(HideHitEffect), 0.3f); // 0.3秒后隐藏
    }

    private void HideHitEffect()
    {
        if (HitImage == null) return;
        HitImage.gameObject.SetActive(false);
    }

    // 修改 HandleHpChanged 签名以匹配 Action<HealthChangedEvent>
    private void HandleHpChanged(HealthChangedEvent evt)
    {
        if (evt.Health != _health) return; // 只处理玩家的血量变化
        if (evt.Current > 0) ShowHitEffect();
        UpdateHealthUI(evt.Current, evt.Max);
    }

    private void UpdateHealthUI()
    {
        if (_health == null) return;
        UpdateHealthUI(_health.CurrentHP, _health.maxHP);
    }

    private void UpdateHealthUI(int curHp, int maxHp)
    {
        if (HealthSlider != null)
            HealthSlider.value = (maxHp <= 0) ? 0f : (float)curHp / maxHp;

        if (HealthTxt != null)
            HealthTxt.text = $"{curHp} / {maxHp}";
    }

    private void UpdateGameStats(ProgressUpdateEvent evt)
    {
        if (StatsTxt == null) return;

        if (_victory == null)
        {
            StatsTxt.text = "";
            return;
        }

        var stats = _victory.Stats;
        var conditions = _victory.GetConditions();

        if (conditions == null || conditions.Count == 0)
        {
            StatsTxt.text = "";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder(128);
        foreach (var condition in conditions)
        {
            if (condition == null) continue;
            sb.AppendLine(condition.GetProgressText(stats));
        }
        StatsTxt.text = sb.ToString();
    }
}