// ArchNet · Load Scene Extension — inspector
// Shows the scenes of the build list as a dropdown instead of a text field.
// Docs: https://archnet-plateform.fr/#LoadSceneExtension

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArchNet.Extension.Editor
{
    [CustomEditor(typeof(LoadSceneExtension))]
    public class LoadSceneEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty sceneName = serializedObject.FindProperty("_sceneName");
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            if (scenes.Length == 0)
            {
                EditorGUILayout.HelpBox("No scene in the build list. Add one in File > Build Profiles > Scene List.", MessageType.Warning);
                EditorGUILayout.PropertyField(sceneName);
            }
            else
            {
                int index = System.Array.IndexOf(scenes, sceneName.stringValue);
                if (index < 0 && !string.IsNullOrEmpty(sceneName.stringValue))
                    EditorGUILayout.HelpBox($"'{sceneName.stringValue}' is not in the build list.", MessageType.Warning);
                int picked = EditorGUILayout.Popup("Scene", Mathf.Max(index, 0), scenes);
                if (picked != index) sceneName.stringValue = scenes[picked];
            }

            DrawPropertiesExcluding(serializedObject, "m_Script", "_sceneName");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
