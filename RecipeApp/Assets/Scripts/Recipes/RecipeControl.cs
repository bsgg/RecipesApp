using System.Collections;
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
        public string FileName;
        public RecipeModel Recipe;
        public Sprite Sprite;
    }

    public class RecipeControl : Base
    {
        [SerializeField]
        private string m_DataPath = "Data/Recipes/";

        [SerializeField]
        private List<RecipeData> m_RecipeSet;

        [SerializeField]
        private RecipeUI m_RecipeUI;

        private int m_SelectedRecipeID;

        public override void Init()
        {
            base.Init();

            m_RecipeUI.Hide();

            for (int i= 0; i< m_RecipeSet.Count; i++)
            {
                string path = Application.dataPath + "/Resources/" + m_DataPath + m_RecipeSet[i].FileName + ".json";
                m_RecipeSet[i].Recipe = JsonUtility.FromJson<RecipeModel>(path);

            }

        }
    }
}
