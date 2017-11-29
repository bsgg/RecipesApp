using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace RecipeApp
{
    public class RecipeUI : UIBase
    {
        [SerializeField]
        private Text m_Title;
        public string Title
        {
            set { m_Title.text = value; }
            get { return m_Title.text; }
        }

        [SerializeField]
        private Image m_Sprite;
        public Sprite SpriteObject
        {
            set
            {
                m_Sprite.sprite = value;
                m_Sprite.preserveAspect = true;
            }
            get { return m_Sprite.sprite; }
        }


        [SerializeField]
        private Text m_Description;
        public string Description
        {
            set { m_Description.text = value; }
            get { return m_Description.text; }
        }
    }
}
