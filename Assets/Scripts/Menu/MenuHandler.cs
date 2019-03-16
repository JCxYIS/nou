using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField]Text verText;
    [SerializeField]Transform panels;


    // Start is called before the first frame update
    void Start()
    {

        DiscordHandler.instance.SetPresence("Idling", "主選單");
        verText.text = "V. 1.0.0";

        
        for(int i = 0; i < panels.childCount; i++)
        {
            GameObject go = panels.GetChild(i).gameObject;
            if(go.name == "Main Panel")
                go.SetActive(true);
            else
                go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadOsz()
    {
        string[] getPath = SFB.StandaloneFileBrowser.OpenFilePanel("開啟譜面", Application.dataPath, "osz", false);
        string path = getPath[0];

        Debug.Log("讀取譜面開始="+path);
        var readOsz = File.ReadAllBytes(path);
        string tempPath = Application.temporaryCachePath + "\\SongCache-" + System.DateTime.Now.ToString("yyyyMMdd-HHmmss");
        Directory.CreateDirectory(tempPath);
        ZipUtil.Unzip(path, tempPath);
        Debug.Log("osz解壓完成="+tempPath); 

        foreach(var f in Directory.GetFiles(tempPath))
        {
            if(Path.GetExtension(f) == ".osu")
            {
                OsuFile o = new OsuFile(f);
            }
        }
    }

    public void GoGame()
    {
        SceneManager.LoadScene("Main");
    }


}
