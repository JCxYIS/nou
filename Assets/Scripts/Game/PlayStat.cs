using System.Collections.Generic;
using UnityEngine;

public class PlayStat : MonoBehaviour 
{
    static public PlayStat instance;

    public OsuFile playing = null; // playing 
    static public string[] ModString = {"Groove Coaster", "Auto Click"}; 

    ///<summary>
    /// Mods are gay!
    ///</summary> 
    public enum Mods {AutoMove, AutoClick}

    ///<summary>
    /// 計分方式
    ///</summary> 
    static public string[] CalcMode = {"nou! (F=1000000)", "osu! ()", "LANDY (NullReferenceException)"};

    ///<summary>負值表示結算成績"直接扣除"(最多扣至0)，正值表示"直接乘法" {見ScoreManager.CalcTrueScore)</summary>
    static public float[] modMultipler = {-0.5f, -0.5f};
    static public string[] noteRatings = {"Perfect!", "OK", "Bad", "Miss.."}; // name
    static public float[] noteScore = {1f, 0.7f, 0.3f, 0f}; // full==1
    static public float[] noteScoreOsu = { 1f, 0.3333333333f, 0.1666666666f, 0f }; // full==1
    static public int[] noteOffset = {68, 177, 274}; // ms

    
    [Header("Game Play")]
    public Mods[] mods;
    public double score = 0;
    /// <summary>
    /// current total score to calc percentage
    /// </summary>
    double totalScore = 0;
    ///<summary>
    /// 最終結算時之實得分數
    ///</summary> 
    public double trueScore = 0;
    /// <summary>
    /// 表示未受Mod影響之真實分數(F=1000000)。 
    /// </summary>
    public double unscaledScore = 0;
    public int maxCombo = 0;
    public int combo = 0;
    public int[] noteResult = new int[4]; 
    public float percentage;
    /// <summary>
    /// 每個note的基礎分數
    /// </summary>
    double scorePerCircle = 0;
    public float hp = 50, hpmax = 100;
    public float sp = 0, spmax = 100;

    private void Start() 
    {
        instance = this;
    }
    private void Update() 
    {
        if(totalScore == 0)// div!0
            percentage = 0;
        else
            percentage = (float)(unscaledScore/totalScore*100f);

        if(maxCombo < combo)
            maxCombo = combo;
        
        trueScore = CalcTrueScore(score, mods);
    }
    
    public void Init(List<GameObject> CircleList)
    {
        scorePerCircle = 1000000f / (float)CircleList.Count;    
    }

    ///<summary>
    ///  由 Circle 呼叫，增加分數
    ///</summary>
    public void GotCircle(int rating, Vector3 pos)
    {
        if(rating < 2)
        {
            combo ++;
        }
        else
        {
            combo = 0;
            score += 0;
        }
        noteResult[rating]++;
        double delta = scorePerCircle * noteScore[rating];
        unscaledScore += delta;
        
        switch(Userpref.data.calcScoreMod)
        {
            case 0://norm
            default:
                break;
            case 1://osu
                //Dx(N-2)x300A/25xM+300A
                int N = combo - 2;
                if (N < 0) N = 0;
                delta = playing.OverallDifficulty * N * noteScoreOsu[rating] * 1 * 12f + 300f * noteScoreOsu[rating];
                break;
            case 2:
                N = combo - 4;
                if (N < 0) N = 0;
                delta = Mathf.Pow(playing.OverallDifficulty * N * noteScoreOsu[rating] * 1 * 8.7f, 2.019f) + 1 * noteScoreOsu[rating];
                break;
        }
        score += delta;
        totalScore += scorePerCircle;

        Debug.Log($"Using {CalcMode[Userpref.data.calcScoreMod]} Mode: Add {delta} pt.");

        //製作Label
        var go = Instantiate(GameHandler.instance.noteResult[rating], GameHandler.WorldCanvas);
        go.transform.position = pos;
        Destroy(go, 7);
    }

    public bool HasMod(Mods mod)
    {
        foreach (var m in mods)
        {
            if(m == mod)
                return true;
        }
        //Debug.Log("Not "+mod);
        return false;
    }

    /// <summary>
    /// 計算最終結算實得分
    /// </summary>
    double CalcTrueScore(double originalScore, Mods[] mods)
    {
        double s = originalScore;
        foreach(var m in mods)
        {
            float mul = PlayStat.modMultipler[(int)m];
            if(mul < 0)
            {
                s -= originalScore*(-mul);
                if(s <= 0)
                    s = 0;
            }
            else
            {
                s *= mul;
            }
        }
        return s;
    }

}