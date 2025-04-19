using UnityEditor;
using UnityEngine;

namespace BuildHelperPlugin
{
    public class BuildConfigCreator : EditorWindow
    {
        private int configCount = 1;

        [MenuItem("Build Helper Plugin/Create Build Configs")]
        public static void ShowWindow()
        {
            GetWindow<BuildConfigCreator>("Create Build Configs");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create Build Configs", EditorStyles.boldLabel);
            configCount = EditorGUILayout.IntField("Number of Configs:", configCount);

            if (configCount < 1)
            {
                configCount = 1;
                EditorGUILayout.HelpBox("Number of configs must be at least 1.", MessageType.Warning);
            }

            if (GUILayout.Button("Create Configs"))
                CreateConfigs();
        }

        private void CreateConfigs()
        {
            string folderPath = "Assets/Build Helper Configs";

            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets", "Build Helper Configs");

            for (int i = 0; i < configCount; i++)
            {
                BuildConfig config = ScriptableObject.CreateInstance<BuildConfig>();
                string assetPath = $"{folderPath}/BuildConfig_{i + 1}.asset";

                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                AssetDatabase.CreateAsset(config, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created {configCount} BuildConfig(s) in {folderPath}");

            Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
            EditorGUIUtility.PingObject(folder);
        }
    }
}
