using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace RecipeApp
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

    public class LauncherControl : Base
    {

        public delegate void LauncherAction();
        public event LauncherAction OnGetDataEnd;

        [SerializeField]
        private string m_FileDataUrl = "http://beatrizcv.com/Data/FileData.json";


        [SerializeField]
        private FileData m_FileData;
        public FileData FileData
        {
            get
            {
                return m_FileData;
            }
        }

        [SerializeField]
        private LauncherUI m_LauncherUI;


        private float m_PercentProgress;


        public override void Show()
        {
            base.Show();

            m_LauncherUI.Progress = "Getting recipes...";

            // Show UI
            m_LauncherUI.Show();

            StartCoroutine(DelayedShow());

        }

        private IEnumerator DelayedShow()
        {
            m_FileData = new FileData();
            m_PercentProgress = 0.0f;
            m_LauncherUI.Progress =  m_PercentProgress.ToString() + " % ";

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
                            m_LauncherUI.Progress = m_PercentProgress.ToString() + " % ";
                            yield return null;
                        }

                        m_PercentProgress = www.progress * 100.0f;
                        m_LauncherUI.Progress = m_PercentProgress.ToString() + " % ";

                        m_FileData.Data[i].Data = www.text;


                        Debug.LogWarning("<color=yellow>" + "[FileRequestManager] Data Retrieved: " + m_FileData.Data[i].Data + "</color>");
                    }
                }
            }
            else
            {
                Debug.LogWarning("<color=yellow>" + "[FileRequestManager] File Data Json is null or empty" + "</color>");
            }

            if (OnGetDataEnd != null)
            {
                OnGetDataEnd();
            }
        }

        public override void Hide()
        {
            base.Hide();

            // Show UI
            m_LauncherUI.Hide();
        }

    }

    
}
