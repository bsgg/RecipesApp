using LitJson;
using System;
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
        public string Title;
        public string FileName;
        public string FileExtension;
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
                if (Data[i].FileName.Equals(name))
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
        private string m_LocalIndexFileURL = string.Empty;

        [SerializeField]
        private string m_PicturesFolder = "Pictures";
        [SerializeField]
        private string m_RecipesFolder = "Recipes";

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

            if (string.IsNullOrEmpty(m_DataUrl) || string.IsNullOrEmpty(m_IndexFileName))
            {                
                yield return null;
            }

            m_LocalIndexFileURL = Path.Combine(Application.persistentDataPath, m_IndexFileName);

            // Check if file exits in local
            if (File.Exists(m_LocalIndexFileURL))
            {
                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] File Index exits at : " + m_LocalIndexFileURL + "</color>");

                // Retrive file
                StreamReader reader = new StreamReader(m_LocalIndexFileURL);

                string text = reader.ReadToEnd();

                reader.Close();
                

                yield return new WaitForEndOfFrame();

                m_FileData = JsonUtility.FromJson<FileData>(text);

                yield return RefreshScrollList();  

            }
            else
            {
                m_LauncherUI.Progress = "No Recipes found in local, please download the list of recipes";               
            }
        }

        public void OnDownloadIndexFile()
        {
            // TODO: CHECK INTERNET
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                StopAllCoroutines();
                StartCoroutine(DownloadIndexFile());
            }
            else
            {

                m_LauncherUI.Progress = "Please connect to internet to continue";
            }           
        }

        private IEnumerator DownloadIndexFile()
        {
            string urlFile = Path.Combine(m_DataUrl, m_IndexFileName);

            Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Requesting data from: " + urlFile + "</color>");

            WWW wwwFile = new WWW(urlFile);
            yield return wwwFile;
            string jsonData = wwwFile.text;

            // Save file in local
            SaveFileToLocal(m_LocalIndexFileURL, wwwFile);
            yield return new WaitForEndOfFrame();


            if (!string.IsNullOrEmpty(jsonData))
            {
                m_FileData = JsonMapper.ToObject<FileData>(jsonData);

                yield return RefreshScrollList();

            }
            else
            {
                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] File Data Json is null or empty" + "</color>");
            }
        }


        private void SaveFileToLocal(string url, WWW request)
        {
            try
            {
                if (File.Exists(url))// File exists
                {
                    File.Delete(url);
                }
                // Save file
                byte[] bytes = request.bytes;
                File.WriteAllBytes(url, bytes);
            }
            catch (Exception e)
            {
                Debug.Log("<color=purple>" + "[DataManager] Unable to store " + url + "</color>");
            }
        }

        private IEnumerator RequestRecipe(int id)
        {



            string localDirectory = Path.Combine(Application.persistentDataPath, m_RecipesFolder);
            if (!Directory.Exists(localDirectory))
            {
                Directory.CreateDirectory(localDirectory);
            }

            if (m_FileData.Data == null) yield break;

            if ((id < 0) || (id > m_FileData.Data.Count)) yield break;



            //Debug.Log("<color=blue>" + "[LauncherControl.RequestRecipe] Requesting data from: " + m_FileData.Data[id].URL + "</color>");

            // TODO: FIX THIS
            string fileName = m_FileData.Data[id].FileName + m_FileData.Data[id].FileExtension;
            string url = Path.Combine(m_DataUrl+ "\\Data\\", fileName);

           WWW wwwFile = new WWW(url);
            yield return wwwFile;
            string jsonData = wwwFile.text;

            // Save file in local
            // TODO SAVE FILE WITH EXTENSION
            string localFile = Path.Combine(localDirectory, fileName);
            SaveFileToLocal(localFile, wwwFile);
            yield return new WaitForEndOfFrame();


            if (!string.IsNullOrEmpty(jsonData))
            {

                //m_FileData = JsonMapper.ToObject<FileData>(jsonData);
                RecipeModel rData = JsonUtility.FromJson<RecipeModel>(jsonData);

            }
            else
            {
                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] File Data Json is null or empty" + "</color>");
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
                
                if ((string.IsNullOrEmpty(m_FileData.Data[i].FileName)) || (string.IsNullOrEmpty(m_FileData.Data[i].ImageExtension)))
                {
                    continue;
                }

                string fileName = m_FileData.Data[i].FileName + "." + m_FileData.Data[i].ImageExtension;
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

        #region RequestFiles


        #endregion RequestFiles


        #region ScrollList

        private IEnumerator RefreshScrollList()
        {
            if (m_FileData.Data == null) yield break;

            List<string> lRecipes = new List<string>();
            for (int i = 0; i < m_FileData.Data.Count; i++)
            {
                // Initialize scroll list
                lRecipes.Add(m_FileData.Data[i].FileName);
            }

            yield return (m_LauncherUI.ScrollList.InitScroll(lRecipes, OnScrollItemClicked));

            if (m_LauncherUI.ScrollList.ListElements != null)
            {
                for (int i = 0; i < m_LauncherUI.ScrollList.ListElements.Count; i++)
                {
                    if (m_LauncherUI.ScrollList.ListElements[i].transform.childCount > 1)
                    {
                        Transform downloadObjectChild = m_LauncherUI.ScrollList.ListElements[i].transform.GetChild(1);
                        ButtonWithText downloadBtn = downloadObjectChild.GetComponent<ButtonWithText>();
                        if (downloadBtn != null)
                        {
                            downloadBtn.Set(i, "", OnScrollItemDownloadClicked);
                        }
                    }

                }
            }
        }

        private void OnScrollItemClicked(ButtonWithText button)
        {
            Debug.Log("OnScrollItemClicked: " + button.IdButton);



            AppController.Instance.PopupWithButtons.ShowPopup("Test", "You want to see recipe: " + button.IdButton, "Ok", OnOkPopup, string.Empty, null, string.Empty, null);
        }

        private void OnOkPopup(ButtonWithText button)
        {
            AppController.Instance.PopupWithButtons.Hide();
        }

        private void OnScrollItemDownloadClicked(ButtonWithText button)
        {
            Debug.Log("OnScrollItemDownloadClicked: " + button.IdButton);

            StartCoroutine(RequestRecipe(button.IdButton));

            
            AppController.Instance.PopupWithButtons.ShowPopup("Test", "You want to download recipe: " + button.IdButton, "Ok", OnOkPopup, string.Empty, null, string.Empty, null);
        }

        #endregion ScrollList
    }


}
