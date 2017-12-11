//using LitJson;
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
        public List<int> Tags;
        public List<string> Ingredients;
        public List<string> Instructions;

        public RecipeModel()
        {
            Tags = new List<int>();
            Ingredients = new List<string>();
            Instructions = new List<string>();
        }
    }

    #endregion DataModel

    [System.Serializable]
    public class SpriteData
    {
        public string ID;
        public Sprite Sprite; 
    }

    

    public class RecipeControl : Base
    {
        [SerializeField]
        private List<RecipeModel> m_RecipeList;


        [SerializeField]
        private Dictionary<ETAG, List<RecipeModel>> m_RecipeData;

        [SerializeField]
        private RecipeUI m_RecipeUI;

        [SerializeField]
        private CategoriesUI m_Category;

        private int m_SelectedRecipeID;

        

        public enum ETAG { BREAKFAST = 0, HIGHCARB, LOWCARB, DESSERT, TREAT, NUM };

        public override void Init()
        {
            base.Init();

            m_RecipeUI.Hide();

            // Init recipe data
            m_RecipeData = new Dictionary<ETAG, List<RecipeModel>>();
            for (int i = 0; i < (int)ETAG.NUM; i++)
            {
                m_RecipeData.Add((ETAG)i,new List<RecipeModel>());
            }


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
                            RecipeModel rData = JsonUtility.FromJson<RecipeModel>(data);

                            // Check tags
                            for (int tag=0; tag < rData.Tags.Count; tag++)
                            {
                                m_RecipeData[(ETAG)tag].Add(rData);
                            }
                            //m_RecipeList.Add(JsonUtility.FromJson<RecipeModel>(data));

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

            // Set categories
            List<string> categories = new List<string>();
            for (int i=0; i<(int)ETAG.NUM; i++)
            {
                categories.Add(((ETAG)i).ToString());
            }
            m_Category.ScrollMenu.InitScroll(categories);
        }

        public override void Show()
        {
            base.Show();

            // Show recipe

            m_RecipeUI.Hide();
            m_Category.ScrollMenu.OnItemPress += OnCategoryPress;
            m_Category.Show();




           /* */
        }

        public override void Back()
        {
           
            m_Category.ScrollMenu.OnItemPress -= OnCategoryPress;
            m_Category.ScrollMenu.OnItemPress -= OnSubcategoryPress;

            base.Back();
        }

        public override void Hide()
        {
            m_Category.ScrollMenu.OnItemPress -= OnCategoryPress;
            m_Category.ScrollMenu.OnItemPress -= OnSubcategoryPress;
            base.Hide();

        }

        private void OnCategoryPress(int buttonID, int x, int y)
        {
            m_Category.ScrollMenu.OnItemPress -= OnCategoryPress;

            // Set recipes with this category
            //for (int i=0; i<)

        }

        private void OnSubcategoryPress(int buttonID, int x, int y)
        {
            m_Category.ScrollMenu.OnItemPress -= OnSubcategoryPress;

            // Set subcategories

            
           /* m_RecipeUI.Title = m_RecipeData[(ETAG)buttonID].Title;

            if (!string.IsNullOrEmpty(m_RecipeData[(ETAG)buttonID].Sprite))
            {
                SetPicture();
            }
            else
            {
                SetInfo();
            }
            m_RecipeUI.Show();*/

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
