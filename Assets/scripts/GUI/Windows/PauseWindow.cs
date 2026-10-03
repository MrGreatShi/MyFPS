using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : UIWindow
{
    public override string Key => "UI/PauseWindow";
    public override UILayer Layer => UILayer.Top;

    public Button MainMenuButton;

    public Button RestartButton;

    public Button ExitButton;
     public override void OnShow(object args)
    {
        Time.timeScale = 0f; // ‘›Õ£”Œœ∑
        MainMenuButton.onClick.AddListener(OnMainMenuClick);
        RestartButton.onClick.AddListener(OnRestartClick);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void OnHide()
    {
        Time.timeScale = 1f; // ª÷∏¥”Œœ∑
        MainMenuButton.onClick.RemoveListener(OnMainMenuClick);
        RestartButton.onClick.RemoveListener(OnRestartClick);
        ExitButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    private void OnRestartClick()
    {
        Time.timeScale = 1f;
        GameManager.Instance.RestartRun();
        GUIManager.Instance.Hide(Key);
    }

    private async void OnMainMenuClick()
    {
        Time.timeScale = 1f;
        GUIManager.Instance.Hide(Key);
        await GameManager.Instance.ReturnToMainMenu();
    }

    private void OnExitButtonClicked()
    {
        GUIManager.Instance.Hide(Key);
    }
}
