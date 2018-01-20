using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RecipeApp
{
    public class MediaController : MonoBehaviour
    {
        #region Instance
        private static MediaController m_Instance;
        public static MediaController Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (MediaController) FindObjectOfType(typeof(MediaController));

                    if (m_Instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(MediaController) + " is needed in the scene, but there is none.");
                    }
                }
                return m_Instance;
            }
        }
        #endregion Instance       

        [Header("Sprites")]
        [SerializeField]
        private List<Sprite> m_Sprites;

        public Sprite GetSprite(string name)
        {
            for (int i = 0; i < m_Sprites.Count; i++)
            {
                Debug.Log("Sprite: " + m_Sprites[i].name);
                if (m_Sprites[i].name.Equals(name))
                {
                    return m_Sprites[i];
                }
            }

            return null;

        }
    }
}
