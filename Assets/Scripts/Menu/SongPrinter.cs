using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SongPrinter : MonoBehaviour 
{
    [SerializeField] GameObject buttU, buttD;
    void Awake()
    {
        List<string> songList = new List<string>();
        if(!Directory.Exists(Application.persistentDataPath + "\\Songs\\"))
            Directory.CreateDirectory(Application.persistentDataPath + "\\Songs\\");

        foreach (var item in Directory.GetFiles(Application.persistentDataPath + "\\Songs\\"))
        {
            Debug.Log("Get Song: "+item);
            songList.Add(item);
        }


        int cPos = 0;//相對於第一個butt，位置差
        for(int i = 0; i < 20; i++)
        {
            GameObject go;
            if(i % 2 == 0)
                go = Instantiate(buttU, buttU.transform.parent);
            else
            {
                go = Instantiate(buttD, buttD.transform.parent);
            }
            go.name = "Button "+i;
            Vector3 p = go.GetComponent<RectTransform>().localPosition;
            go.GetComponent<RectTransform>().localPosition = new Vector3(p.x+cPos, p.y, p.z);
            cPos += 50;
        } 

        buttU.SetActive(false);
        buttD.SetActive(false);     
    }
}