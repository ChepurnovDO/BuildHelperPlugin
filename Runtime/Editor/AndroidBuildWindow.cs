using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;

public class AndroidBuildWindow : EditorWindow
{
    private BuildConfig _config;
    private string[] _configPaths;
    private int _selectedConfigIndex;

    [MenuItem("Tools/Build Helper Plugin")]
    public static void ShowWindow() => GetWindow<AndroidBuildWindow>("Build Helper Plugin");

    private void OnEnable() => LoadConfigs();

    private void LoadConfigs()
    {
        _configPaths = AssetDatabase.FindAssets("t:BuildConfig")
        .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
        .ToArray();
    }

    private void OnGUI()
    {
        GUILayout.Label("Build Helper Plugin", EditorStyles.boldLabel);

        if (_configPaths.Length == 0)
        {
            EditorGUILayout.HelpBox("No Build Configs found. Create one in Assets/Create/Build Config/Build Config.", MessageType.Warning);
            return;
        }

        _selectedConfigIndex = EditorGUILayout.Popup("Selected Config", _selectedConfigIndex, _configPaths.Select(Path.GetFileNameWithoutExtension).ToArray());
        _config = AssetDatabase.LoadAssetAtPath<BuildConfig>(_configPaths[_selectedConfigIndex]);

        if (_config == null) 
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Store Type", _config.StoreType.ToString());
        EditorGUILayout.LabelField("Package Type", _config.PackageType.ToString());
        EditorGUILayout.LabelField("Package Name", _config.PackageName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Version", _config.Version);
        EditorGUILayout.LabelField("Bundle Version Code", _config.BundleVersionCode.ToString());
        EditorGUILayout.LabelField("Split Application Binary", _config.SplitApplicationBinary.ToString());


        EditorGUILayout.LabelField("Output Folder", _config.OutputFolder);

        EditorGUILayout.Space();
        if (GUILayout.Button("Build"))
            Build();
    }

    private void Build()
    {
        PlayerSettings.bundleVersion = _config.Version;
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.applicationIdentifier = _config.PackageName;
        PlayerSettings.Android.keyaliasPass = _config.AliasPassword;
        PlayerSettings.Android.keystorePass = _config.KeystorePassword;
        PlayerSettings.Android.bundleVersionCode = _config.BundleVersionCode;
        PlayerSettings.Android.useAPKExpansionFiles = _config.SplitApplicationBinary;

        BuildOptions options = BuildOptions.None;
        string extension = _config.PackageType == PackageType.APK ? "apk" : "aab";
        string outputPath = Path.Combine(_config.OutputFolder, $"{_config.PackageName}_{_config.BundleVersionCode}({_config.Version}).{extension}");

        if (!Directory.Exists(_config.OutputFolder))
            Directory.CreateDirectory(_config.OutputFolder);

        EditorUserBuildSettings.buildAppBundle = _config.PackageType == PackageType.AAB;

        switch (_config.StoreType)
        {
            case StoreType.GooglePlay:
                break;
            case StoreType.AppGallery:
                break;
            case StoreType.RuStore:
                break;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = options
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"Build completed: {outputPath}");
    }
}
