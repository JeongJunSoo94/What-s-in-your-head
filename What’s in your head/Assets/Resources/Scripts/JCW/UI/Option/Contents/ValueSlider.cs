using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSlider : MonoBehaviour
{
    private int _value = 0;
    private Slider slider = null;
    private Text valueText = null;

    private void Awake()
    {
        slider = this.gameObject.GetComponent<Slider>();        
        //slider.value = _value / 100.0f;
        valueText = transform.GetChild(0).gameObject.GetComponent<Text>();        
    }

    void Update()
    {
        _value = (int)(slider.value*100);
        valueText.text = _value.ToString();
    }
}
