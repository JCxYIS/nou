using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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
        string[] getPath = SFB.StandaloneFileBrowser.OpenFilePanel("開啟譜面", Application.dataPath, "zip", false);
        string path = getPath[0];

        Debug.Log("讀取譜面開始="+path);
        var readOsz = File.ReadAllBytes(path);
        //Debug.Log(readOsz);
        var decompressed = Assets.SimpleZip.Zip.Decompress(readOsz);
        Debug.Log(decompressed);
        
    }

    public void GoGame()
    {
        SceneManager.LoadScene("Main");
    }


}
