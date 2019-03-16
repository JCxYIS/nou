using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
            Debug.Log("Get Dir: "+item);
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
            go.name = "Button "+i;
            Vector3 p = go.GetComponent<RectTransform>().localPosition;
            go.GetComponent<RectTransform>().localPosition = new Vector3(p.x+cPos, p.y, p.z);
            cPos += 50;
            go.SetActive(true);
            buttList.Add(go);
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
        Awake();
    }

}