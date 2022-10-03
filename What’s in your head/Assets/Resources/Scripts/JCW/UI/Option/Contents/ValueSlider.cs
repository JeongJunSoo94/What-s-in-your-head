using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSlider : MonoBehaviour
{
    [Header("½ÃÀÛ °ª")] [SerializeField] [Range(0, 100)] private int _value = 50;
    private GameObject handle = null;
    private Text valueText = null;
    RectTransform handlePos;

    private void Awake()
    {
        this.gameObject.GetComponent<Slider>().value = _value / 100.0f;
        valueText = transform.GetChild(0).gameObject.GetComponent<Text>();
        handle = transform.GetChild(2).gameObject.transform.GetChild(0).gameObject;
        handlePos = handle.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        handlePos.anchorMin = new Vector2(_value / 100.0f, 0.0f);
        handlePos.anchorMax = new Vector2(_value / 100.0f, 1.0f);
    }

    void Update()
    {
        _value = (int)(handlePos.anchorMin.x * 100);
        valueText.text = _value.ToString();
    }
}
