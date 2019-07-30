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
    bool PanelPrinted = false;


    // Start is called before the first frame update
    void Start()
    {
        DiscordHandler.instance.SetPresence("Idling", "主選單");
        verText.text = GameManager.version;

        GameObject g = Instantiate(new GameObject());
        g.name = "GameValue";
        g.AddComponent<ToGameValue>();
        DontDestroyOnLoad(g);
        
        for(int i = 0; i < panels.childCount; i++)
        {
            GameObject go = panels.GetChild(i).gameObject;
            go.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!PanelPrinted)
        {
            for(int i = 0; i < panels.childCount; i++)
            {
                GameObject go = panels.GetChild(i).gameObject;
                if(go.name == "Main Panel")
                    go.SetActive(true);
                else
                    go.SetActive(false);
            }
            PanelPrinted = true;
        }
       
        
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

        string newFolderName = "ERROR";
        foreach(var f in Directory.GetFiles(tempPath))
        {
            if(Path.GetExtension(f) == ".osu")
            {
                OsuFile o = new OsuFile(f);
                newFolderName = o.Title + "-" + o.Artist;
                newFolderName = ToLegalPath(newFolderName);
                Debug.Log("新曲包名稱: "+newFolderName);
                break;
            }
        }
        if(Directory.Exists(Application.persistentDataPath+"\\Songs\\"+newFolderName))
        {
            Debug.LogWarning("已經存在同一首歌");
        }
        else
        {
            Debug.Log(tempPath+" | "+Application.persistentDataPath+"\\Songs\\"+newFolderName);
            Directory.Move(tempPath, Application.persistentDataPath+"\\Songs\\"+newFolderName);
        }

        SongPrinter.instance.Reprint();
    }
    static public string ToLegalPath(string inpath)
    {
        foreach(char ill in Path.GetInvalidPathChars())
        {
            inpath = inpath.Replace(ill, '_');
        }
        foreach(char ill in Path.GetInvalidFileNameChars())
        {
            inpath = inpath.Replace(ill, '_');
        }
        inpath = inpath.Replace(" ", "_");
        return inpath;
    }
      
    public void GoGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ClearCache()
    {
        if(Directory.Exists(Application.persistentDataPath + "/.thumbnail"))
        {
            foreach (string f in Directory.GetFiles(Application.persistentDataPath + "/.thumbnail"))
            {
                File.Delete(f);
            }
        }
        Caching.ClearCache();
        Debug.Log("Cleared cache");
        SceneManager.LoadScene("Menu");
    }


    public static Texture2D LoadPic(string picpath)
    {
        try
        {

            Texture2D t2d = new Texture2D(1, 1);
            FileStream fileStream = new FileStream(picpath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
            if (!t2d.LoadImage(bytes))
                Debug.LogError("Load Pic Error!");
            return t2d;
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Failed to load an image! \n{e}");
            return Resources.Load<Texture2D>("NoPic");
        }
    }

}
