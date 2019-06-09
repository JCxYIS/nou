using System.Collections.Generic;
//#if UNITY_STANDALONE || UNITY_EDITOR
using System.IO;
//#endif
using UnityEngine;
using UnityEngine.UI;

public class SongPrinter : MonoBehaviour 
{
    public static SongPrinter instance;
    public enum Sauce { nou, osu, resources};

    [System.Serializable]
    public class MenuSong
    {
        public Sauce sauce = Sauce.nou;
        public string path = "";//DIR PATH
        public MenuSong(string Path, Sauce Source)
        {
            path = Path;
            sauce = Source;
        }
    }

    [Header("Example songs")]
    [Tooltip("path=在Resources/Songs的曲名, 這裡的sauce必為resources(要不要調隨便)")]
    public List<MenuSong> exampleSongs;

    [Header("Objects")]
    public List<GameObject> buttList;
    [SerializeField] GameObject buttU, buttD, shouldPlaceIn;
    public Button printNouButt, printOsuButt, printResButt;
    public Image printNouImg, printOsuImg, printResImg;//由此判斷

    readonly float initPos = 258.4f;//歌butt x init pos
    readonly float[] initPosY = { -110.7f, -196.7f };////歌butt y init pos


    void Awake()
    {
        instance = this;


        #region Fetch Song
        List<MenuSong> songList = new List<MenuSong>();

#if UNITY_STANDALONE || UNITY_EDITOR
        if (!Directory.Exists(Application.persistentDataPath + "\\Songs\\"))
            Directory.CreateDirectory(Application.persistentDataPath + "\\Songs\\");
        
        //nou! folder
        foreach (var item in Directory.GetDirectories(Application.persistentDataPath + "\\Songs\\"))
        {
            Debug.Log("從nou存檔獲取曲包 路徑: "+item);
            songList.Add( new MenuSong(item, Sauce.nou) );
        }

        //osu! folder (user pre-defined)
        if(Directory.Exists(Userpref.data.customOsuPath))
            foreach (var item in Directory.GetDirectories(Userpref.data.customOsuPath))
            {
                Debug.Log("從osu獲取曲包 路徑: "+item);
                songList.Add(new MenuSong(item, Sauce.osu) );
            }
        else
            Debug.Log("未找到osu資料夾");
#endif
        foreach(var item in exampleSongs)
        {
            Debug.Log("從Asset獲取曲包 相對路徑: Resources/Song/" + item.path);
            item.sauce = Sauce.resources;
            songList.Add(item);
        }
        #endregion

        #region Make Butt
        for(int i = 0; i < songList.Count; i++)
        {
            GameObject go;
            if(i % 2 == 0)
                go = Instantiate(buttU, buttU.transform.parent);
            else
                go = Instantiate(buttD, buttD.transform.parent);
            
            go.name = "Button ("+i+")";
            go.tag = songList[i].sauce.ToString();
            go.SetActive(true);
            buttList.Add(go);
            ReArrangeButts();      
            
            //Load osu files
            List<OsuFile> o = new List<OsuFile>();
            if (songList[i].sauce == Sauce.resources)
            {
                foreach (var f in Resources.LoadAll($"Songs/{songList[i].path}") )
                {
                    //Debug.Log(f.name);
                    for(int a = 0; a<=30; a++)
                    {
                        if(f.name == a.ToString())
                            o.Add( new OsuFile( $"RESOURCES/Songs/{songList[i].path}/{f.name}",f.ToString() ) );
                    }
                }
            }
            else
            {
#if UNITY_EDITOR || UNITY_STANDALONE //not for webgl
                foreach (var f in Directory.GetFiles(songList[i].path) )
                {
                    //Debug.Log(f);
                    if (Path.GetExtension(f) == ".osu")
                    {
                        o.Add(new OsuFile(f));
                    }
                }
#endif
            }

            if (o.Count == 0)
            {
                Debug.LogError("沒有找到.osu, path:" + songList[i].path);
                continue;
            }


            //Load BG
            Texture2D t2d = new Texture2D(1, 1);
            if (songList[i].sauce == Sauce.resources)
            {
                string t2dpath = $"Songs/{songList[i].path}/{Path.GetFileNameWithoutExtension(o[0].BGfileName)}";
                t2d = Resources.Load<Texture2D>(t2dpath);
                Debug.Log("RES t2d path="+t2dpath);
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            else
            {
                string thumbnailPath = Application.persistentDataPath + "\\.thumbnail";
                Directory.CreateDirectory(thumbnailPath);
                thumbnailPath += "\\" + Path.GetFileName(songList[i].path) + ".png";
                if (!File.Exists(thumbnailPath))
                {
                    if (string.IsNullOrEmpty(o[0].BGfileName))
                    {
                        Debug.LogWarning($"BG is empty path (Song={o[0].Title})");
                        t2d = Resources.Load<Texture2D>("NoPic");
                    }
                    else
                        t2d = MenuHandler.LoadPic(o[0].dirPath + "/" + o[0].BGfileName);
                    TextureScale.Bilinear(t2d, (int)(((float)t2d.width / (float)t2d.height) * 100), 100);
                    byte[] bytes = t2d.EncodeToPNG();
                    File.WriteAllBytes(thumbnailPath, bytes);
                    Debug.Log("Finished Making Thumbnail! Path= <color=yellow>" + thumbnailPath + "</color>");
                }
                else
                {
                    t2d = MenuHandler.LoadPic(thumbnailPath);
                }
            }
#endif
            RawImage bg = go.transform.Find("Content/Gradient/SongBG").GetComponent<RawImage>();
            bg.texture = t2d;
            float bgXscale = t2d.width / t2d.height;
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(
                bg.GetComponent<RectTransform>().sizeDelta.x*bgXscale, bg.GetComponent<RectTransform>().sizeDelta.y);

            //info設定
            go.transform.Find("Content/Indicator/Title").GetComponent<Text>().text = o[0].Title +" - "+ o[0].Artist;
            string dif = "";
            for(int j = 0; j < o.Count; j++)
            {
                if(j > 0)   dif+=" | ";
                dif += o[j].Version;
                dif += $"(★{o[j].OverallDifficulty})";
            }
            go.transform.Find("Content/Indicator/Description").GetComponent<Text>().text = dif;
            string sp = songList[i].path;//, t = thumbnailPath;
            go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener( 
                ()=>DifficultyPrinter.instance.Print(sp, t2d, o.ToArray()) );
            
        } 

        buttU.SetActive(false);
        buttD.SetActive(false);
        #endregion
    }

    /// <summary>
    /// 刷新歌單後列印
    /// </summary>
    public void Reprint()
    {
        foreach(var go in buttList)
        {
            if (go.tag != Sauce.resources.ToString())
            {
                Destroy(go.transform.Find("Content/Gradient/SongBG").GetComponent<RawImage>().texture);
            }
            Destroy(go);  
        }
        buttList.Clear();
        Awake();
    }

    /// <summary>
    /// 不刷新歌單重印
    /// </summary>
    public void ReprintNoRefresh()
    {
        foreach(var go in buttList)
        {
            if (go.tag == Sauce.nou.ToString())
            {
                go.SetActive(printNouImg.enabled);
            }
            else if(go.tag == Sauce.osu.ToString())
            {
                go.SetActive(printOsuImg.enabled);
            }
            else if(go.tag == Sauce.resources.ToString())
            {
                go.SetActive(printResImg.enabled);
            }
        }
    }

    /// <summary>
    ///  nou, osu, res
    /// </summary>
    /// <param name="toggleSauce"></param>
    public void TogglePrint(string toggleSauce)
    {
        switch(toggleSauce)
        {
            case "nou":
                printNouImg.enabled = !printNouImg.enabled;
                break;
            case "osu":
                printOsuImg.enabled = !printOsuImg.enabled;
                break;
            case "res":
                printResImg.enabled = !printResImg.enabled;
                break;
        }
        ReprintNoRefresh();
        ReArrangeButts();
    }

    /// <summary>
    /// 重新排列歌曲按鈕位置
    /// </summary>
    void ReArrangeButts()
    {
        int cPos = 0;//相對於第一個butt，位置差
        int songOrder = 0;
        foreach (GameObject go in buttList)
        {
            go.transform.SetParent(shouldPlaceIn.transform, true);
            Vector3 p = go.GetComponent<RectTransform>().localPosition;

            
            if (go.activeInHierarchy)
            {
                go.GetComponent<RectTransform>().localPosition = new Vector3(initPos + cPos, initPosY[songOrder % 2], p.z);
                cPos += 50;
                Vector2 v = shouldPlaceIn.GetComponent<RectTransform>().offsetMax;
                shouldPlaceIn.GetComponent<RectTransform>().offsetMax = new Vector2(300 + cPos, v.y); //L padding
                songOrder++;
                
            }
        }
        
    }

}