// ArchNet · Load Scene Extension
// Loads a scene from the build list: on start, after a delay, or when Load() is called
// (from a button's OnClick for example). The scene is picked by name in the inspector.
// Docs: https://archnet-plateform.fr/#ModuleScenes

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArchNet.Extension
{
    [AddComponentMenu("ArchNet/Load Scene")]
    public class LoadSceneExtension : MonoBehaviour
    {
        [Tooltip("Scene to load. It must be in File > Build Profiles > Scene List.")]
        [SerializeField] string _sceneName;
        [SerializeField] bool _loadOnStart;
        [Min(0)]
        [SerializeField] float _delay;

        public string SceneName { get => _sceneName; set => _sceneName = value; }

        void Start()
        {
            if (_loadOnStart) Load();
        }

        /// <summary>Loads the chosen scene after the delay.</summary>
        public void Load() => Load(_sceneName);

        /// <summary>Loads a scene by name after the delay.</summary>
        public void Load(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[ArchNet] Scene '{sceneName}' is not in the build's scene list.", this);
                return;
            }
            StopAllCoroutines();
            StartCoroutine(LoadAfterDelay(sceneName));
        }

        IEnumerator LoadAfterDelay(string sceneName)
        {
            if (_delay > 0f) yield return new WaitForSeconds(_delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}
