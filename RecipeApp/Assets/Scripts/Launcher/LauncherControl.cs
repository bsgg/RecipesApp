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
        public RecipeModel Recipe;
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

                // TODO:: Load DATA IF EXITS



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
            string localRecipeDirectory = Path.Combine(Application.persistentDataPath, m_RecipesFolder);
            if (!Directory.Exists(localRecipeDirectory))
            {
                Directory.CreateDirectory(localRecipeDirectory);
            }

            string localPictureDirectory = Path.Combine(Application.persistentDataPath, m_PicturesFolder);
            if (!Directory.Exists(localPictureDirectory))
            {
                Directory.CreateDirectory(localPictureDirectory);
            }

            if (m_FileData.Data == null) yield break;

            if ((id < 0) || (id > m_FileData.Data.Count)) yield break;


            AppController.Instance.PopupWithButtons.ShowPopup("Downlad Recipe", "Downloading recipe " + m_FileData.Data[id].Title + "\n Wait..", string.Empty, null, string.Empty, null, string.Empty, null);

            string fileName = m_FileData.Data[id].FileName + "." + m_FileData.Data[id].FileExtension; 
            string recipeFolder = Path.Combine(m_DataUrl, m_RecipesFolder);
            string recipeUrl = Path.Combine(recipeFolder, fileName);

            Debug.Log("<color=blue>" + "[LauncherControl.RequestRecipe] Requesting File: " + fileName + " URL: " + recipeUrl + "</color>");

            WWW wwwFile = new WWW(recipeUrl);
            yield return wwwFile;
            string jsonData = wwwFile.text;

            // Save file in local
            string localFile = Path.Combine(localRecipeDirectory, fileName);
            SaveFileToLocal(localFile, wwwFile);
            yield return new WaitForEndOfFrame();

            if (!string.IsNullOrEmpty(jsonData))
            {
                m_FileData.Data[id].Recipe = JsonUtility.FromJson<RecipeModel>(jsonData);

            }
            else
            {
                AppController.Instance.PopupWithButtons.MessageText = "There was an error trying to download the file" + fileName + "\n Try again later";
                Debug.Log("<color=blue>" + "[LauncherControl.RequestRecipe] File Data Json is null or empty" + "</color>");
            }

            string pictureName = m_FileData.Data[id].FileName + "." + m_FileData.Data[id].ImageExtension;
            string pictureFolder = Path.Combine(m_DataUrl, m_PicturesFolder);
            string pictureUrl = Path.Combine(pictureFolder, pictureName);

            AppController.Instance.PopupWithButtons.MessageText = "Downloading picture " + m_FileData.Data[id].Title + "\n Wait..";

            Debug.Log("<color=blue>" + "[LauncherControl.RequestRecipe] Requesting Picture: " + pictureName + " URL: " + pictureUrl + "</color>");

            WWW wwwPictureFile = new WWW(pictureUrl);
            yield return wwwPictureFile;
            if (wwwPictureFile.texture != null)
            {
                Debug.Log("<color=blue>" + "[LauncherControl.RequestRecipe] Texture: (" + wwwFile.texture.width + " x " + wwwFile.texture.height + ")" + "</color>");

                Texture2D texture = new Texture2D(wwwPictureFile.texture.width, wwwPictureFile.texture.height, TextureFormat.DXT1, false);
                wwwPictureFile.LoadImageIntoTexture(texture);

                Rect rec = new Rect(0, 0, texture.width, texture.height);

                m_FileData.Data[id].Sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

                yield return new WaitForEndOfFrame();

                string localPicture = Path.Combine(localPictureDirectory, pictureName);
                SaveFileToLocal(localPicture, wwwPictureFile);
            }
            else
            {
                AppController.Instance.PopupWithButtons.MessageText = "There was an error trying to download  the picture" + pictureName + "\n Try again later";
                Debug.Log("<color=blue>" + "[LauncherControl.RequestRecipe] No texture:" + "</color>");

                yield return new WaitForSeconds(0.5f);
                AppController.Instance.PopupWithButtons.Hide();
            }

            wwwFile.Dispose();
            wwwFile = null;

            AppController.Instance.PopupWithButtons.MessageText = "Completed! ";
            yield return new WaitForSeconds(0.3f);

            AppController.Instance.PopupWithButtons.Hide();
        }

        private IEnumerator RequestPictures()
        {

            string localDirectory = string.Empty;
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
                lRecipes.Add(m_FileData.Data[i].Title);
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

        

        private void OnScrollItemDownloadClicked(ButtonWithText button)
        {
            Debug.Log("OnScrollItemDownloadClicked: " + button.IdButton);

            

            StartCoroutine(RequestRecipe(button.IdButton));

            
            
        }

        private void OnOkPopup(ButtonWithText button)
        {
            AppController.Instance.PopupWithButtons.Hide();
        }

        #endregion ScrollList
    }


}
