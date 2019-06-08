using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ToGameValue : MonoBehaviour 
{
    public static ToGameValue instance;

    [Header("Bring these to Game...")]
    //public bool isFromResource = false;
    public OsuFile FinalOsu;
    public AudioClip FinalMusic;
    public Texture2D FinalBG;

    void Start()
    {
        instance = this;
    }

    ///<summary>
    /// 依據 FinalOsu 來設定GameValue
    ///</summary>
    public void Set()
    {
        Debug.Log($"[GV] isFromAsset={FinalOsu.isFromAsset} | DirPath={FinalOsu.dirPath}");
        if (FinalOsu.isFromAsset)
        {
            FinalBG = Resources.Load<Texture2D>($"{FinalOsu.dirPath}/{Path.GetFileNameWithoutExtension(FinalOsu.BGfileName)}");
            FinalMusic = Resources.Load<AudioClip>($"{FinalOsu.dirPath}/{Path.GetFileNameWithoutExtension(FinalOsu.AudioFilename)}");
        }
        else
        {
            FinalBG = MenuHandler.LoadPic($"{FinalOsu.dirPath}\\{FinalOsu.BGfileName}");
            AudioClip aud = LoadBGM($"{FinalOsu.dirPath}\\{FinalOsu.AudioFilename}");
            FinalMusic = aud;
        }

        if (!FinalBG)
            Debug.LogError("No BG Loaded!");
        if (!FinalMusic)
            Debug.LogError("No Audio Loaded!");

        Debug.Log($"Set Game Value! BGM_Length={FinalMusic.length} | BG_size={FinalBG.width}x{FinalBG.height}");
        //return aud;
    }

    static AudioClip LoadBGM(string path)
    {
        string type = Path.GetExtension(path).Replace(".","");
        Debug.Log("[Audio] path="+path+" | type="+type );
        AudioClip myClip = null;
        UnityWebRequest www = null;
        byte[] byteArray;
        try
        {
            switch(type.ToLower())
            {
                case "ogg":
                    #if UNITY_STANDALONE || UNITY_EDITOR
                        www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS);
                        www.SendWebRequest();
                        while(!www.isDone){}
                        myClip = DownloadHandlerAudioClip.GetContent(www);
                    #else
                        byteArray = File.ReadAllBytes(path);
                        myClip = WAV.FromOggData(byteArray);
                    #endif
                    break;
                case "mp3":
                    #if UNITY_STANDALONE || UNITY_EDITOR
                        byteArray = File.ReadAllBytes(path);
                        myClip = NAudioPlayer.FromMp3Data(byteArray);
                    #else
                        www = UnityWebRequestMultimedia.GetAudioClip("file://"+path, AudioType.MPEG);
                        www.SendWebRequest();
                        while(!www.isDone){}
                        myClip = DownloadHandlerAudioClip.GetContent(www);
                    #endif
                    break;
                case "wav":
                    byteArray = File.ReadAllBytes(path);
                    myClip = WAV.FromWavData(byteArray);
                    break;
                default:
                    Debug.LogError("Unexpected audio type");
                    return null;
            }
            Debug.Log("[Audio] Audio Length: " + myClip.length );
            return myClip;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Audio]錯誤！"+e);
            return null;
        }
    }

}