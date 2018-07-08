//using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace RecipeApp
{

    public class RecipeControl : Base
    {

        private RecipeModel m_CurrentRecipe;
        public RecipeModel CurrentRecipe
        {
            get { return m_CurrentRecipe; }
            set { m_CurrentRecipe = value; }
        }

        [SerializeField]
        private RecipeUI m_RecipeUI;


        /*


        [SerializeField]
        private Dictionary<ETAG, List<RecipeModel>> m_RecipeData;

        public enum ETAG { BREAKFAST = 0, HIGHCARB = 1, LOWCARB = 2, DESSERT = 3, TREAT = 4, NUM };
        private string[] m_TagTitles = { "Breakfast", "High Carb", "Low Carb", "Dessert", "Treat"};

       
        [SerializeField]
        private CategoriesUI m_Category;

        private enum ESELECTEDLEVEL { NONE = 0, FOODTYPE, RECIPELIST, RECIPE };
        private int m_SelectedLevel = 0;
        private int m_SelectedRecipeID;
        private ETAG m_SelectedCategory;*/




        /*public override void Init()
        {
            base.Init();

            m_RecipeUI.Hide();

            // Init recipe data
            m_RecipeData = new Dictionary<ETAG, List<RecipeModel>>();
            for (int i = 0; i < (int)ETAG.NUM; i++)
            {
                m_RecipeData.Add((ETAG)i,new List<RecipeModel>());
            }

           // m_RecipeList = new List<RecipeModel>();
            if (AppController.Instance.Launcher.FileData.Data != null)
            {
                for (int i = 0; i < AppController.Instance.Launcher.FileData.Data.Count; i++)
                {
                    //string data = AppController.Instance.Launcher.FileData.Data[i].Data;
                    string data = AppController.Instance.Launcher.FileData.Data[i].FileExtension;

                    try
                    {
                        if (!string.IsNullOrEmpty(data))
                        {
                            RecipeModel rData = JsonUtility.FromJson<RecipeModel>(data);

                            // Check tags
                            for (int t=0; t < rData.Tags.Count; t++)
                            {
                                ETAG tag = (ETAG)rData.Tags[t];

                                m_RecipeData[tag].Add(rData);
                            }

                        }
                        else
                        {
                            Debug.Log("[RecipeControl.Init] JSON not found: " + AppController.Instance.Launcher.FileData.Data[i].FileName + " Data: " + data);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[RecipeControl.Init] Bad Format JSON File: " + AppController.Instance.Launcher.FileData.Data[i].FileName + " Data: " + data);
                    }
                }
            }            
        }*/



        public override void Show()
        {
            base.Show();

            m_RecipeUI.Title = m_CurrentRecipe.Title;
            if (m_CurrentRecipe.Sprite != null)
            {
                SetPicture();
            }
            else
            {

                SetInfo();
            }
            // Show recipe
            m_RecipeUI.Show();

            // Start on Food level type
            //m_SelectedLevel = (int)ESELECTEDLEVEL.FOODTYPE;
           // SetCategoryByLevel();
            //m_Category.ScrollMenu.OnItemPress += OnCategoryPress;            

        }

        /// <summary>
        /// Sets category scroll menu according to the seletcted level
        /// </summary>
        /*public void SetCategoryByLevel()
        {
            switch (m_SelectedLevel)
            {
                case (int)ESELECTEDLEVEL.NONE: // Hide all

                    //Hide();
                    break;

                case (int)ESELECTEDLEVEL.FOODTYPE: // Set list of foods

                    m_SelectedCategory = 0;
                    m_SelectedRecipeID = 0;
                    // Set categories
                    List<string> foodTypes = new List<string>();
                    for (int i = 0; i < (int)ETAG.NUM; i++)
                    {
                        foodTypes.Add(m_TagTitles[i]);
                    }
                    //StartCoroutine(m_Category.ScrollMenu.InitScroll(foodTypes));
                    m_RecipeUI.Hide();
                    m_Category.Show();
                    break;

                case (int)ESELECTEDLEVEL.RECIPELIST:

                    // Check at least 1 recipe in this category
                    // Set recipes with this category    
                    List<string> recipeList = new List<string>();
                    for (int i = 0; i < m_RecipeData[m_SelectedCategory].Count; i++)
                    {
                        recipeList.Add(m_RecipeData[m_SelectedCategory][i].Title);
                    }
                    if (recipeList.Count > 0)
                    {

                        //StartCoroutine(m_Category.ScrollMenu.InitScroll(recipeList));
                        m_RecipeUI.Hide();
                    }else
                    {
                        // No recipes, it will stay in Food Type
                        m_SelectedLevel = (int)ESELECTEDLEVEL.FOODTYPE;
                    }                        
                    m_Category.Show();

                break;
                case (int)ESELECTEDLEVEL.RECIPE:
                    m_RecipeUI.Title = m_RecipeData[m_SelectedCategory][m_SelectedRecipeID].Title;
                    if (!string.IsNullOrEmpty(m_RecipeData[m_SelectedCategory][m_SelectedRecipeID].Sprite))
                    {
                        SetPicture();
                    }
                    else
                    {

                        SetInfo();
                    }

                    UpdateMenu();
                    m_Category.Hide();
                    m_RecipeUI.Show();
               break;
            }
        }*/

       /* private void UpdateMenu()
        {
            if (string.IsNullOrEmpty(m_RecipeData[m_SelectedCategory][m_SelectedRecipeID].Link))
            {
                m_RecipeUI.LinkBtn.interactable = false;
            }
            else
            {
                m_RecipeUI.LinkBtn.interactable = true;
            }

            if (string.IsNullOrEmpty(m_RecipeData[m_SelectedCategory][m_SelectedRecipeID].Sprite))
            {
                m_RecipeUI.PictureBtn.interactable = false;
            }
            else
            {
                m_RecipeUI.PictureBtn.interactable = true;
            }

            m_RecipeUI.InfoBtn.interactable = true;
            m_RecipeUI.IngredientsBtn.interactable = true;
            m_RecipeUI.InstructionsBtn.interactable = true;
        }*/

        /*private void OnCategoryPress(int buttonID)
        {
            if (m_SelectedLevel == 0) m_SelectedLevel = (int)ESELECTEDLEVEL.FOODTYPE;

            switch (m_SelectedLevel)
            {
                case (int)ESELECTEDLEVEL.FOODTYPE:
                    m_SelectedRecipeID = 0;
                    m_SelectedCategory = (ETAG)buttonID;
                break;

                case (int)ESELECTEDLEVEL.RECIPELIST:
                    m_SelectedRecipeID = buttonID;
                break;
            }

            // Add 1 level
            m_SelectedLevel += 1;
            SetCategoryByLevel();
        }*/

        public override void Back()
        {
            // Subctract 1 level
            //m_SelectedLevel -= 1;

            //if (m_SelectedLevel < 0) m_SelectedLevel = 0;

            //SetCategoryByLevel();
            m_RecipeUI.Hide();
            base.Back();
        }

        public override void Hide()
        {
            //m_Category.ScrollMenu.OnItemPress -= OnCategoryPress;
           // m_Category.Hide();
            m_RecipeUI.Hide();
           // m_SelectedCategory = 0;
           // m_SelectedRecipeID = 0;

            base.Hide();
        }

        #region Menu
        public void SetPicture()
        {
            m_RecipeUI.SpriteContainer.SetActive(false);
            m_RecipeUI.LongTextContainer.SetActive(true);

            m_RecipeUI.SpriteObject = m_CurrentRecipe.Image;
            m_RecipeUI.SpriteContainer.SetActive(true);
            m_RecipeUI.LongTextContainer.SetActive(false);            
        }

        public void SetInfo()
        {
            string info = string.Empty;

            info = "\n" + "- Preparation Time " + m_CurrentRecipe.PreparationTime + " min\n";
            info += "\n" + "- Cook Time " + m_CurrentRecipe.CookTime + " min\n";
            info += "\n" + "- Total Time " + (m_CurrentRecipe.PreparationTime + m_CurrentRecipe.CookTime) + " min\n";
            info += "\n" + "- Servings " + m_CurrentRecipe.Serves + "\n";
            info += "\n" + "- Calories " + m_CurrentRecipe.Calories + " Kcal\n";
            info += "\n" + "- Difficulty " + m_CurrentRecipe.Difficulty;
            //info += "\n" + "- Tags " + m_RecipeData[m_SelectedCategory][m_SelectedRecipeID].Tags + "\n";

            m_RecipeUI.LongText = info;
            m_RecipeUI.SpriteContainer.SetActive(false);
            m_RecipeUI.LongTextContainer.SetActive(true);
        }


        public void SetIngredients()
        {
            string info = string.Empty;
            for (int i=0; i< m_CurrentRecipe.Ingredients.Count; i++)
            {
                info += "\n - " + m_CurrentRecipe.Ingredients[i] + "\n";
                
            }

            m_RecipeUI.LongText = info;
            m_RecipeUI.SpriteContainer.SetActive(false);
            m_RecipeUI.LongTextContainer.SetActive(true);
        }

        public void SetInstructions()
        {
            string info = string.Empty;
            for (int i = 0; i < m_CurrentRecipe.Instructions.Count; i++)
            {
                info += "\n - " + m_CurrentRecipe.Instructions[i] + "\n";

            }

            m_RecipeUI.LongText = info;
            m_RecipeUI.SpriteContainer.SetActive(false);
            m_RecipeUI.LongTextContainer.SetActive(true);
        }

        public void OnURLPress()
        {
            string url = m_CurrentRecipe.Link;
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
        }

        #endregion Menu


    }
}
