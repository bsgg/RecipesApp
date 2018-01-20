
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace RecipeApp
{
    public class AppController : MonoBehaviour
    {
        #region Instance
        private static AppController m_Instance;
        public static AppController Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (AppController)FindObjectOfType(typeof(AppController));

                    if (m_Instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(AppController) + " is needed in the scene, but there is none.");
                    }
                }
                return m_Instance;
            }
        }
        #endregion Instance

        // public enum EMenu { NONE = -1, KANJI = 0, KANA = 1, ROMAJI = 2, DESCRIPTION = 3, EXAMPLES = 4, NEXT = 5, SOUND = 6 };

        [Header("Controls")]

        [SerializeField]
        private LauncherControl m_Launcher;
        public LauncherControl Launcher
        {
            get { return m_Launcher; }
        }

        [SerializeField]
        private RecipeControl m_RecipeControl;

        private Base m_CurrentControl;


        void Start ()
        {

            m_Launcher.OnGetDataEnd += OnLauncherGetDataEnd;
            m_CurrentControl = m_Launcher;

            m_CurrentControl.Show();

        }

        private void OnLauncherGetDataEnd()
        {
            m_Launcher.OnGetDataEnd -= OnLauncherGetDataEnd;

            m_RecipeControl.Init();

            m_CurrentControl.Hide();

            m_CurrentControl = m_RecipeControl;

            m_CurrentControl.Show();
        }
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
        }  
        
        public void Back()
        {
            m_CurrentControl.Back();            
        }

    }
}
