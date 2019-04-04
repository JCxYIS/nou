using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public Text Score, Percent, NoteStat, MaxCombo;
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

        Score.text = string.Format("{0:F0}", ps.score);
        Percent.text =  string.Format("{0:F2}%", ps.percentage );   
        NoteStat.text = string.Format(
            "<color=green>{0:N0}</color>\n<color=#FFB000>{1:N0}</color>\n<color=#8277FF>{2:N0}</color>\n<color=RED>{3:N0}</color>\n<color=BLACK>{4:N0}</color>",
            ps.noteResult[0], ps.noteResult[1], ps.noteResult[2], ps.noteResult[3], ps.maxCombo); 
        //MaxCombo.text = string.Format("{0:F0}", ps.score);
 
    }
    

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
        Destroy(ps.gameObject);
    }
}
