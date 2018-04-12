using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public string ImageName;
        public string ImageExtension;
        public Sprite Sprite;
    }

    [System.Serializable]
    public class FileData
    {
        public List<IndexFile> Data;
        public FileData()
        {
            Data = new List<IndexFile>();
        }

        public Sprite GetSprite(string name)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].ImageName.Equals(name))
                {
                    return Data[i].Sprite;
                }
            }

            return null;

        }
    }    

    public class LauncherControl : Base
    {

        public delegate void LauncherAction();
        public event LauncherAction OnGetDataEnd;

        [SerializeField] private string m_DataUrl = "http://beatrizcv.com/Data/";

        [SerializeField] private string m_IndexFileName = "FileData.json";
        [SerializeField]
        private string m_PicturesFolder = "Pictures";

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
            m_LauncherUI.Progress =  "Progress " + m_PercentProgress.ToString() + " % ";
            m_LauncherUI.Description = "";
            if (string.IsNullOrEmpty(m_DataUrl) || string.IsNullOrEmpty(m_IndexFileName))
            {
                
                yield return null;
            }

            string urlFile = Path.Combine(m_DataUrl, m_IndexFileName);
            m_LauncherUI.Description = "Getting recipes from " + urlFile;

            Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Requesting data from: " + urlFile + "</color>");

            WWW wwwFile = new WWW(urlFile);
            yield return wwwFile;
            string jsonData = wwwFile.text;
            if (!string.IsNullOrEmpty(jsonData))
            {                
                m_FileData = JsonMapper.ToObject<FileData>(jsonData);

                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Requesting... " + m_FileData.Data.Count + " Files " + "</color>");
                for (int i = 0; i < m_FileData.Data.Count; i++)
                {
                    if (string.IsNullOrEmpty(m_FileData.Data[i].URL))
                    {
                        continue;
                    }

                    m_LauncherUI.Description += "\n- "  +(i + 1) + "/" + m_FileData.Data.Count + " : " + m_FileData.Data[i].FileName;

                    // Request
                    Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Requesting: " + (i + 1) + "/" + m_FileData.Data.Count + " : " + m_FileData.Data[i].FileName + "</color>");
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


                    Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Data Retrieved: " + m_FileData.Data[i].Data + "</color>");
                        
                }

                m_LauncherUI.Description += "Completed";
                // Load images
                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Retrieve pictures: " +  "</color>");
                yield return RequestPictures();

                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] RequestPicturesfinished " + "</color>");
                
            }
            else
            {
                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] File Data Json is null or empty" + "</color>");
            }

            if (OnGetDataEnd != null)
            {
                OnGetDataEnd();
            }
        }

        private IEnumerator RequestPictures()
        {
            string localDirectory = Path.Combine(Application.persistentDataPath, m_PicturesFolder);
            if (!Directory.Exists(localDirectory))
            {
                Directory.CreateDirectory(localDirectory);
            }

            for (int i = 0; i < m_FileData.Data.Count; i++)
            {
                
                if ((string.IsNullOrEmpty(m_FileData.Data[i].ImageName)) || (string.IsNullOrEmpty(m_FileData.Data[i].ImageExtension)))
                {
                    continue;
                }

                string fileName = m_FileData.Data[i].ImageName + "." + m_FileData.Data[i].ImageExtension;
                string localPath = Path.Combine(localDirectory, fileName);


                if (File.Exists(localPath))
                {
                    Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] File exits at :" + localPath + "</color>");

                    byte[] fileData = File.ReadAllBytes(localPath);

                    Texture2D texture = new Texture2D(2, 2);

                    texture.LoadImage(fileData);

                    yield return new WaitForEndOfFrame();

                    Rect rec = new Rect(0, 0, texture.width, texture.height);

                    m_FileData.Data[i].Sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

                    yield return new WaitForEndOfFrame();
                }
                else
                { 
                    string directory = Path.Combine(m_DataUrl, m_PicturesFolder);
                    string url = Path.Combine(directory, fileName);

                    WWW wwwFile = new WWW(url);

                    Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Requesting file: " + url + "</color>");

                    yield return wwwFile;

                    Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Requesting file: " + url + "</color>");

                    if (wwwFile.texture != null)
                    {
                        Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Texture: (" + wwwFile.texture.width + " x " + wwwFile.texture.height + ")" + "</color>");

                        Texture2D texture = new Texture2D(wwwFile.texture.width, wwwFile.texture.height, TextureFormat.DXT1, false);
                        wwwFile.LoadImageIntoTexture(texture);

                        Rect rec = new Rect(0, 0, texture.width, texture.height);

                        m_FileData.Data[i].Sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

                        yield return new WaitForEndOfFrame();


                        // Save bytes
                        byte[] bytes = wwwFile.bytes;
                        File.WriteAllBytes(localPath, bytes);

                        Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Writing: " + wwwFile.bytes.Length + "  At: " + localPath + "</color>");

                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] No texture:" + "</color>");
                    }

                    wwwFile.Dispose();
                    wwwFile = null;
                }
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
