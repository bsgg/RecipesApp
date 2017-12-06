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
        public List<string> Ingredients;
        public List<string> Instructions;

        public RecipeModel()
        {
            Ingredients = new List<string>();
            Instructions = new List<string>();
        }
    }

    #endregion DataModel

    [System.Serializable]
    public class RecipeData
    {
        public enum ETAG { BREAKFAST, HIGHCARB, LOWCARB, DESSERT, TREAT };        
        public string FileName;

        public List<ETAG> Tags;

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

        public override void Init()
        {
            base.Init();

            m_RecipeUI.Hide();

            for (int i = 0; i < m_RecipeSet.Count; i++)
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
                
            }

            m_SelectedRecipeID = 0;

        }

        public override void Show()
        {
            base.Show();

            // Show recipe

            m_RecipeUI.Title = m_RecipeSet[m_SelectedRecipeID].Recipe.Title;

            if (!string.IsNullOrEmpty(m_RecipeSet[m_SelectedRecipeID].Recipe.Sprite))
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

            info = "\n" + "- Preparation Time " + m_RecipeSet[m_SelectedRecipeID].Recipe.PreparationTime + " min\n";
            info += "\n" + "- Cook Time " + m_RecipeSet[m_SelectedRecipeID].Recipe.CookTime + " min\n";
            info += "\n" + "- Total Time " + (m_RecipeSet[m_SelectedRecipeID].Recipe.PreparationTime + m_RecipeSet[m_SelectedRecipeID].Recipe.CookTime) + " min\n";
            info += "\n" + "- Servings " + m_RecipeSet[m_SelectedRecipeID].Recipe.Serves + "\n";
            info += "\n" + "- Calories " + m_RecipeSet[m_SelectedRecipeID].Recipe.Calories + " Kcal\n";
            info += "\n" + "- Difficulty " + m_RecipeSet[m_SelectedRecipeID].Recipe.Difficulty + "\n";

            m_RecipeUI.LongText = info;
        }


        public void SetIngredients()
        {
            string info = string.Empty;
            for (int i=0; i< m_RecipeSet[m_SelectedRecipeID].Recipe.Ingredients.Count; i++)
            {
                info += "\n - " + m_RecipeSet[m_SelectedRecipeID].Recipe.Ingredients[i] + "\n";
                
            }

            m_RecipeUI.LongText = info;
        }

        public void SetInstructions()
        {
            string info = string.Empty;
            for (int i = 0; i < m_RecipeSet[m_SelectedRecipeID].Recipe.Instructions.Count; i++)
            {
                info += "\n - " + m_RecipeSet[m_SelectedRecipeID].Recipe.Instructions[i] + "\n";

            }

            m_RecipeUI.LongText = info;
        }

        #endregion Menu


    }
}
