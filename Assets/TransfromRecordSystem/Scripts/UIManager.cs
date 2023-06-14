using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public RuntimeGizmos.TransformGizmo transformGizmo;

    [SerializeField] Text txtCurrentTime;
    [SerializeField] Slider sliderDragTime;
    [SerializeField] Toggle togRecord;
    [SerializeField] Transform group;
    [SerializeField] GameObject prefabNameBtn;
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
        sliderDragTime.interactable = RecordManager.instance.recordUnits.Count == 0 ? false : true;
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

        AnimationPlayer.instance.Init(RecordManager.instance.GetObjectDataList);
    }

    void SliderValueChange()
    {
        currentTime = maxRecordingTime * sliderDragTime.value;
        currentTime = Mathf.Round(currentTime * 10) / 10;
        txtCurrentTime.text = currentTime.ToString("0.0");

        AnimationPlayer.instance.Loop(currentTime);
    }

    public void EndRecord()
    {
        AnimationPlayer.instance.Init(RecordManager.instance.GetObjectDataList);
    }

    public void AddItem(List<ObjectHandler> _list)
    {
        foreach (var i in _list)
        {
            if (i.objBase.Name != "root")
            {
                GameObject go = Instantiate(prefabNameBtn);
                go.transform.GetComponent<NameBtn>().SetText(i.objBase.Name);
                go.transform.SetParent(group);
                go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }
}
