using System.Collections.Generic;
using UnityEngine;

public class PlayStat : MonoBehaviour 
{
    static public PlayStat instance;

    public OsuFile playing = null; // playing 
    static public string[] ModString = {"Groove Coaster", "Auto Click"}; 
    public enum Mods {AutoMove, AutoClick}
    public Mods[] mods;
    public double score = 0;
    public double totalScore = 0; // current total score to calc percentage
    public int maxCombo = 0;
    public int combo = 0;
    static public string[] noteRatings = {"Perfect!", "OK", "Bad", "Miss.."}; // name
    static public float[] noteScore = {1f, 0.7f, 0.3f, 0f}; // full==1
    static public int[] noteOffset = {68, 177, 274}; // ms
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
    }
    
    public void Init(List<GameObject> CircleList)
    {
        scorePerCircle = 1000000f / (float)CircleList.Count;    
    }

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
}