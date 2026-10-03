using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject appRootPrefab;
    [SerializeField] private string homeSceneName = "Home";

    private static bool _booted;

    private async void Awake()
    {
        if (_booted)
        {
            Destroy(gameObject);
            return;
        }
        _booted = true;
        DontDestroyOnLoad(gameObject);

        // 1) 初始化 Addressables（虽然不远程更新，但建议做一次）
        await Addressables.InitializeAsync().Task;

        // 2) 创建 AppRoot（ManagersRoot）
        EnsureAppRoot();

        // 3) 进入 Home
        await SceneManager.LoadSceneAsync(homeSceneName, LoadSceneMode.Single).AsTask();
    }

    private void EnsureAppRoot()
    {
        if (GameManager.Instance != null) return;

        var root = Instantiate(appRootPrefab);
        root.name = "AppRoot";
        DontDestroyOnLoad(root);
    }
}