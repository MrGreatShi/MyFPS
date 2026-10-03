using UnityEngine;

namespace Game.Foundation
{
    public sealed class TimeEventBootstrap : MonoBehaviour
    {
        [Tooltip("若勾选：在 Awake 时自动常驻不销毁")]
        public bool dontDestroyOnLoad = true;

        private void Awake()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            TimeEventManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}