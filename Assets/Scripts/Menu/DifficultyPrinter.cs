using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class DifficultyPrinter : MonoBehaviour 
{
    public static DifficultyPrinter instance;
    [SerializeField] GameObject buttU, buttD;
    [SerializeField] Text confirmPanel_Text;
    [SerializeField] AudioSource audioPlayer;
    public Button mod_AutoMove, mod_AutoClick;
    public List<GameObject> buttList;


    void Awake()
    {
        instance = this;
    }
    public void Print(string songDirPath, string thumbnailPath)
    {
        foreach(var go in buttList)
        {
            Destroy(go);
        }
        buttList.Clear();
        Destroy(audioPlayer.clip);

        Texture2D t2d = MenuHandler.LoadPic(thumbnailPath);
        List<OsuFile> o = new List<OsuFile>();
        foreach(var f in Directory.GetFiles(songDirPath))
        {
            if(Path.GetExtension(f) == ".osu")
            {
                OsuFile no = new OsuFile(f);
                o.Add(no);
                ToGameValue.instance.FinalOsu = no; //先預先設定，以便後面獲取Audio
            }
        }
        if(o.Count == 0)
        {
            Debug.LogError("沒有找到.osu");
            return;
        }   
            
        int cPos = 0;//相對於第一個butt，位置差
        for(int i = 0; i < o.Count; i++)
        {
            GameObject go;
            if(i % 2 == 0)
                go = Instantiate(buttU, buttU.transform.parent);
            else
                go = Instantiate(buttD, buttD.transform.parent);
            
            go.name = "Button "+i;
            Vector3 p = go.GetComponent<RectTransform>().localPosition;
            go.GetComponent<RectTransform>().localPosition = new Vector3(p.x+cPos, p.y, p.z);
            cPos += 50;
            go.SetActive(true);
            buttList.Add(go);
                
            go.transform.Find("Content/Indicator/Title").GetComponent<Text>().text = o[i].Title +" - "+ o[i].Artist;
            go.transform.Find("Content/Indicator/Description").GetComponent<Text>().text = o[i].Creator +"\'s "+ o[i].Version;
            go.transform.Find("Content/Gradient/SongBG").GetComponent<RawImage>().texture = t2d;
            string ojson = JsonUtility.ToJson(o[i]);
            go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener( ()=>GoConfirm(ojson) );
        } 

        buttU.SetActive(false);
        buttD.SetActive(false);     
    }

    void GoConfirm(string osujson)
    {
        OsuFile osu = JsonUtility.FromJson<OsuFile>(osujson);
        ToGameValue.instance.FinalOsu = osu;
        PrintConfirm();
    }
    void PrintConfirm()
    {
        OsuFile osu = ToGameValue.instance.FinalOsu;

        string modString = "";
        foreach(var m in Userpref.data.mods)
        {
            modString += ", "+PlayStat.ModString[(int)m];
        }
        if(modString == "")
            modString = "null";
        else
            modString = modString.Substring(2);

        string s = $"Mods: <color=yellow>{modString}</color>\n";
        s     +=   $"<color=yellow>{osu.Title}</color> - <color=yellow>{osu.Artist}</color>\n";
        s     +=   $"<color=yellow>{osu.Creator}</color>\'s <color=yellow>{osu.Version}</color>\n";
        s += "<size=20><b>ARE YOU READY?</b></size>";
        confirmPanel_Text.text = s;
        mod_AutoClick.transform.Find("Icon").GetComponent<Image>().enabled = Userpref.HasModStatic(PlayStat.Mods.AutoClick);
        mod_AutoMove.transform.Find("Icon").GetComponent<Image>().enabled = Userpref.HasModStatic(PlayStat.Mods.AutoMove);
    }

    public void ModButt_AutoClick_OnClick()
    {
        Userpref.ToggleModStatic(PlayStat.Mods.AutoClick);
        PrintConfirm();
    }
    public void ModButt_AutoMove_OnClick()
    {
        Userpref.ToggleModStatic(PlayStat.Mods.AutoMove);
        PrintConfirm();
    }

    public void Butt_StartGoGame()
    {
        StartCoroutine( GoGameAsync() );
    }
    IEnumerator GoGameAsync()
    {
        //GetMusic and init the game value
        ToGameValue.instance.Set();
        confirmPanel_Text.text = confirmPanel_Text.text.Replace("ARE YOU READY?", "NOW LOADING...");
        Userpref.Save();

        var L = SceneManager.LoadSceneAsync("Main");
        while(!L.isDone)
        {
            yield return L.progress;
        }
    }


}