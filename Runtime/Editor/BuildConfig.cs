using UnityEngine;

[CreateAssetMenu(fileName = "BuildConfig", menuName = "Build Config/Build Config", order = 1)]
public class BuildConfig : ScriptableObject
{
    [SerializeField] private string _outputFolder;

    [field: SerializeField] public StoreType StoreType { get; private set; }
    [field: SerializeField] public PackageType PackageType { get; private set; }
    [field: SerializeField] public bool SplitApplicationBinary { get; private set; }
    [field: SerializeField] public string PackageName { get; private set; }
    [field: SerializeField] public string Version { get; private set; }
    [field: SerializeField] public int BundleVersionCode { get; private set; }
    [field: SerializeField] public string KeystorePassword { get; private set; }
    [field: SerializeField] public string AliasPassword { get; private set; }
    public string OutputFolder
    {
        get => string.IsNullOrEmpty(_outputFolder) ? DefaultOutputFolder() : _outputFolder;
        set => _outputFolder = value;
    }

    private string DefaultOutputFolder() => $"Builds/{StoreType}/";

    private void OnValidate() => _outputFolder = DefaultOutputFolder();
}
public enum StoreType
{
    GooglePlay,
    AppGallery,
    RuStore
}

public enum PackageType
{
    APK,
    AAB
}
