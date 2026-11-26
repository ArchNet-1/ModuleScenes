// ArchNet · Load Scene Extension — scene loader
// Loads a scene in the background behind a loading screen (progress bar, a random tip), shown
// at least a moment so it does not flash. The Hub creates GameLoader (your class):
//
//   Services.Get<GameLoader>().LoadScene(GameLoader.Gameplay);
//
// Time scale is reset to 1 after a load, so leaving a paused game never freezes the next scene.
// Docs: https://archnet-plateform.fr/#ModuleScenes

using System;
using System.Collections;
using ArchNet.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArchNet.Extension
{
    public class SceneLoader : GameService
    {
        [SerializeField] LoadingView _view;
        [Tooltip("Shortest time the loading screen stays on screen, in seconds.")]
        [SerializeField, Min(0f)] float _minimumDuration = 0.6f;
        [Tooltip("One is picked at random for each load.")]
        [SerializeField, TextArea] string[] _tips =
        {
            "Tip: settings are saved as soon as you close the menu.",
            "Tip: your progress is saved when you leave the game.",
        };

        public bool IsLoading { get; private set; }

        /// <summary>Raised once the new scene is active and the loading screen gone.</summary>
        public event Action<string> SceneLoaded;

        /// <summary>Starts loading a scene of the build list. Returns false when it cannot.</summary>
        public bool LoadScene(string sceneName)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"[ArchNet] Already loading a scene; \"{sceneName}\" is ignored.", this);
                return false;
            }
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[ArchNet] Scene \"{sceneName}\" is not in the build list: add it in File > Build Profiles > Scene List (the Hub's Menu and Gameplay services add theirs).", this);
                return false;
            }
            StartCoroutine(Load(sceneName));
            return true;
        }

        IEnumerator Load(string sceneName)
        {
            IsLoading = true;
            float start = Time.unscaledTime;
            if (_view != null)
            {
                _view.Show(_tips != null && _tips.Length > 0 ? _tips[UnityEngine.Random.Range(0, _tips.Length)] : null);
                _view.SetProgress(0f);
                yield return null; // let the screen draw before the heavy work starts
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
            {
                if (_view != null) _view.SetProgress(operation.progress / 0.9f);
                yield return null;
            }
            if (_view != null) _view.SetProgress(1f);
            while (Time.unscaledTime - start < _minimumDuration) yield return null;

            operation.allowSceneActivation = true;
            while (!operation.isDone) yield return null;

            Time.timeScale = 1f;
            if (_view != null) _view.Hide();
            IsLoading = false;
            OnSceneLoaded(sceneName);
        }

        /// <summary>Called when a scene is loaded. Override it to start music, analytics…</summary>
        protected virtual void OnSceneLoaded(string sceneName) => SceneLoaded?.Invoke(sceneName);
    }
}
