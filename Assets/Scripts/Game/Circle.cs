using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    // Circle parameters
    private float PosX = 0;
    private float PosY = 0;
    private float PosZ = 0;
    
    public int PosA = 0;//SPAWN AT (ms)

    private Color MainColor, MainColor1, MainColor2; // Circle sprites color
    public GameObject MainApproach, MainFore, MainBack; // Circle objects

    [HideInInspector]
    public SpriteRenderer Fore, Back, Appr; // Circle sprites

    // Checker stuff
    private bool RemoveNow = false;
    private bool GotIt = false;

    private void Awake()
    {
        Fore = MainFore.GetComponent<SpriteRenderer>();
        Back = MainBack.GetComponent<SpriteRenderer>();
        Appr = MainApproach.GetComponent<SpriteRenderer>();
    }

    // Set circle configuration
    public void Set(float x, float y, float z, int a)
    {
        Debug.Log($"({x}, {y})");
        PosX = x;
        PosY = y;
        PosZ = z;
        PosA = a;
        MainColor = Appr.color;
        MainColor1 = Fore.color;
        MainColor2 = Back.color;
    }

    // Spawning the circle
    public void Spawn()
    {
        gameObject.transform.position = new Vector3(PosX, PosY, PosZ);
        this.enabled = true;
        StartCoroutine(Checker());
    }

    // If circle wasn't clicked
    public void Remove ()
    {
        if (!GotIt)
        {
            Debug.Log("Miss");
            PlayStat.instance.GotCircle(rating:3, transform.position);
            RemoveNow = true;
            this.enabled = true;
        }
    }

    ///<summary>
    /// 點擊到我了！
    ///</summary>
    public void Got ()
    {
        if (!RemoveNow)
        {
            int judgeTime = PosA + GameHandler.ApprRate;
            float timedelta = Mathf.Abs( (float)GameHandler.timer - judgeTime );
            int rating = 0;
            if(timedelta < PlayStat.noteOffset[0])
                rating = 0;
            else if(timedelta < PlayStat.noteOffset[1])
                rating = 1;
            else if(timedelta < PlayStat.noteOffset[2])
                rating = 2;
            else
                rating = 3;    
            Debug.Log($"{PlayStat.noteRatings[rating]}! 時間差={timedelta}");

            PlayStat.instance.GotCircle(rating, transform.position);
            GotIt = true;
            MainApproach.transform.position = new Vector2(-1001, -1001);
            GameHandler.pSounds.PlayOneShot(GameHandler.pHitSound);
            RemoveNow = false;
            this.enabled = true;
        }
    }

    // Check if circle wasn't clicked
    private IEnumerator Checker()
    {
        while (true)
        {
            // 75 means delay before removing
            if (GameHandler.timer >= PosA + (GameHandler.ApprRate + PlayStat.noteOffset[2]) && !GotIt)
            {
                Remove();
                break;
            }
            yield return null;
        }
    }

    // Main Update
    private void Update ()
    {
        // Approach Circle modifier
        if (MainApproach.transform.localScale.x >= 0.9f)
        {
            MainApproach.transform.localScale -= new Vector3(5.166667f, 5.166667f, 0f) * Time.deltaTime;
            MainColor.a += 4f * Time.deltaTime;
            MainColor1.a += 4f * Time.deltaTime;
            MainColor2.a += 4f * Time.deltaTime;
            Fore.color = MainColor1;
            Back.color = MainColor2;
            Appr.color = MainColor;

        }
        // If circle wasn't clicked
        else if (!GotIt)
        {
            // Remove circle
            if (!RemoveNow)
            {
                MainApproach.transform.position = new Vector2(-101, -101);
                this.enabled = false;
            }
            // If circle wasn't clicked
            else
            {
                MainColor1.a -= 10f * Time.deltaTime;
                MainColor2.a -= 10f * Time.deltaTime;
                MainFore.transform.localPosition += (Vector3.down * 2) * Time.deltaTime;
                MainBack.transform.localPosition += Vector3.down * Time.deltaTime;
                Fore.color = MainColor1;
                Back.color = MainColor2;
                if (MainColor1.a <= 0f)
                {
                    gameObject.transform.position = new Vector2(-101, -101);
                    this.enabled = false;
                }
            }
        }
        // If circle was clicked
        if (GotIt)
        {
            MainColor1.a -= 10f * Time.deltaTime;
            MainColor2.a -= 10f * Time.deltaTime;
            MainFore.transform.localScale += new Vector3(2, 2, 0) * Time.deltaTime;
            MainBack.transform.localScale += new Vector3(2, 2, 0) * Time.deltaTime;
            Fore.color = MainColor1;
            Back.color = MainColor2;
            if (MainColor1.a <= 0f)
            {
                gameObject.transform.position = new Vector2(-101, -101);
                this.enabled = false;
            }
        }
    }
}