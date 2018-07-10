
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


        [Header("UI")]
        [SerializeField]
        private Utility.UI.PopupWithButtons m_PopupWithButtons;
        public Utility.UI.PopupWithButtons PopupWithButtons
        {
            get { return m_PopupWithButtons; }
        }

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
            m_PopupWithButtons.Hide();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("No Internet");
                m_PopupWithButtons.ShowPopup("", "Please connect to internet and restart to download recipes.",
                    "Restart App", OkPopup,
                    string.Empty, null,
                    string.Empty, null);

            }
            else
            {
                // Download recipes data
                
                m_Launcher.OnRequestRecipeEnd += OnLauncherRequestRecipeEnd;
                m_CurrentControl = m_Launcher;

                m_CurrentControl.Show();
                StartCoroutine(m_Launcher.DownloadData());
            }



            

            //m_Launcher.OnGetDataEnd += OnLauncherGetDataEnd;
           
        }

        private void OkPopup(ButtonWithText Button)
        {
            Application.Quit();
        }

        private void OnLauncherRequestRecipeEnd(RecipeModel recipe)
        {
            m_RecipeControl.CurrentRecipe = recipe;
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
            if (m_CurrentControl == m_RecipeControl)
            {
                m_CurrentControl.Hide();
                m_CurrentControl = m_Launcher;
                m_CurrentControl.Show();
            }         
        }

    }
}
