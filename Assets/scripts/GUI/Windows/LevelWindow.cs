using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelWindow : UIWindow
{
    public override string Key => "UI/LevelWindow";
    public override UILayer Layer => UILayer.Normal;

    [Header("Buttons")]
    public Button ExitButton;

    [Header("Level Grid")]
    [SerializeField] private Transform LevelGridRoot;            // GridLayoutGroup 所在节点
    [SerializeField] private LevelEntryButton LevelButtonPrefab; // 关卡按钮模板

    private readonly List<LevelEntryButton> _spawned = new();

    public override void OnShow(object args)
    {
        ExitButton.onClick.AddListener(OnExitButtonClicked);
        BuildLevelButtons();
    }

    public override void OnHide()
    {
        ExitButton.onClick.RemoveListener(OnExitButtonClicked);
        ClearLevelButtons();
    }

    private void OnExitButtonClicked()
    {
        GUIManager.Instance.Hide(Key);
    }

    private void BuildLevelButtons()
    {
        ClearLevelButtons();

        var lm = LevelManager.Instance;
        if (lm == null)
        {
            Debug.LogError("LevelManager.Instance is null.");
            return;
        }
        if (LevelGridRoot == null)
        {
            Debug.LogError("LevelGridRoot is null.");
            return;
        }
        if (LevelButtonPrefab == null)
        {
            Debug.LogError("LevelButtonPrefab is null.");
            return;
        }

        int total = lm.TotalLevels;
        int maxPass = lm.MaxPassLevel; 

        // 生成 1..total
        for (int levelId = 1; levelId <= total; levelId++)
        {
            bool interactable = levelId <= maxPass;

            var btn = Instantiate(LevelButtonPrefab, LevelGridRoot);
            _spawned.Add(btn);

            btn.Bind(levelId, interactable, OnLevelButtonClicked);
        }
    }

    private void ClearLevelButtons()
    {
        for (int i = 0; i < _spawned.Count; i++)
        {
            if (_spawned[i] != null)
                Destroy(_spawned[i].gameObject);
        }
        _spawned.Clear();
    }

    private void OnLevelButtonClicked(int levelId)
    {

        try
        {
            Debug.Log($"Level loaded: {levelId}");
            // 进入战斗场景/创建关卡内容（看你的项目架构）
            GameManager.Instance.StartGameFromHome(levelId);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load level failed: {levelId}\n{e}");
        }
    }
}
