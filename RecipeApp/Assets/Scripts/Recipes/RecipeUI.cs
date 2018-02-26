using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace RecipeApp
{
    public class RecipeUI : UIBase
    {
        [Header("Menu")]
        [SerializeField]
        private Button m_PictureBtn;
        public Button PictureBtn
        {
            get { return m_PictureBtn; }
        }

        [SerializeField]
        private Button m_InfoBtn;
        public Button InfoBtn
        {
            get { return m_InfoBtn; }
        }

        [SerializeField]
        private Button m_IngredientsBtn;
        public Button IngredientsBtn
        {
            get { return m_IngredientsBtn; }
        }

        [SerializeField]
        private Button m_InstructionsBtn;
        public Button InstructionsBtn
        {
            get { return m_InstructionsBtn; }
        }


        [SerializeField]
        private Button m_LinkBtn;
        public Button LinkBtn
        {
            get { return m_LinkBtn; }
        }


        [Header("Body")]
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
        private GameObject m_SpriteContainer;
        public GameObject SpriteContainer
        {
            get { return m_SpriteContainer; }
        }


        [SerializeField]
        private Text m_LongText;
        public string LongText
        {
            set { m_LongText.text = value; }
            get { return m_LongText.text; }
        }

        [SerializeField]
        private GameObject m_LongTextContainer;
        public GameObject LongTextContainer
        {
            get { return m_LongTextContainer; }
        }
    }
}
