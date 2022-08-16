using UnityEditor;
using UnityEngine;

namespace ArchNet.Extension.LoadScene
{
    /// <summary>
    /// [EXTENSION] - [ARCH NET] - [SCENE LOADER] Editor side
    /// author : LOUIS PAKEL
    /// </summary>
    [CustomEditor(typeof(LoadSceneExtension))]
    public class LoadSceneEditor : Editor
    {
        /// <summary>
        /// This Editor only create a Dropdown effect on a string list
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LoadSceneExtension script = (LoadSceneExtension) target;
            GUIContent scenes = new GUIContent("Scenes List");
            script.SceneIndex = EditorGUILayout.Popup(scenes, script.SceneIndex, script.Scenes);
        }
    }
}