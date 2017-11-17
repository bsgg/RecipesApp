
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private RecipeControl m_RecipeControl;


        /* [Header("Controls")]
         [SerializeField]
         private VocabularyControl m_VocabularyControl;

         [SerializeField]
         private GrammarControl m_GrammarControl;

         [SerializeField]
         private MainMenuController m_MainMenuController;

         [SerializeField]
         private ABCControl m_HiraganaController;

         [SerializeField]
         private ABCControl m_KatakanaController;

         [SerializeField]
         private DialogControl m_DialogControl;

         [SerializeField]
         private TopBar m_TopBar;
         private Base m_CurrentControl;*/


        void Start ()
        {

            m_RecipeControl.Init();


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
            /*if (m_CurrentControl == m_MainMenuController)
            {
                Application.Quit();
            }
            else
            {
                m_CurrentControl.Back();
            }*/
        }     

       

    }
}
