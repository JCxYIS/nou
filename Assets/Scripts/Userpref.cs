using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Userpref : MonoBehaviour {
    //public static Userpref instance;
    static public Pref data; 

    [System.Serializable]
    public class Pref
    {
        public int skinType = 0;
        public int calcScoreMod = 0;
        public float volumeBgm = 1;
        public float volumeSfx = 0.87f;
        public List<PlayStat.Mods> mods = null;
        public string customOsuPath = "";
        ///<summary>
        /// 用法: bestRecords["{曲名Title} - {作者Artist}"][{作圖者Creator}'s {難度Version}]；
        /// 推薦使用LoadBestRecord()
        ///</summary>
        public Dictionary<string, Dictionary<string, BestRecord> > bestRecords = null;
    }
    [System.Serializable]
    public class BestRecord
    {
        public PlayStat.Mods[] usingMods;
        public double originalScore;
        public double trueScore;
    }

    void Awake()
    {
        //instance = this;
        Load();
    }

    public void DoSave()
    {
        Save();
    }
    static public void Save()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath+"\\Userpref.txt", json);
        Debug.Log("存檔完成！ Path="+Application.persistentDataPath+"\\Userpref.txt");
    }
    static public void Load()
    {
        if( !File.Exists(Application.persistentDataPath+"\\Userpref.txt") )
        {
            Debug.LogWarning("未找到存檔！重新建立中");
            data = new Pref();
            Save();
        }  
        string json = File.ReadAllText(Application.persistentDataPath+"\\Userpref.txt");
        var n = JsonUtility.FromJson<Pref>(json);
        data = n;
        Debug.Log("讀取存檔完成！ Path="+Application.persistentDataPath+"\\Userpref.txt");
    }

    static public bool HasModStatic(PlayStat.Mods mod)
    {
        foreach (var m in data.mods)
        {
            if(m == mod)
                return true;
        }
        //Debug.Log("Not "+mod);
        return false;
    }
    static public void ToggleModStatic(PlayStat.Mods mod)
    {
        if(HasModStatic(mod))
        {
            data.mods.Remove(mod);
            Debug.Log("Remove Mod:"+mod);
        } 
        else
        {
            data.mods.Add(mod);
            Debug.Log("Add Mod:"+mod);
        }
        Save();
    }
    ///<summary>
    /// 如果輸入的成績(trueScore)比原本高，便會覆蓋原資料
    ///</summary>
    static public void SetBestRecord(OsuFile osu, PlayStat playStat)
    {
        BestRecord best = LoadBestRecord(osu);
        if(playStat.trueScore > best.trueScore || 
            (playStat.trueScore == best.trueScore && playStat.mods.Length < best.usingMods.Length) )
        {
            best.trueScore = playStat.trueScore;
            best.usingMods = playStat.mods;
            best.originalScore = playStat.score;
        }
    }
    static public BestRecord LoadBestRecord(string title, string artist, string creator, string version)

    {
        return data.bestRecords[$"{title} - {artist}"][$"{creator}'s {version}"];
    }
    static public BestRecord LoadBestRecord(OsuFile osu)
    {
        return LoadBestRecord(osu.Title, osu.Artist, osu.Creator, osu.Version);
    }
}