using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestImages : MonoBehaviour {


    [SerializeField] private Image m_ImageToDisplay;


	void Start ()
    {
        StartCoroutine(RequestFile());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator RequestFile()
    {
        
        string localDirectory = Path.Combine(Application.persistentDataPath, "Files");
        if (!Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }

        string fileName = "test.jpg";
        

        string localPath = Path.Combine(localDirectory, fileName);
        

        if (File.Exists(localPath))
        {
            Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] File exits at :" + localPath + "</color>");

            byte[] fileData = File.ReadAllBytes(localPath);

            Texture2D texture = new Texture2D(2, 2);

            texture.LoadImage(fileData);

            yield return new WaitForEndOfFrame();

            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

            m_ImageToDisplay.sprite = spriteToUse;

            yield return new WaitForEndOfFrame();
        }
        else
        {
            string url = "http://beatrizcv.com/Data/Recipes/Pictures/test.jpg";

            WWW wwwFile = new WWW(url);

            Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Requesting file: " + url + "</color>");

            yield return wwwFile;

            Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Requesting file: " + url + "</color>");

            if (wwwFile.texture != null)
            {
                Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Texture: (" + wwwFile.texture.width + " x " + wwwFile.texture.height + ")" + "</color>");

                Texture2D texture = new Texture2D(wwwFile.texture.width, wwwFile.texture.height, TextureFormat.DXT1, false);
                wwwFile.LoadImageIntoTexture(texture);

                Rect rec = new Rect(0, 0, texture.width, texture.height);
                Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

                m_ImageToDisplay.sprite = spriteToUse;

                // Save bytes
                byte[] bytes = wwwFile.bytes;
                File.WriteAllBytes(localPath, bytes);

                Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] Writing: " + wwwFile.bytes.Length +  "  At: " + localPath  + "</color>");

                yield return new WaitForEndOfFrame();
            }
            else
            {
                Debug.Log("<color=blue>" + "[LauncherControl.RequestFile] No texture:" + "</color>");
            }

            wwwFile.Dispose();
            wwwFile = null;
        }
    }
}
