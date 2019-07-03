//ref: https://github.com/ElonGame/osu_unity-mp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    static public GameHandler instance;
    // ----------------------------------------------------------------------------

    [Header("Objects")]
    public GameObject Circle; // Circle Object
    [SerializeField] RawImage BG; // backgrounf
    [SerializeField] AudioSource BGM; // BGM player
    [SerializeField] VideoPlayer BGMovie;
    public GameObject[] noteResult; //顯示Perfect, good之類的
    [SerializeField] Slider progressBar;
    [SerializeField] GameObject autoMoveCursor;
    [SerializeField] HealthBar SPbar;
    [SerializeField] HealthBar HPbar;

    #if UNITY_EDITOR
    [Header("In Editor Test")]
    public string TestMapPath; // Map file (.osu format)
    public AudioClip TestMusic;
    public Texture2D TestImage;
    #endif

    [Header("In Game")]
    public AudioClip MainMusic; // Music file, attach from editor
    public AudioClip HitSound; // Hit sound
    public double timer = 0; // Main song timer
    public int ApprRate = 600; // NOTE產生於 (ms) 秒前
    private int DelayPos = 0; // Delay song position

    private int ObjCount = 0; // Spawned objects counter
    private int BestObjCount = 0; //理想路徑點過的note
    public int ClickedObjCount = 0; //點過的note
    private float BestMoveSpeed = 0; //理想到下一點的speed
    public PlayStat playStat;


    [SerializeField]
    private List<GameObject> CircleList; // Circles List
    //private static string[] LineParams; // Object Parameters
    private float endGameTime = 1; //還有幾秒節算?
    

    // Audio stuff
    private AudioSource Sounds;//attach to self
    private AudioSource Music;//attach to "Music Source"
    public static AudioSource pSounds;
    public static AudioClip pHitSound;

    // Other stuff
    private Camera MainCamera;
    private GameObject CursorTrail;
    private Vector3 MousePosition;
    private Ray MainRay;
    private RaycastHit MainHit;
    private Text comboText;
    private Number scoreText;
    private Text percentageText;
    static public Transform WorldCanvas;

    #region UNITY
    private void Start()
    {
        instance = this;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Music = GameObject.Find("Music Source").GetComponent<AudioSource>();
        Sounds = gameObject.GetComponent<AudioSource>();
        CursorTrail = GameObject.Find("Cursor Trail");
        comboText = GameObject.Find("Canvas/Combo").GetComponent<Text>();
        scoreText = GameObject.Find("Canvas/Score").GetComponent<Number>();
        percentageText = GameObject.Find("Canvas/Percentage").GetComponent<Text>();
        WorldCanvas = GameObject.Find("Canvas World").GetComponent<Transform>();

        DontDestroyOnLoad(playStat.gameObject);

        switch(Userpref.data.skinType)
        {
            case 1:
                Circle = Resources.Load<GameObject>("Skin1/CircleBhe");
                HitSound = Resources.Load<AudioClip>("Skin1/ㄅtrim");
                break;
            default:
            case 0:
            Debug.Log("USING SKIN"+Userpref.data.skinType);
                Circle = Resources.Load<GameObject>("Skin0/Circle");
                HitSound = Resources.Load<AudioClip>("Skin0/player_knocked");
                break;
        }
        
        if(GameObject.Find("GameValue"))
        {
            Debug.Log("找到GameValue。正在套用");
            ToGameValue v = GameObject.Find("GameValue").GetComponent<ToGameValue>();
            playStat.playing = v.FinalOsu;
            if (v.FinalOsu.isFromAsset)
            {
                TextAsset txt = Resources.Load<TextAsset>( v.FinalOsu.path.Replace("RESOURCES/", "") );
                string p = $"{Application.temporaryCachePath}/{Path.GetRandomFileName()}";
                File.WriteAllText(p, txt.text);
                Debug.Log("Song is from Resources! TempSheetPath=" + p);
                ReadCircles(p);
            }
            else
                ReadCircles(v.FinalOsu.path);
            MainMusic = v.FinalMusic;
            if( !string.IsNullOrEmpty(v.FinalOsu.BGmovieFileName) )
                BGMovie.url = Path.Combine(v.FinalOsu.dirPath, v.FinalOsu.BGmovieFileName);
            else
                BG.texture = v.FinalBG;
            Destroy(v.gameObject);
        }
        else
        {
            #if UNITY_EDITOR
                ReadCircles( Application.dataPath+"/"+TestMapPath );
                MainMusic = TestMusic;
                BG.texture = TestImage;
            #else
                Debug.LogError("未找到GameValue! 這不該發生!");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            #endif
        }
        playStat.mods = Userpref.data.mods.ToArray();

        if (!playStat.HasMod(PlayStat.Mods.AutoMove))
            autoMoveCursor.SetActive(false);

        Music.clip = MainMusic;
        pSounds = Sounds;
        pHitSound = HitSound;        
        GameStart();
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync("Score");
        }
        try
        {
            DiscordHandler.instance.SetPresence(
            playStat.playing.Title + " - " + playStat.playing.Artist + "(" + playStat.playing.Creator+"'s "+playStat.playing.Version+")", 
            string.Format("{0:F0} ({1:F2}%) | {2:N0}x", playStat.score, playStat.percentage , playStat.combo) );
        }
        catch
        {
            Debug.LogError("Cannot set Discord!");
        }
        
        if(Music.isPlaying == false)
        {
            endGameTime -= Time.deltaTime;
        }
        if(endGameTime <= 0 )
        {
            SceneManager.LoadSceneAsync("Score");
        }
    }
    #endregion

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
        float Z_Index = -5; //-(float.Parse(TotalLinesStr));

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

            string[] LineParams = line.Split(','); // Line parameters (X&Y axis, time position)

            // Sorting configuration
            GameObject CircleObject = Instantiate(Circle, new Vector2(-999, -999), Quaternion.identity);
            CircleObject.GetComponent<Circle>().Fore.sortingOrder = ForeOrder;
            CircleObject.GetComponent<Circle>().Back.sortingOrder = BackOrder;
            CircleObject.GetComponent<Circle>().Appr.sortingOrder = ApproachOrder;
            CircleObject.transform.localPosition = new Vector3(-999, -999, (float) Z_Index);
            CircleObject.transform.SetAsFirstSibling();
            CircleObject.name = "Medkit #"+Z_Index;
            ForeOrder++; BackOrder++; ApproachOrder++; Z_Index -= 0.0001f;

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
        playStat.Init(CircleList);
        Debug.Log("Done Reading Map! (副檔名: .osu)");
    }

    // END MAP READER
	
    private void GameStart()
    {
        Application.targetFrameRate = -1; // Unlimited Frame Rate
        Music.volume = Userpref.data.volumeBgm;
        Music.Play();
        StartCoroutine(UpdateRoutine()); // Using coroutine instead of Update()
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            // Spawn object
            timer = (Music.time * 1000); // Convert timer
            if(ObjCount < CircleList.Count)
                DelayPos = (CircleList[ObjCount].GetComponent<Circle>().PosA);
            else
                DelayPos = int.MaxValue; //end, cannot spawn anymore!

            if (timer >= DelayPos)
            {
                CircleList[ObjCount].GetComponent<Circle>().Spawn();
                ObjCount++;
            }

            // Pre: AutoMove mod
            Vector3 mousePos = Input.mousePosition;
            if( playStat.HasMod(PlayStat.Mods.AutoMove) )
            {
                mousePos = MainCamera.WorldToScreenPoint(transform.position);
            }


            // Check if cursor is over object
            MainRay = MainCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(MainRay, out MainHit))
            {
                if (MainHit.collider.gameObject.GetComponent<Circle>())
                {
                    //Auto Full 的判斷在 circle 裡
                    //Debug.Log(timer+" Touched! Should="+(ApprRate + CircleList[ClickedObjCount].GetComponent<Circle>().PosA) );
                    if(playStat.HasMod(PlayStat.Mods.AutoClick) && !playStat.HasMod(PlayStat.Mods.AutoMove))   // auto click
                    {    if(timer >= ApprRate + CircleList[ClickedObjCount].GetComponent<Circle>().PosA - PlayStat.noteOffset[0])
                        {
                            MainHit.collider.gameObject.GetComponent<Circle>().Got();
                            ClickedObjCount++;
                            MainHit.collider.enabled = false;
                        }
                    }
                    else if(Input.anyKeyDown)
                    {
                        MainHit.collider.gameObject.GetComponent<Circle>().Got();
                        ClickedObjCount++;
                        MainHit.collider.enabled = false;
                    }                    
                }
            }
                

            // Determine best Pos
            if(BestObjCount < CircleList.Count)
            {               
                    transform.position = Vector3.MoveTowards(
                        transform.position, 
                        CircleList[BestObjCount].GetComponent<Circle>().MyPos() + new Vector3(0,0,0.1f), 
                        BestMoveSpeed * Time.deltaTime);         
            }
            else
            {
                CursorTrail.SetActive(false);
            }
            if (timer >= ApprRate + CircleList[BestObjCount].GetComponent<Circle>().PosA)
            { 
                if(BestObjCount+1 < CircleList.Count)
                {
                    Circle Obj1 = CircleList[BestObjCount].GetComponent<Circle>();
                    Circle Obj2 = CircleList[BestObjCount+1].GetComponent<Circle>();                
                    transform.position = Obj1.MyPos();
                    BestMoveSpeed = Vector3.Distance( Obj2.MyPos(), transform.position); // distance
                    BestMoveSpeed /= ( (float)(Obj2.PosA - Obj1.PosA) )/1000f; // time
                    BestObjCount ++;                          
               }
                //Debug.Log("BestMoveSpeed="+BestMoveSpeed);
            }


            // Cursor trail movement
            MousePosition = MainCamera.ScreenToWorldPoint(mousePos);
            CursorTrail.transform.position = new Vector3(MousePosition.x, MousePosition.y, -9);

            // UI Display
            Combo.Set(playStat.combo);
            scoreText.Set((float)playStat.score);
            percentageText.text = string.Format("{0:F2} %", playStat.percentage);
            progressBar.value = BGM.time / BGM.clip.length;
            HPbar.Set(playStat.hp, playStat.usingChara.hp);
            SPbar.Set(playStat.sp, 36550666f);

            // GamePlay Logic
            isdead?

            yield return null;
        }
    }

}
