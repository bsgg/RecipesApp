using UnityEditor;
using UnityEngine;
using System.Collections;

public class ExportBundleIOS : MonoBehaviour 
{
    [MenuItem("Assets/Build AssetBundles IOS")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None , BuildTarget.iOS);
    }
}
