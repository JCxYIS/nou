using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongPrinter : MonoBehaviour 
{
    public static SongPrinter instance;
    [SerializeField] GameObject buttU, buttD;
    public List<GameObject> buttList;

    void Awake()
    {
        instance = this;

        List<string> songList = new List<string>();
        if(!Directory.Exists(Application.persistentDataPath + "\\Songs\\"))
            Directory.CreateDirectory(Application.persistentDataPath + "\\Songs\\");

        foreach (var item in Directory.GetDirectories(Application.persistentDataPath + "\\Songs\\"))
        {
            Debug.Log("獲取曲包 路徑: "+item);
            songList.Add(item);
        }

        int cPos = 0;//相對於第一個butt，位置差
        for(int i = 0; i < songList.Count; i++)
        {
            GameObject go;
            if(i % 2 == 0)
                go = Instantiate(buttU, buttU.transform.parent);
            else
                go = Instantiate(buttD, buttD.transform.parent);
            
            go.name = "Button ("+i+")";
            Vector3 p = go.GetComponent<RectTransform>().localPosition;
            go.GetComponent<RectTransform>().localPosition = new Vector3(p.x+cPos, p.y, p.z);
            cPos += 50;
            go.SetActive(true);
            buttList.Add(go);

            List<OsuFile> o = new List<OsuFile>();
            foreach(var f in Directory.GetFiles(songList[i]))
            {
                if(Path.GetExtension(f) == ".osu")
                {
                    o.Add( new OsuFile(f) );
                }
            }
            if(o.Count == 0)
            {
                Debug.LogError("沒有找到.osu");
                return;
            }    
            
            //Load BG
            string thumbnailPath = Application.persistentDataPath + "\\.thumbnail";
            Directory.CreateDirectory(thumbnailPath);
            thumbnailPath += "\\"+Path.GetFileName(songList[i])+".png";
            Texture2D t2d = new Texture2D(1,1);
            if( !File.Exists(thumbnailPath) )
            {
                t2d = MenuHandler.LoadPic(o[0].BGpath);
                TextureScale.Bilinear(t2d, (int)( ((float)t2d.width / (float)t2d.height)*100 ), 100);
                byte[] bytes = t2d.EncodeToPNG();
                File.WriteAllBytes(thumbnailPath, bytes);
                Debug.Log("Finished Making Thumbnail! Path= <color=yellow>"+thumbnailPath+"</color>");
            }
            else
            {
                t2d = MenuHandler.LoadPic(thumbnailPath);
            }
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
            }
            go.transform.Find("Content/Indicator/Description").GetComponent<Text>().text = dif;
            string sp = songList[i], t = thumbnailPath;
            go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener( ()=>DifficultyPrinter.instance.Print(sp, t) );
            
        } 

        buttU.SetActive(false);
        buttD.SetActive(false);     
    }

    public void Reprint()
    {
        foreach(var go in buttList)
        {
            Destroy(go);
        }
        buttList.Clear();
        Awake();
    }

}