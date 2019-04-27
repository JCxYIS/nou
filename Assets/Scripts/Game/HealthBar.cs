using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Text text;
    float targetValue = -1;

    void Update()
    {
        GetComponent<Slider>().value = Mathf.Lerp(GetComponent<Slider>().value, targetValue, Time.deltaTime * 1f);
    }

    public void Set(float value, float maxvalue)
    {
        if (targetValue == -1)
            GetComponent<Slider>().value = value / maxvalue;
        targetValue = value / maxvalue;
        text.text = $"{value}<color=#d2d2d2>/{maxvalue}</color>";
    }
}
