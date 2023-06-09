using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;


    [SerializeField] Text txtCurrentTime;
    [SerializeField] Slider sliderDragTime;
    [SerializeField] Toggle togRecord;
    [SerializeField] int maxRecordingTime;
    [SerializeField] float currentTime;

    public float GetCurrentTime { get { return currentTime; } }
    public float GetMaxRecordTime { get { return maxRecordingTime; } }


    private MySliderHandler sliderHandler;


    void Awake()
    {
        instance = this;
        sliderHandler = sliderDragTime.GetComponent<MySliderHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sliderDragTime.onValueChanged.AddListener(delegate { SliderValueChange(); });
        sliderHandler.SliderValue += EndDragSlider;
    }

    void OnApplicationQuit()
    {
        sliderHandler.SliderValue -= EndDragSlider;
    }

    void EndDragSlider(float value)
    {
        if (RecordManager.instance.GetRecordingStatus)
            RecordManager.instance.AddTimestamp(currentTime);
    }

    void SliderValueChange()
    {
        currentTime = maxRecordingTime * sliderDragTime.value;
        currentTime = Mathf.Round(currentTime * 10) / 10;
        txtCurrentTime.text = currentTime.ToString("0.0");

        AnimationPlayer.instance.Loop(currentTime);
    }

    public void EnableSlider()
    {
        sliderDragTime.interactable = true;
    }
}
