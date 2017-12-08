using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace RecipeApp
{
    #region DataModel

    [System.Serializable]
    public class RecipeModel
    {
        public string Title;
        public string Sprite;
        public int PreparationTime;
        public int CookTime;
        public int Serves;
        public int Calories;
        public int Difficulty;
        public List<string> Tags;
        public List<string> Ingredients;
        public List<string> Instructions;

        public RecipeModel()
        {
            Tags = new List<string>();
            Ingredients = new List<string>();
            Instructions = new List<string>();
        }
    }

    #endregion DataModel

    [System.Serializable]
    public class RecipeData
    {
        //public enum ETAG { BREAKFAST, HIGHCARB, LOWCARB, DESSERT, TREAT };        
        public string FileName;

        //public List<ETAG> Tags;

        public RecipeModel Recipe;

        public Sprite Sprite;
    }

    

    public class RecipeControl : Base
    {
        [SerializeField]
        private string m_DataPath = "Recipes";

        [SerializeField]
        private List<RecipeData> m_RecipeSet;

        [SerializeField]
        private RecipeUI m_RecipeUI;

        private int m_SelectedRecipeID;

        private List<RecipeModel> m_RecipeList;

        public override void Init()
        {
            base.Init();

            m_RecipeUI.Hide();

            /* for (int i = 0; i < m_RecipeSet.Count; i++)
             {

                 string path = m_DataPath + "\\" + m_RecipeSet[i].FileName;
                string json = Utility.Utility.LoadJSONResource(path);
                 try
                 {
                     if (!string.IsNullOrEmpty(json))
                     {
                         m_RecipeSet[i].Recipe = JsonMapper.ToObject<RecipeModel>(json);
                     }
                     else
                     {
                         Debug.Log("[RecipeControl.Init] JSON not found: " + path);
                     }
                 }
                 catch (Exception e)
                 {
                     Debug.LogError("[RecipeControl.Init] Bad Format JSON File: " + path);
                 }

             }*/


            m_RecipeList = new List<RecipeModel>();
            if (FileRequestManager.Instance.FileData.Data != null)
            {
                for (int i = 0; i < FileRequestManager.Instance.FileData.Data.Count; i++)
                {
                    string data =FileRequestManager.Instance.FileData.Data[i].Data;

                    try
                    {
                        if (!string.IsNullOrEmpty(data))
                        {
                            m_RecipeList.Add(JsonMapper.ToObject<RecipeModel>(data));
                        }
                        else
                        {
                            Debug.Log("[RecipeControl.Init] JSON not found: " + FileRequestManager.Instance.FileData.Data[i].FileName + " Data: " + data);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[RecipeControl.Init] Bad Format JSON File: " + FileRequestManager.Instance.FileData.Data[i].FileName + " Data: " + data);
                    }
                }
            }

            m_SelectedRecipeID = 0;

        }

        public override void Show()
        {
            base.Show();

            // Show recipe

            m_RecipeUI.Title = m_RecipeList[m_SelectedRecipeID].Title;

            if (!string.IsNullOrEmpty(m_RecipeList[m_SelectedRecipeID].Sprite))
            {
                SetPicture();
            }
            else
            {
                SetInfo();
            }
            m_RecipeUI.Show();
        }

        #region Menu
        public void SetPicture()
        {

        }

        public void SetInfo()
        {
            string info = string.Empty;

            info = "\n" + "- Preparation Time " + m_RecipeList[m_SelectedRecipeID].PreparationTime + " min\n";
            info += "\n" + "- Cook Time " + m_RecipeList[m_SelectedRecipeID].CookTime + " min\n";
            info += "\n" + "- Total Time " + (m_RecipeList[m_SelectedRecipeID].PreparationTime + m_RecipeList[m_SelectedRecipeID].CookTime) + " min\n";
            info += "\n" + "- Servings " + m_RecipeList[m_SelectedRecipeID].Serves + "\n";
            info += "\n" + "- Calories " + m_RecipeList[m_SelectedRecipeID].Calories + " Kcal\n";
            info += "\n" + "- Difficulty " + m_RecipeList[m_SelectedRecipeID].Difficulty + "\n";
            info += "\n" + "- Tags " + m_RecipeList[m_SelectedRecipeID].Tags + "\n";

            m_RecipeUI.LongText = info;
        }


        public void SetIngredients()
        {
            string info = string.Empty;
            for (int i=0; i< m_RecipeList[m_SelectedRecipeID].Ingredients.Count; i++)
            {
                info += "\n - " + m_RecipeList[m_SelectedRecipeID].Ingredients[i] + "\n";
                
            }

            m_RecipeUI.LongText = info;
        }

        public void SetInstructions()
        {
            string info = string.Empty;
            for (int i = 0; i < m_RecipeList[m_SelectedRecipeID].Instructions.Count; i++)
            {
                info += "\n - " + m_RecipeList[m_SelectedRecipeID].Instructions[i] + "\n";

            }

            m_RecipeUI.LongText = info;
        }

        #endregion Menu


    }
}
