using Game.Foundation;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunSession : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game1";

    private VictorySystem _victory;
    private SpawnerController _spawner;
    private GameObject _player;

    private bool _sceneHooked;

    private void OnEnable()
    {
        if (_sceneHooked) return;
        _sceneHooked = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (_sceneHooked)
        {
            _sceneHooked = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public async Task StartRunAsync(int levelId)
    {
        await LevelManager.Instance.LoadLevelAsync(levelId);
        gameSceneName = LevelManager.Instance.CurrentLevelConfig.sceneName;
        await SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single).AsTask();
        BeginRunAfterSceneReady();
    }

    public async Task RestartAsync()
    {
        int levelId = LevelManager.Instance.CurrentLevelId;
        await SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single).AsTask();
        BeginRunAfterSceneReady(levelId);
    }

    public async Task NextLevelAsync(int nextLevelId)
    {
        await StartRunAsync(nextLevelId);
    }

    public async Task ReturnMainMenu()
    {
        GUIManager.Instance.HideAll();
        await SceneManager.LoadSceneAsync("home", LoadSceneMode.Single).AsTask();
        // 这里可以添加一些清理逻辑，例如重置 LevelManager 状态等
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != gameSceneName) return;

        _player = GameObject.FindGameObjectWithTag("Player");       
        _victory = FindFirstObjectByType<VictorySystem>();
        _spawner = FindFirstObjectByType<SpawnerController>();

        if (_victory != null)
        {
            EventManager.Subscribe<VictoryEvent>(HandleVictory);
            EventManager.Subscribe<DefeatEvent>(evt => HandleDefeat());
        }
    }

    private async void BeginRunAfterSceneReady(int? forcedLevelId = null)
    {
        await Task.Yield();

        var levelConfig = LevelManager.Instance.CurrentLevelConfig;
        if (levelConfig == null)
        {
            Debug.LogError("RunSession: CurrentLevelConfig is null.");
            return;
        }

        if (_victory != null)
            _victory.SetConfig(levelConfig.victory);

        if (_spawner != null)
        {
            _spawner.Clear();
            await _spawner.ApplyLevelConfigAsync(levelConfig);
        }

        if (_player != null)
        {
            ActivatePlayer();
            _player.GetComponent<FirstPersonCtrl>()?.Reset();

            var hp = _player.GetComponent<Health>();
            if (hp != null) hp.ResetToFull();
        }

        var playerHealth = _player != null ? _player.GetComponent<Health>() : null;

        GUIManager.Instance.Hide("UI/GameWindow");
        await GUIManager.Instance.ShowAsync<GameWindow>(
            "UI/GameWindow",
            new GameWindowArgs
            {
                PlayerHealth = playerHealth,
                Victory = _victory
            });

        _victory?.StartRun();
        _spawner?.StartAll();

        Time.timeScale = 1f;
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    private async void HandleVictory(VictoryEvent evt)
    {
        FinishCurrentGame();
        if(LevelManager.Instance.CurrentLevelConfig != null && LevelManager.Instance.CurrentLevelConfig.levelId == LevelManager.Instance.MaxPassLevel)
        {
            LevelManager.Instance.MaxPassLevel++;
        }
        await GUIManager.Instance.ShowAsync<ResultWindow>("UI/ResultWindow", true);
    }

    private async void HandleDefeat()
    {
        FinishCurrentGame();
        await GUIManager.Instance.ShowAsync<ResultWindow>("UI/ResultWindow", false);
    }

    private void FinishCurrentGame()
    {
        _spawner?.StopAllAndDespawn();
        _victory?.Pause();
        DeactivatePlayer();
        GameManager.Instance.UpdateGameState(GameState.GameOver);
    }

    private void ActivatePlayer()
    {
        if (_player == null) return;

        // 启用 FirstPersonCtrl（玩家移动/视角等）
        var fp = _player.GetComponent<FirstPersonCtrl>();
        if (fp != null)
            fp.enabled = true;

        // 启用 GunController（开火等）
        var gun = _player.GetComponentInChildren<GunController>(true);
        if (gun != null)
            gun.enabled = true;
    }
    private void DeactivatePlayer()
    {
        if (_player == null) return;

        // 禁用 FirstPersonCtrl（玩家移动/视角等）
        var fp = _player.GetComponent<FirstPersonCtrl>();
        if (fp != null)
            fp.enabled = false;

        // 禁用 GunController（开火等）
        var gun = _player.GetComponentInChildren<GunController>(true);
        if (gun != null)
            gun.enabled = false;
    }
}