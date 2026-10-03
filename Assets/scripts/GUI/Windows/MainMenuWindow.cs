using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : UIWindow
{
    public override string Key => "UI/MainMenuWindow";
    public override UILayer Layer => UILayer.Normal;

    public Button StartButton;
    public Button ExitButton;

    public Button SettingButton;
    public override void OnCreate()
    {

    }

    public override void OnShow(object args)
    {
        if (StartButton != null)
            StartButton.onClick.AddListener(OnStartClicked);
        if (ExitButton != null)
            ExitButton.onClick.AddListener(OnExitClicked);
        if (SettingButton != null)
            SettingButton.onClick.AddListener(OnSettingClicked);
    }

    public override void OnHide()
    {
        if (StartButton != null)
            StartButton.onClick.RemoveListener(OnStartClicked);
        if (ExitButton != null)
            ExitButton.onClick.RemoveListener(OnExitClicked);
        if (SettingButton != null)
            SettingButton.onClick.RemoveListener(OnSettingClicked);
    }

    private async void OnStartClicked()
    {
        await GUIManager.Instance.ShowAsync<LevelWindow>("UI/LevelWindow");
    }

    private async void OnSettingClicked()
    {
        await GUIManager.Instance.ShowAsync<SettingWindow>("UI/SettingWindow");
    }
    private void OnExitClicked()
    {
        Application.Quit();
    }
}
