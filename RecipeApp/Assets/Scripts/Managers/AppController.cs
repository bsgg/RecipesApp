
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
        [SerializeField] private LauncherControl m_Launcher;

        [SerializeField] private RecipeControl m_RecipeControl;

        void Start ()
        {
            HideAll();
            m_Launcher.Init();

            m_Launcher.OnRequestRecipeCompleted += Launcher_OnRequestRecipeCompleted;
            m_Launcher.Show();
            if (m_Launcher.CheckIfDataExits())
            {
                StartCoroutine(m_Launcher.LoadLocalData());
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    m_PopupWithButtons.ShowPopup("No Internet", "Please connect to internet to download data and restart the app");
                }
                else
                {
                    StartCoroutine(m_Launcher.DownloadData());
                }
            }           

        }

        private void HideAll()
        {
            m_PopupWithButtons.Hide();
            m_RecipeControl.Hide();
            m_Launcher.Hide();
        }
        

        private void Launcher_OnRequestRecipeCompleted(RecipeModel recipe)
        {
            m_RecipeControl.CurrentRecipe = recipe;
            m_Launcher.Hide();
            m_RecipeControl.Show();        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
        }  
        
        public void Back()
        {
            if (m_RecipeControl.Visible)
            {
                m_RecipeControl.Hide();
                m_Launcher.Show();
            }         
        }

    }
}
