using UnityEngine;

public class HomeEntry : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            if (GUIManager.Instance == null)
            {
                Debug.LogError("[HomeEntry] GUIManager.Instance is null.");
                return;
            }

            if (!GUIManager.Instance.IsShowing("UI/MainMenuWindow"))
                await GUIManager.Instance.ShowAsync<MainMenuWindow>("UI/MainMenuWindow");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}