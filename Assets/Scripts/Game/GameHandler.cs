﻿//ref: https://github.com/ElonGame/osu_unity-mp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameHandler : MonoBehaviour
{
    // ----------------------------------------------------------------------------

    [Header("Objects")]
    public GameObject Circle; // Circle Object
    [SerializeField] RawImage BG; // backgrounf
    [SerializeField] AudioSource BGM; // BGM player
    [SerializeField] VideoPlayer BGMovie;

    [Header("Map")]
    public TextAsset MapPath; // Map Path
    public TextAsset MapFile; // Map file (.osu format)
    public AudioClip MainMusic; // Music file, attach from editor
    public AudioClip HitSound; // Hit sound

    // ----------------------------------------------------------------------------

    const int SPAWN = -100; // Spawn coordinates for objects

    public static double timer = 0; // Main song timer
    public static int ApprRate = 600; // Approach rate (in ms)
    private int DelayPos = 0; // Delay song position

    static float scorePerCircle = 0;

    public static int ClickedCount = 0; // Clicked objects counter
    public static float score = 0;
    public static float totalScore = 0; //current total score to calc percentage
    public static int combo = 0;
    private static int ObjCount = 0; // Spawned objects counter

    [SerializeField]
    private List<GameObject> CircleList; // Circles List
    private static string[] LineParams; // Object Parameters

    // Audio stuff
    private AudioSource Sounds;
    private AudioSource Music;
    public static AudioSource pSounds;
    public static AudioClip pHitSound;

    // Other stuff
    private Camera MainCamera;
    private GameObject CursorTrail;
    private Vector3 MousePosition;
    private Ray MainRay;
    private RaycastHit MainHit;
    private Text comboText;
    private Text scoreText;
    private Text percentageText;

    private void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Music = GameObject.Find("Music Source").GetComponent<AudioSource>();
        Sounds = gameObject.GetComponent<AudioSource>();
        CursorTrail = GameObject.Find("Cursor Trail");
        comboText = GameObject.Find("Canvas/Combo").GetComponent<Text>();
        scoreText = GameObject.Find("Canvas/Score").GetComponent<Text>();
        percentageText = GameObject.Find("Canvas/Percentage").GetComponent<Text>();

        Music.clip = MainMusic;
        pSounds = Sounds;
        pHitSound = HitSound;
        
        if(GameObject.Find("GameValue"))
        {
            Debug.Log("找到GameValue。正在套用");
            ToGameValue v = GameObject.Find("GameValue").GetComponent<ToGameValue>();
            ReadCircles(v.FinalOsu.path);
            BGM.clip = v.FinalMusic;
            if( !string.IsNullOrEmpty(v.FinalOsu.BGmoviePath) )
                BGMovie.url = v.FinalOsu.BGmoviePath;
            else
                BG.texture = v.FinalBG;
            //Destroy(v.gameObject);
        }
        else
        {
            #if UNITY_EDITOR
                ReadCircles(UnityEditor.AssetDatabase.GetAssetPath(MapFile));
            #else
                Debug.LogError("未找到GameValue! 這不該發生!");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            #endif
        }
        
        GameStart();
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        DiscordHandler.instance.SetPresence("Playing song", 
            string.Format("{0} ({1:F2}%) | {2:N0}x", score, score/totalScore*100f , combo) );
    }

    /// <summary>
    ///讀取map
    ///</summary>
    void ReadCircles(string path)
    {
        CircleList = new List<GameObject>();
        StreamReader reader = new StreamReader(path);
        string line;

        // Skip to [HitObjects] part
        while(true)
        {
            if (reader.ReadLine() == "[HitObjects]")
                break;
        }
            
        int TotalLines = 0;

        // Count all lines
        while (!reader.EndOfStream)
        {
            reader.ReadLine();
            TotalLines++;
        }

        reader.Close();

        reader = new StreamReader(path);

        // Skip to [HitObjects] part again
        while(true)
        {
            if (reader.ReadLine() == "[HitObjects]")
                break;
        }

        // Sort objects on load
        int ForeOrder = TotalLines + 2; // Sort foreground layer
        int BackOrder = TotalLines + 1; // Sort background layer
        int ApproachOrder = TotalLines; // Sort approach circles layer

        // Some crazy Z axis modifications for sorting
        string TotalLinesStr = "0.";
        for (int i = 3; i > TotalLines.ToString().Length; i--)
            TotalLinesStr += "0";
        TotalLinesStr += TotalLines.ToString();
        float Z_Index = -(float.Parse(TotalLinesStr));

        GameObject Parent = Instantiate(new GameObject());
        Parent.name = "Medkits' parent";

        while (!reader.EndOfStream)
        {
            // Uncomment to skip sliders
            /*while (true)
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    if (!line.Contains("|"))
                        break;
                }
                else
                    break;
            }*/

            line = reader.ReadLine();
            if (line == null)
                break;

            LineParams = line.Split(','); // Line parameters (X&Y axis, time position)

            // Sorting configuration
            GameObject CircleObject = Instantiate(Circle, new Vector2(SPAWN, SPAWN), Quaternion.identity);
            CircleObject.GetComponent<Circle>().Fore.sortingOrder = ForeOrder;
            CircleObject.GetComponent<Circle>().Back.sortingOrder = BackOrder;
            CircleObject.GetComponent<Circle>().Appr.sortingOrder = ApproachOrder;
            CircleObject.transform.localPosition += new Vector3((float) CircleObject.transform.localPosition.x, (float) CircleObject.transform.localPosition.y, (float) Z_Index);
            CircleObject.transform.SetAsFirstSibling();
            ForeOrder--; BackOrder--; ApproachOrder--; Z_Index += 0.01f;

            int FlipY = 384 - int.Parse(LineParams[1]); // Flip Y axis

            int AdjustedX = Mathf.RoundToInt(Screen.height * 1.333333f); // Aspect Ratio

            // Padding
            float Slices = 8f;
            float PaddingX = AdjustedX / Slices;
            float PaddingY = Screen.height / Slices;

            // Resolution set
            float NewRangeX = ((AdjustedX - PaddingX) - PaddingX);
            float NewValueX = ((int.Parse(LineParams[0]) * NewRangeX) / 512f) + PaddingX + ((Screen.width - AdjustedX) / 2f);
            float NewRangeY = Screen.height;
            float NewValueY = ((FlipY * NewRangeY) / 512f) + PaddingY;

            Vector3 MainPos = MainCamera.ScreenToWorldPoint(new Vector3 (NewValueX, NewValueY, 0)); // Convert from screen position to world position
            Circle MainCircle = CircleObject.GetComponent<Circle>();

            MainCircle.Set(MainPos.x, MainPos.y, CircleObject.transform.position.z, int.Parse(LineParams[2]) - ApprRate);
            CircleObject.transform.SetParent(Parent.transform);

            CircleList.Add(CircleObject);
        }
        scorePerCircle = 1000000f / (float)CircleList.Count;
        Debug.Log("Done Reading Map! (副檔名: .osu)");
    }

    // END MAP READER
	
    private void GameStart()
    {
        Application.targetFrameRate = -1; // Unlimited Frame Rate
        Music.Play();
        StartCoroutine(UpdateRoutine()); // Using coroutine instead of Update()
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            timer = (Music.time * 1000); // Convert timer
            if(ObjCount < CircleList.Count)
                DelayPos = (CircleList[ObjCount].GetComponent<Circle>().PosA);
            else
                DelayPos = int.MaxValue;//end, cannot spawn anymore!
            MainRay = MainCamera.ScreenPointToRay(Input.mousePosition);

            // Spawn object
            if (timer >= DelayPos)
            {
                CircleList[ObjCount].GetComponent<Circle>().Spawn();
                ObjCount++;
            }

            // Check if cursor is over object
            if (Physics.Raycast(MainRay, out MainHit))
            {
                if (MainHit.collider.gameObject.GetComponent<Circle>() && timer >= MainHit.collider.gameObject.GetComponent<Circle>().PosA + ApprRate)
                {
                    MainHit.collider.gameObject.GetComponent<Circle>().Got();
                    MainHit.collider.enabled = false;
                    ClickedCount++;
                    
                }
            }

            // Cursor trail movement
            MousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            CursorTrail.transform.position = new Vector3(MousePosition.x, MousePosition.y, -9);

            //cb
            comboText.text = string.Format("{0:N0}", combo); //+ " Kills / "+ ClickedCount+ " Players";
            scoreText.text = string.Format("{0:F0}", score);
            percentageText.text = string.Format("{0:F2} %", score/totalScore*100f);

            yield return null;
        }
    }


    static public void GotCircle(bool isGet)
    {
        if(isGet)
        {
            combo ++;
            score += scorePerCircle;
        }
        else
        {
            combo = 0;
            score += 0;
        }
        totalScore += scorePerCircle;
    }

}
