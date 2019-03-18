using UnityEngine;
using System.IO;

public class Userpref : MonoBehaviour {
    public static Userpref instance;
    public Pref data; 

    [System.Serializable]
    public class Pref
    {
        public int skinType = 0;
    }

    void Awake()
    {
        instance = this;
        Load();
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(this.data);
        File.WriteAllText(Application.persistentDataPath+"\\Userpref.txt", json);
        Debug.Log("存檔完成！ Path="+Application.persistentDataPath+"\\Userpref.txt");
    }
    public void Load()
    {
        if( !File.Exists(Application.persistentDataPath+"\\Userpref.txt") )
        {
            Debug.LogWarning("未找到存檔！重新建立中");
            Save();
        }  
        string json = File.ReadAllText(Application.persistentDataPath+"\\Userpref.txt");
        var n = JsonUtility.FromJson<Pref>(json);
        this.data = n;
        Debug.Log("讀取存檔完成！ Path="+Application.persistentDataPath+"\\Userpref.txt");
    }
}