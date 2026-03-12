using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
using System;

public class SliderAmountText : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private UnityEngine.UI.Text _text;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        _text.text = ((int)_slider.value).ToString();
    }

}
