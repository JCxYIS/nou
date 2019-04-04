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
        public List<PlayStat.Mods> mods;
    }

    void Awake()
    {
        //instance = this;
        Load();
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
}