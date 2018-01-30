using LitJson;
using RecipeApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{

    [System.Serializable]
    public class IndexFile
    {
        public string FileName;
        public string URL;
        public string Data;
    }

    [System.Serializable]
    public class FileData
    {
        public List<IndexFile> Data;
        public FileData()
        {
            Data = new List<IndexFile>();
        }
    }

    public class FileRequestManager : MonoBehaviour
    {
        #region Instance
        private static FileRequestManager m_Instance;
        public static FileRequestManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (FileRequestManager)FindObjectOfType(typeof(FileRequestManager));

                    if (m_Instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(FileRequestManager) + " is needed in the scene, but there is none.");
                    }
                }
                return m_Instance;
            }
        }
        #endregion Instance

        [SerializeField]
        private string m_FileDataUrl = "http://beatrizcv.com/Data/FileData.json";

        [SerializeField] private string m_FolderName = "CookingTime";


        [SerializeField]
        private FileData m_FileData;
        public FileData FileData
        {
            get
            {
                return m_FileData;
            }
        }

        private float m_PercentProgress;
        private string m_ProgressText;

        public string ProgressText
        {
            get { return m_ProgressText; }
        }
        
        [SerializeField]
        private List<RecipeModel> m_RecipeList;
        public IEnumerator RequestFiles()
        {
            m_RecipeList = new List<RecipeModel>();
            m_FileData = new FileData();
            m_PercentProgress = 0.0f;
            m_ProgressText = m_PercentProgress.ToString() + " % ";

            if (string.IsNullOrEmpty(m_FileDataUrl))
            {
                Debug.LogWarning("<color=yellow>" + "[FileRequestManager] File Data Url is null or empty" + "</color>");
                yield return null;
            }

            WWW wwwFile = new WWW(m_FileDataUrl);
            yield return wwwFile;
            string jsonData = wwwFile.text;
            if (!string.IsNullOrEmpty(jsonData))
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    m_FileData = JsonMapper.ToObject<FileData>(jsonData);

                    Debug.LogWarning("<color=yellow>" + "[FileRequestManager] Requesting... " + m_FileData.Data.Count + " Files " + "</color>");
                    for (int i = 0; i < m_FileData.Data.Count; i++)
                    {
                        if (string.IsNullOrEmpty(m_FileData.Data[i].URL))
                        {
                            continue;
                        }

                        // Request
                        Debug.LogWarning("<color=yellow>" + "[FileRequestManager] Requesting: " + (i + 1) + "/" + m_FileData.Data.Count + " : " + m_FileData.Data[i].FileName + "</color>");
                        WWW www = new WWW(m_FileData.Data[i].URL);
                        while (!www.isDone)
                        {
                            m_PercentProgress = www.progress * 100.0f;
                            m_ProgressText = m_PercentProgress.ToString() + " % ";
                            yield return null;
                        }

                        m_PercentProgress = www.progress * 100.0f;
                        m_ProgressText = m_PercentProgress.ToString() + " % ";

                        m_FileData.Data[i].Data = www.text;


                        Debug.LogWarning("<color=yellow>" + "[FileRequestManager] Data Retrieved: " + m_FileData.Data[i].Data +  "</color>");
                    }
                }
            }else
            {
                Debug.LogWarning("<color=yellow>" + "[FileRequestManager] File Data Json is null or empty" + "</color>");
            }            
        }

        private IEnumerator LoadAssetBundles()
        {
            // Start a download of the given URL
            WWW www = new WWW("file://" + "test");

            // Wait for download to complete
            yield return www;

            // Load and retrieve the AssetBundle
            AssetBundle bundle = www.assetBundle;

            // Load the object asynchronously
            AssetBundleRequest request = bundle.LoadAllAssetsAsync();

            // Wait for completion
            yield return request;

            // Get the reference to the loaded object
            if (request.allAssets != null)
            {
                ProcessAssetBundleRequest(request, "test");
            }
            else
            {
                Debug.LogWarning("<color=yellow>" + "[LoadAssetBundles] Could not load objects in theatre bundle" + "</color>");
            }

            // Unload the AssetBundles compressed contents to conserve memory
            bundle.Unload(false);

            // Frees the memory from the web stream
            www.Dispose();

        }

        private Transform m_AssetParent;

        private void ProcessAssetBundleRequest(AssetBundleRequest request, string assetID)
        {
            Debug.Log(string.Format("Successfully loaded {0} objects", request.allAssets.Length));

            try
            {
                m_AssetParent.parent.gameObject.SetActive(true);

                //Create parent group object and make sure it sits centre of the marker
                GameObject assetIDParent = new GameObject(assetID);
                assetIDParent.transform.SetParent(m_AssetParent);
                assetIDParent.transform.localPosition = Vector3.zero;
                assetIDParent.transform.localRotation = Quaternion.identity;
                assetIDParent.transform.localScale = Vector3.one;
                assetIDParent.SetActive(true);

                //Instantiate each of the loaded objects and add them to the group
                foreach (UnityEngine.Object o in request.allAssets)
                {
                    GameObject go = o as GameObject;
                    GameObject instantiatedGO = Instantiate(go);

                    instantiatedGO.transform.SetParent(assetIDParent.transform);
                    instantiatedGO.transform.localPosition = Vector3.zero;
                    instantiatedGO.transform.localRotation = Quaternion.identity;
                    instantiatedGO.transform.localScale = Vector3.one;

                    /*TheatreObject to = GetTheatreObjectWithAssetID(assetID);

                    if (to == null)
                    {
                        ARConsole.LogError("Theatre object not created before assets were requested: " + assetID);
                        to = CreateTheatreObject(assetID, string.Empty);
                    }

                    to.instance = instantiatedGO;*/
                    instantiatedGO.SetActive(false);

                   // StartCoroutine(TheatrePieceFrameDelay(to));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("<color=yellow>" + "[LoadAssetBundles] Failed to load asset bundle, reason: " + e.Message + "</color>");
            }
        }


    }
}
