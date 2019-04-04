using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    [Tooltip("請見String.Format能用的格式如F0 / N2之類的")]
    public string DisplayStyle = "F0";
    public float targetValue;
    [Tooltip("數值變動速率")]
    public AnimationCurve curve;
    private float baseValue;
    private float completePercent;
    private float currentValue;
    [Tooltip("要花多少秒 從0%到100%")]
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        completePercent += Time.deltaTime / duration;  
        float deltaPercent =  curve.Evaluate(completePercent);
        currentValue = baseValue + (targetValue - baseValue)*deltaPercent;
        GetComponent<Text>().text = string.Format("{0:"+DisplayStyle+"}", currentValue); 
    }

    public void Set(float value)
    {
        if(value != targetValue)
        {
            baseValue = currentValue;
            targetValue = value;
            completePercent = 0;
        }
    }
}
