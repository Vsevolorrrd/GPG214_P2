using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
    bool loaded = false;
    public void LoadDesertTile()
    {
        if (loaded) return;

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "tilesbundle");
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return;
        }

        GameObject newTile = bundle.LoadAsset<GameObject>("DesertHex");
        Instantiate(newTile, new Vector3(0, 0, 0), Quaternion.identity);

        bundle.Unload(false); // Keeps loaded assets in memory

        loaded = true;
    }
}