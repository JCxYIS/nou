using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    static int value = 0;
    static int nextbig = 25;//big jump 25x
    static bool shouldJump = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update() 
    {
        GetComponent<Text>().text = string.Format("{0:F0}", value); 
        if(shouldJump)
        {
            GetComponent<Animator>().Play("Null");
            GetComponent<Animator>().Play("Jump");
            //Debug.Log("play");
            shouldJump = false;
        }    
        if(value >= nextbig)
        {
            var g = Instantiate(gameObject, transform.parent);
            g.GetComponent<Animator>().Play("JumpBig");
            Destroy( g.GetComponent<Combo>() );
            Destroy(g, 5);
            while(nextbig <= value)
                nextbig += 25;
        }
    }

    static public void Set(int curCombo)
    {
        if(value != curCombo)
        {
            shouldJump = true;
        }
        value = curCombo;
        
    }
}
