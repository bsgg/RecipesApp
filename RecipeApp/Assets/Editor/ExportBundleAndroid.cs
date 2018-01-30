using UnityEditor;
using UnityEngine;
using System.Collections;

public class ExportBundleAndroid : MonoBehaviour 
{
    [MenuItem("Assets/Build AssetBundles Android")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None , BuildTarget.Android);
    }
}
