using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RecordManager))]
[RequireComponent(typeof(AnimationController))]
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public RuntimeGizmos.TransformGizmo transformGizmo;

    [SerializeField] Text txtCurrentTime;
    [SerializeField] Slider sliderDragTime;
    [SerializeField] Toggle togRecord;
    [SerializeField] Toggle togPlay;
    [Space, SerializeField] Transform group;
    [SerializeField] GameObject prefabNameBtn;
    [SerializeField] int maxRecordingTime;
    [SerializeField] float currentTime;

    public Slider GetSlider { get { return sliderDragTime; } }
    public float CurrentTime { get { return currentTime; } set { currentTime = value; } }

    private bool isPlaying = false;
    private MySliderHandler sliderHandler;


    private void Awake()
    {
        instance = this;
        sliderHandler = sliderDragTime.GetComponent<MySliderHandler>();
    }

    private IEnumerator Start()
    {
        sliderDragTime.interactable = RecordManager.instance.GetRecordUnits.Count == 0 ? false : true;
        sliderDragTime.onValueChanged.AddListener(delegate { SliderValueChange(); });
        sliderHandler.SliderValue += EndDragSlider;
        togPlay.onValueChanged.AddListener(delegate { PlayAndPause(togPlay.isOn); });

        yield return null;
        AddItem(RecordManager.instance.GetObjectDataList);
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            sliderDragTime.value += Time.deltaTime / maxRecordingTime;
            sliderDragTime.value = Mathf.Clamp(sliderDragTime.value, 0, maxRecordingTime);
        }
    }

    private void OnApplicationQuit()
    {
        sliderHandler.SliderValue -= EndDragSlider;
    }


    private void EndDragSlider(float value)
    {
        if (RecordManager.instance.GetRecordingStatus)
            RecordManager.instance.AddTimestamp(currentTime);

        AnimationController.instance.Init(RecordManager.instance.GetObjectDataList);
    }

    private void SliderValueChange()
    {
        currentTime = maxRecordingTime * sliderDragTime.value;
        currentTime = Mathf.Round(currentTime * 10) / 10;
        txtCurrentTime.text = currentTime.ToString("0.0");

        AnimationController.instance.Loop(currentTime);
    }

    private void PlayAndPause(bool isOn)
    {
        isPlaying = isOn;
    }

    private void AddItem(List<ObjectHandler> _list)
    {
        foreach (var i in _list)
        {
            if (i.objBase.Name != RecordManager.instance.GetRootName)
            {
                GameObject go = Instantiate(prefabNameBtn);
                go.transform.GetComponent<NameBtn>().SetText(i.objBase.Name);
                go.transform.SetParent(group);
                go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }
}
