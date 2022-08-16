using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArchNet.Extension.LoadScene
{
    /// <summary>
    /// [EXTENSION] - [ARCH NET] - [SCENE LOADER] Extension Load Scene 
    /// Allow a GameObject to load a scene with a delay or not, with a trigger or from the start
    /// author : LOUIS PAKEL
    /// </summary>
    public class LoadSceneExtension : MonoBehaviour
    {
        [SerializeField] private bool loadOnStart;
        [SerializeField] private float loadAfterSeconds;

        [HideInInspector] public string[] Scenes;
        [HideInInspector] public int SceneIndex;
        
        /// <summary>
        /// IF YOUR SCENE DOESNT APPEAR, VERIFY BUILD SETTINGS OR / AND SAVE CURRENT SCENE, IT MY HELP TO REFRESH THE LIST
        /// </summary>
        private void OnValidate()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            Scenes = new string[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                string scene = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

                if (!Scenes.Contains(scene))
                    Scenes[i] = scene;
            }
        }

        private void Start()
        {
            if (!loadOnStart)
                return;

            StartCoroutine(LoadAfterSeconds());
        }


        private IEnumerator LoadAfterSeconds()
        {
            yield return new WaitForSeconds(loadAfterSeconds);
            SceneManager.LoadScene(Scenes[SceneIndex]);
        }
        
        /// <summary>
        ///  CALL THIS METHOD FOR LOAD A SCENE WITH A SPECIFIC TRIGGER
        /// </summary>
        public void Load()
        {
            StartCoroutine(LoadAfterSeconds());
        }
    }
}