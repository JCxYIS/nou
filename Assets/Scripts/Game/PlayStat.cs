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
    public enum Mods {AutoMove, AutoClick, ComboScore}

    ///<summary>
    /// 計分方式
    ///</summary> 
    public enum CalcMode {Normal, SupraCombo}

    ///<summary>負值表示結算成績"直接扣除"(最多扣至0)，正值表示"直接乘法" {見ScoreManager.CalcTrueScore)</summary>
    static public float[] modMultipler = {-0.5f, -0.5f};
    static public string[] noteRatings = {"Perfect!", "OK", "Bad", "Miss.."}; // name
    static public float[] noteScore = {1f, 0.7f, 0.3f, 0f}; // full==1
    static public int[] noteOffset = {68, 177, 274}; // ms

    
    [Header("Game Play")]
    public Mods[] mods;
    public double score = 0;
    public double totalScore = 0; // current total score to calc percentage
    ///<summary>
    /// 最終結算時的實得分數
    ///</summary> 
    public double trueScore = 0;
    public int maxCombo = 0;
    public int combo = 0;
    public int[] noteResult = new int[4]; 
    public float percentage;
    double scorePerCircle = 0;

    private void Start() 
    {
        instance = this;
    }
    private void Update() 
    {
        if(totalScore == 0)// div!0
            percentage = 0;
        else
            percentage = (float)(score/totalScore*100f);

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
        score += scorePerCircle * noteScore[rating];
        totalScore += scorePerCircle;

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
        Debug.Log("Not "+mod);
        return false;
    }

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