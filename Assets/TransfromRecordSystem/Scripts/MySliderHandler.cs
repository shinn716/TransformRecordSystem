using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Slider))]
public class MySliderHandler : MonoBehaviour, IPointerUpHandler
{
    public event Action<float> SliderValue;

    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SliderValue?.Invoke(slider.value);
    }
}