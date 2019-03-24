using System.Collections.Generic;
using UnityEngine;

public class PlayStat : MonoBehaviour 
{
    static public PlayStat instance;

    public OsuFile playing = null; // playing 
    public float score = 0;
    public float totalScore = 0; // current total score to calc percentage
    public int combo = 0;
    public int notePerfect = 0;
    public int noteOk = 0;
    public int noteBad = 0;
    public int noteMiss = 0;

    public float percentage;
    float scorePerCircle = 0;

    private void Start() 
    {
        instance = this;
    }
    private void Update() 
    {
        percentage = score/totalScore*100f;
    }
    
    public void Init(List<GameObject> CircleList)
    {
        scorePerCircle = 1000000f / (float)CircleList.Count;    
    }

    public void GotCircle(bool isGet)
    {
        if(isGet)
        {
            combo ++;
            notePerfect++;
            score += scorePerCircle;
        }
        else
        {
            combo = 0;
            score += 0;
            noteMiss++;
        }
        totalScore += scorePerCircle;
    }
}