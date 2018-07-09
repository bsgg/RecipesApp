using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace RecipeApp
{
    public class LauncherUI : UIBase
    { 
        [SerializeField] private Text m_Progress;

        public string Progress
        {
            set { m_Progress.text = value; }
            get { return m_Progress.text; }
        }

        [SerializeField] private ButtonWithText m_DownloadButton;
        public ButtonWithText DownloadButton
        {
            set { m_DownloadButton = value; }
            get { return m_DownloadButton; }
        }

        [SerializeField] private Text m_Description;
        public string Description
        {
            set { m_Description.text = value; }
            get { return m_Description.text; }
        }



        [SerializeField]
        private ScrollUI m_ScrollList;
        public ScrollUI ScrollList
        {
            get { return m_ScrollList; }
        }
        
    }
}
