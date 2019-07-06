using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public Number Score, Percent;
    public Number[] NoteStat;//gay,ok,bed,succ,MaxCB
    public Number TrueScore;
    public Number Revive;
    PlayStat ps = null;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            ps = GameObject.Find("PlayStat").GetComponent<PlayStat>();
        }
        catch (System.Exception e)
        {
            Debug.LogError("No PlayStat Fetched: "+e);
            GoMenu();
        }
        if(ps == null)
        {
            Debug.LogError("No PlayStat Found");
            GoMenu();
        }

        Score.Set((float)ps.score);
        Percent.Set(ps.percentage ); 
        for(int i = 0; i <= 3; i++)  
            NoteStat[i].Set(ps.noteResult[i]);
        NoteStat[4].Set(ps.maxCombo);
        Revive.Set(ps.reviveCount);
        TrueScore.Set((float)ps.trueScore);
        //MaxCombo.text = string.Format("{0:F0}", ps.score);
    }
    

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
        Destroy(ps.gameObject);
    }
}
