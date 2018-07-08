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
    public class RecipeModel
    {
        public string Title;
        
        public string Link;
        public int PreparationTime;
        public int CookTime;
        public int Serves;
        public int Calories;
        public int Difficulty;
        public List<int> Tags;
        public List<string> Ingredients;
        public List<string> Instructions;

        public string Sprite;
        public Sprite Image;


        public RecipeModel()
        {
            Tags = new List<int>();
            Ingredients = new List<string>();
            Instructions = new List<string>();

            Image = null;
        }
    }

    [System.Serializable]
    public class IndexFile
    {
        public string Title;
        public string FileName;
        public string FileExtension;
        public string ImageExtension;
        
        public RecipeModel Recipe;
        public bool Loaded;       

        public IndexFile()
        {
            Loaded = false;
        }
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
        public delegate void LauncherAction(RecipeModel Recipe);
        public event LauncherAction OnRequestRecipeEnd;

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

            // Check if file exits in local
            if (File.Exists(m_LocalIndexFileURL))
            {
                Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] File Index exits at : " + m_LocalIndexFileURL + "</color>");

                // Retrive file
                bool error = false;
                try
                {
                    StreamReader reader = new StreamReader(m_LocalIndexFileURL);

                    string text = reader.ReadToEnd();

                    reader.Close();

                    m_FileData = JsonUtility.FromJson<FileData>(text);

                }
                catch (Exception e)
                {
                    Debug.Log("<color=red>" + "[LauncherControl.DelayedShow] There was an error trying to get file (index recipes) " + m_LocalIndexFileURL + " ERROR: " + e.Message + "</color>");
                }

                yield return new WaitForEndOfFrame();


                for (int i=0;i< m_FileData.Data.Count; i++)
                {
                    String localRecipeURL = Path.Combine(localRecipeDirectory,m_FileData.Data[i].FileName + "." + m_FileData.Data[i].FileExtension);
                    String localPictureURL = Path.Combine(localPictureDirectory, m_FileData.Data[i].FileName + "." + m_FileData.Data[i].ImageExtension);

                    Debug.Log("<color=blue>" + "[LauncherControl.DelayedShow] Retrieving Recipe (" + i + "/" + m_FileData.Data.Count + ") - " + m_FileData.Data[i].FileName + " </color>");

                    // Recipe
                    error = false;
                    if (File.Exists(localRecipeURL))
                    {
                        // Retrive file
                        try
                        {
                            StreamReader readerRecipe = new StreamReader(localRecipeURL);

                            string recipeJSON = readerRecipe.ReadToEnd();

                            m_FileData.Data[i].Recipe = JsonUtility.FromJson<RecipeModel>(recipeJSON);                           

                            readerRecipe.Close();

                        }
                        catch (Exception e)
                        {
                            error = true;
                            Debug.Log("<color=red>" + "[LauncherControl.DelayedShow] There was an error trying to get file (recipe) " + localRecipeURL + " ERROR: " + e.Message + "</color>");
                        }
                    }else
                    {
                        Debug.Log("<color=red>" + "[LauncherControl.DelayedShow] File (recipe) " + localRecipeURL + " doesn't exits " + "</color>");
                    }

                    if (error) continue;

                    // Image
                    //m_FileData.Data[i].Sprite = null;

                    if (File.Exists(localPictureURL))
                    {
                        Texture2D texture = new Texture2D(2, 2);
                      
                        // Retrive file
                        try
                        {
                            byte[] pictureData = File.ReadAllBytes(localPictureURL);

                            texture.LoadImage(pictureData);

                        }
                        catch (Exception e)
                        {
                            Debug.Log("<color=red>" + "[LauncherControl.DelayedShow] There was an error trying to get file (image) " + localPictureURL + " ERROR: " + e.Message + "</color>");
                            error = true;
                        }

                        if (!error)
                        {
                            yield return new WaitForEndOfFrame();

                            Rect rec = new Rect(0, 0, texture.width, texture.width);

                            m_FileData.Data[i].Recipe.Image = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

                            yield return new WaitForEndOfFrame();

                            m_FileData.Data[i].Loaded = true;
                        }

                    }
                    else
                    {
                        Debug.Log("<color=red>" + "[LauncherControl.DelayedShow] File (image) " + localPictureURL + " doesn't exits " + "</color>");
                    }
                }

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

                m_FileData.Data[id].Recipe.Image = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

                yield return new WaitForEndOfFrame();

                string localPicture = Path.Combine(localPictureDirectory, pictureName);
                SaveFileToLocal(localPicture, wwwPictureFile);

                m_FileData.Data[id].Loaded = true;
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

            if (OnRequestRecipeEnd != null)
            {
                OnRequestRecipeEnd(m_FileData.Data[id].Recipe);
            }

        }
        
        public override void Hide()
        {
            base.Hide();

            // Show UI
            m_LauncherUI.Hide();
        }

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
            
            if ((button.IdButton >= 0) && (button.IdButton < m_FileData.Data.Count) && m_FileData.Data[button.IdButton].Loaded)
            {
                if (OnRequestRecipeEnd != null)
                {
                    OnRequestRecipeEnd(m_FileData.Data[button.IdButton].Recipe);
                }
            }else
            {
                AppController.Instance.PopupWithButtons.ShowPopup("Error", "Please download the recipe first","OK", OnOkPopup, string.Empty, null, string.Empty, null);
            }            
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
