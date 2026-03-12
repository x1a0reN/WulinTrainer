using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Reflection;
using System;

public class ToggleSwitch : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private bool _isOn = false;
    public bool isOn
    {
        get
        {
            return _isOn;
        }

    }
    [SerializeField]
    private RectTransform toggleIndicator;
    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private Color onColor;
    [SerializeField]
    private Color offColor;
    private float offX;
    private float onX;
    [SerializeField]
    private float tweenTime = 0.25f;

    public delegate void ValueChanged(bool value);
    public event ValueChanged valueChanged;

    // Start is called before the first frame update
    void Start()
    {
        offX = toggleIndicator.anchoredPosition.x;
        onX = backgroundImage.rectTransform.rect.width - toggleIndicator.rect.width * 2 - 4 ;
    }

    private void OnEnable()
    {
        Toggle(isOn);
    }

    private void Toggle(bool value, bool playSFX = true)
    {

        if (value != isOn)
        {
            _isOn = value;

            ToggleColor(isOn);
            MoveIndicator(isOn);

            if (valueChanged != null)
            {
                valueChanged(isOn);
            }

        }
    }

    private void ToggleColor(bool value)
    {
        Color onColor = new Color(36f, 38f, 43f);
        Color offColor = new Color(44f, 84f, 83f);
        if (value)
        {
            backgroundImage.DOColor(Color.cyan, 0.25f);
        }
        else
        {
            backgroundImage.DOColor(Color.gray, 0.25f);
        }
    }

    private void MoveIndicator(bool value)
    {

        if (value)
        {
            toggleIndicator.DOAnchorPosX(onX, tweenTime);
        }
        else
        {
            toggleIndicator.DOAnchorPosX(offX, tweenTime);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {

        Toggle(!isOn);
    }


}
