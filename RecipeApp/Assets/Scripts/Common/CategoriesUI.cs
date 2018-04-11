using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace RecipeApp
{
    public class CategoriesUI : UIBase
    {
        [Header("CategoriesUI")]

        [SerializeField]
        private ScrollUI m_ScrollMenu;
        public ScrollUI ScrollMenu
        {
            get { return m_ScrollMenu; }
            set { m_ScrollMenu = value; }
        }

        private List<string> m_Categories;
        public List<string> Categories
        {
            get { return m_Categories; }
            set { m_Categories = value; }
        }

    }
}
