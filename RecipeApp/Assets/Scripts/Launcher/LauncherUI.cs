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
    }
}
