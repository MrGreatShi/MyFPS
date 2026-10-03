using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultWindow : UIWindow
{
    public override string Key => "UI/ResultWindow";
    public override UILayer Layer => UILayer.Popup;

    public TextMeshProUGUI ResultText;
    public Button RestartButton;
    public Button MainMenuButton;
    public Button NextLevelButton;
    private bool isClicked = false;
    public override void OnShow(object args)
    {
        // 显示结果界面时暂停游戏
        Time.timeScale = 0f;
        if (args is bool isVictory)
        {
            ResultText.text = isVictory ? "Victory!" : "Defeat!";
        }
        else
        {
            ResultText.text = "Result";
        }
        RestartButton.onClick.AddListener(OnRestartClick);
        MainMenuButton.onClick.AddListener(OnMainMenuClick);
        if(args is bool victory && victory &&
            LevelManager.Instance.CurrentLevelConfig != null && LevelManager.Instance.CurrentLevelConfig.nextLevelId > 0)
        {
            NextLevelButton.enabled = true;
            NextLevelButton.onClick.AddListener(OnNextLevelClick);
        }
        else
        {
            NextLevelButton.enabled = false;
        }
        isClicked = false; // 重置点击状态  
    }

    public override void OnHide()
    {
        RestartButton.onClick.RemoveListener(OnRestartClick);
        MainMenuButton.onClick.RemoveListener(OnMainMenuClick);
        NextLevelButton.onClick.RemoveListener(OnNextLevelClick);
    }
    private void OnRestartClick()
    {
        if (isClicked) return; // 防止重复点击
        isClicked = true;
        Time.timeScale = 1f;
        GameManager.Instance.RestartRun();
        GUIManager.Instance.Hide(Key);
    }

    private async void OnMainMenuClick()
    {
        if (isClicked) return; // 防止重复点击
        isClicked = true;
        Time.timeScale = 1f;
        await GameManager.Instance.ReturnToMainMenu();
    }
    private void OnNextLevelClick()
    {
        if (isClicked) return; // 防止重复点击
        isClicked = true;
        Time.timeScale = 1f;
        var next = LevelManager.Instance.CurrentLevelConfig.nextLevelId;
        GameManager.Instance.NextLevel(next);
        GUIManager.Instance.Hide(Key);
    }
}
