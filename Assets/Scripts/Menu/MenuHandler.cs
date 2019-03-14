using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadOsz()
    {
        SFB.StandaloneFileBrowser.OpenFilePanel("開啟譜面", Application.dataPath, "osz", false);
    }
    public void GoGame()
    {
        SceneManager.LoadScene("Main");
    }

}
