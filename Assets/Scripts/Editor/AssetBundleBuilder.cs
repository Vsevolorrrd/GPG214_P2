using UnityEditor;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string path = "Assets/StreamingAssets";
        if (!System.IO.Directory.Exists(path))
        System.IO.Directory.CreateDirectory(path);

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}