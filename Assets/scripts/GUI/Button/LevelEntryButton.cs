using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class LevelEntryButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;               
    [SerializeField] private GameObject lockOverlay;   // 可选：锁住时显示

    private int _levelId;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    public void Bind(int levelId, bool interactable, Action<int> onClick)
    {
        _levelId = levelId;

        if (label != null)
            label.text = $"Level {levelId}";

        if (lockOverlay != null)
            lockOverlay.SetActive(!interactable);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = interactable;

            if (interactable && onClick != null)
                button.onClick.AddListener(() => onClick(_levelId));
        }
    }
}
