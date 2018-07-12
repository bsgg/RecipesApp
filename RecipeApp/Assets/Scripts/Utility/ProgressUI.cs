using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Utility.UI
{
    public class ProgressUI : UIBase
    {
        [SerializeField] private Text m_ProgressText;
        [SerializeField] private Animator m_Loading;

        public override void Show()
        {
            m_ProgressText.text = "";
            m_Loading.gameObject.SetActive(true);
            base.Show();
        }

        public override void Hide()
        {
            m_Loading.gameObject.SetActive(false);
            base.Hide();
        }

        public void SetProgress(string text,int value)
        {
            m_ProgressText.text = text;
        }
	}
}
