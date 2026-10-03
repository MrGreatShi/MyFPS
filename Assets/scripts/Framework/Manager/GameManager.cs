using System.Threading.Tasks;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState currentGameState = GameState.MainMenu;
    [SerializeField] private RunSession runSession;

    public GameState CurrentGameState => currentGameState;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (runSession == null)
            runSession = GetComponentInChildren<RunSession>(true);
    }

    public void UpdateGameState(GameState newState)
    {
        currentGameState = newState;
        // 这里保留你原来的 TimeScale/逻辑也行
    }

    // 提供给 UI 的入口
    public async void StartGameFromHome(int levelId = 1)
    {
        GUIManager.Instance.HideAll(); // 先隐藏所有 UI，进入游戏后再显示 GameWindow
        await runSession.StartRunAsync(levelId);
    }

    public async void RestartRun()
    {
        await runSession.RestartAsync();
    }

    public async void NextLevel(int nextLevelId)
    {
        await runSession.NextLevelAsync(nextLevelId);
    }
    public async Task ReturnToMainMenu()
    {
        await runSession.ReturnMainMenu();
    }
}