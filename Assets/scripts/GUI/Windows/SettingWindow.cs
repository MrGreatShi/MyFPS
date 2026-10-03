using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : UIWindow
{
    public override string Key => "UI/SettingWindow";
    public override UILayer Layer => UILayer.Top;

    public Slider voiceSlider;

    public Slider lightSlider;

    public Button ExitButton;

    public override void OnShow(object args)
    {
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnExitButtonClicked()
    {
        GUIManager.Instance.Hide(Key);
    }

}
