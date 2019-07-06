using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    /// <summary>
    /// 在game over面板會被替換成"REVIVE"
    /// </summary>
    [SerializeField] Button resumeButt;
    [SerializeField] Text resumeButtTab;
    [SerializeField] Text resumeCountDown;
    [SerializeField] Text titleTab, songInfoTab;
    Animator anim;

    float resumeCd;
    bool isDeadPanel;
    public bool isPaused { get { return gameObject.activeInHierarchy; } }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 開啟暫停面板，注意我們會把GameHandler.Music.pitch = 0;
    /// </summary>
    /// <param name="isDead">是否開啟GameOver面板</param>
    public void ShowPanel(bool isDead)
    {
        GameHandler.instance.Music.pitch = 0;
        gameObject.SetActive(true);
        isDeadPanel = isDead;

        if(isDead)
        {
            titleTab.text = "<color=red><b>YOU'VE ALREADY DEAD</b></color>";
            resumeButtTab.text = $"REVIVE\n<color=green>已復活\u00a0{PlayStat.instance.reviveCount}\u00a0次</color>";
        }
        else
        {

        }
        songInfoTab.text = PlayStat.instance.playing.ToString("ta") + "\n(" + PlayStat.instance.playing.ToString("cv") +")";

        anim.Play("SHOW");
    }

    public void OnResumeButtPressed()
    {
        
        if(isDeadPanel)
        {
            PlayStat.instance.hp = PlayStat.instance.usingChara.hp / 2f;
            PlayStat.instance.reviveCount++;
            Debug.Log($"Revive | ReviveCount={PlayStat.instance.reviveCount}");
        }
        else
        {
            Debug.Log("Resume.");
        }

        anim.Play("UNSHOW");
        resumeCd = 3;
        StartCoroutine(DoResume());
    }
    public void OnRetryButtPressed()
    {
        Debug.Log("Retry.");
    }
    public void OnQuitButtPressed()
    {
        SceneManager.LoadSceneAsync("Score");
        Debug.Log("Quit Game.");
    }

    IEnumerator DoResume()
    {
        while(resumeCd > 0)
        {
            resumeCd -= Time.deltaTime;
            if(resumeCd > 0)
                resumeCountDown.text = resumeCd.ToString("0.00");
            else
                resumeCountDown.text = "<color=cyan>START!</color>";
            yield return 0;
        }
        GameHandler.instance.Music.pitch = 1;
        gameObject.SetActive(false);
    }

}
