using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(AnimationController))]
public class RecordManager : MonoBehaviour
{
    public static RecordManager instance;

    public Transform target;

    public bool GetRecordingStatus { get { return isRecording; } }
    public List<ObjectHandler> GetObjectDataList { get { return objectDataList; } }
    public List<RecordUnit> GetRecordUnits { get { return recordUnits; } }
    public string GetRootName { get { return rootName; } }

    [SerializeField] List<RecordUnit> recordUnits = new List<RecordUnit>();
    List<ObjectHandler> objectDataList = new List<ObjectHandler>();
    ExportJson exportjson = new ExportJson();
    bool isRecording = false;
    string rootName = string.Empty;

    public class ExportJson
    {
        public List<RecordUnit> ObjectList;
    }

    [Serializable]
    public class RecordUnit
    {
        public RecordUnit(float _timeStamp)
        {
            timeStamp = _timeStamp;
        }
        public float timeStamp;
        public List<ObjectTransfromBase> objects = new List<ObjectTransfromBase>();
    }


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        InitRoot();
    }
    private void OnApplicationQuit()
    {
        foreach (var i in objectDataList)
            i.transformHandler -= TransFormUpdate;
    }


    private void TransFormUpdate(ObjectTransfromBase _obj)
    {
        if (!isRecording)
            return;

        foreach (var i in recordUnits)
        {
            if (i.timeStamp.Equals(UIManager.instance.CurrentTime))
            {
                var target = i.objects.Find(x => x.Name == _obj.Name);
                if (target != null)
                {
                    target.Position = _obj.Position;
                    target.RotationEuler = _obj.RotationEuler;
                    target.Scale = _obj.Scale;
                }
                else
                {
                    i.objects.Add(new ObjectTransfromBase(_obj.Name, _obj.Bom, _obj.Position, _obj.RotationEuler, _obj.Scale));
                }
            }
        }
    }


    public void InitRoot()
    {
        Transform[] allChildren = target.GetComponentsInChildren<Transform>();
        int count = 0;
        foreach (Transform child in allChildren)
        {
            if (count == 0)
                rootName = child.name;
            else
                child.gameObject.AddComponent<BoxCollider>();

            var objHandler = child.gameObject.AddComponent<ObjectHandler>();
            objectDataList.Add(objHandler);
            objHandler.transformHandler += TransFormUpdate;
            count++;
        }
        AnimationController.instance.Init(objectDataList);
    }

    public void AddTimestamp(float _timestamp)
    {
        if (recordUnits.Count.Equals(0))
        {
            recordUnits.Add(new RecordUnit(0));
            foreach (var i in objectDataList)
                recordUnits[0].objects.Add(new ObjectTransfromBase(i.objBase.Name, i.objBase.Bom, i.objBase.Position, i.objBase.RotationEuler, i.objBase.Scale));
        }
        else
        {
            var target = recordUnits.Find(x => x.timeStamp == _timestamp);
            if (target == null)
                recordUnits.Add(new RecordUnit(_timestamp));
        }
    }

    [ContextMenu("StartRecording")]
    public void StartRecording()
    {
        isRecording = true;
        AddTimestamp(UIManager.instance.CurrentTime);
    }

    [ContextMenu("EndRecording")]
    public void EndRecording()
    {
        isRecording = false;
        List<RecordUnit> removeList = new List<RecordUnit>();
        foreach (var i in recordUnits)
            if (i.objects.Count.Equals(0))
                removeList.Add(i);

        foreach (var i in removeList)
            recordUnits.Remove(i);

        recordUnits = recordUnits.OrderBy(x => x.timeStamp).ToList();
        AnimationController.instance.Init(objectDataList);
    }

    [ContextMenu("ExportDataList")]
    public void ExportDataList()
    {
        exportjson.ObjectList = recordUnits;
        String json = JsonUtility.ToJson(exportjson);
        Debug.Log(json);
    }
}