using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPrinter : MonoBehaviour 
{
    public static DifficultyPrinter instance;
    [SerializeField] GameObject buttU, buttD;
    public List<GameObject> buttList;

    void Awake()
    {
        instance = this;
    }
    public void Print(string songDirPath)
    {
        foreach(var go in buttList)
        {
            Destroy(go);
        }
        buttList.Clear();

        List<OsuFile> o = new List<OsuFile>();
        foreach(var f in Directory.GetFiles(songDirPath))
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
        } 

        buttU.SetActive(false);
        buttD.SetActive(false);     
    }


}