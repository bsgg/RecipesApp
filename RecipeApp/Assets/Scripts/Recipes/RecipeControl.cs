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
        public List<string> Ingredients;
        public List<string> Method;

        public RecipeModel()
        {
            Ingredients = new List<string>();
            Method = new List<string>();
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

        }

        public override void Show()
        {
            base.Show();

            // Show recipe


            m_RecipeUI.Show();
        }
    }
}
