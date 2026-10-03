using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    public int CurrentLevelId = -1; // -1 表示未加载任何关卡
    public LevelConfig CurrentLevelConfig { get; private set; }

    public int TotalLevels { get; } = 10; // 可选：总关卡数，用于边界检查
    public int MaxPassLevel = 10; 

    // 只保留当前关卡的 handle，用于释放旧关卡
    private AsyncOperationHandle<LevelConfig>? _currentHandle;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        ReleaseCurrent();
    }

    /// <summary>
    /// 加载关卡配置（Addressables key 例如：LevelConfig_001）
    /// 只保留当前关卡配置：加载新关卡时会释放旧 handle。
    /// </summary>
    public async Task<LevelConfig> LoadLevelAsync(int levelId)
    {
        Debug.Log($"Loading level {levelId}...");
        string key = GetLevelAddress(levelId);

        // 释放旧关卡
        ReleaseCurrent();

        var handle = Addressables.LoadAssetAsync<LevelConfig>(key);
        _currentHandle = handle;

        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            ReleaseCurrent();
            throw new Exception($"Failed to load level config: {key}");
        }

        CurrentLevelConfig = handle.Result;
        CurrentLevelId = CurrentLevelConfig.levelId;
        return CurrentLevelConfig;
    }

    public Task<LevelConfig> ReloadCurrentAsync()
    {
        return LoadLevelAsync(CurrentLevelId);
    }

    public Task<LevelConfig> LoadNextLevelAsync()
    {
        return LoadLevelAsync(CurrentLevelId + 1);
    }

    private string GetLevelAddress(int levelId)
        => $"LevelConfig_{levelId:D3}";

    private void ReleaseCurrent()
    {
        if (_currentHandle.HasValue)
        {
            var h = _currentHandle.Value;
            if (h.IsValid())
                Addressables.Release(h);
            _currentHandle = null;
        }
        CurrentLevelConfig = null;
    }

}