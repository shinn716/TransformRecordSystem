using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(RecordManager))]
public class AnimationController : MonoBehaviour
{
    public static AnimationController instance;

    // [SerializeField]
    private List<ObjProp> objProps = new List<ObjProp>();

    private float totalRecordTime = 0;

    [Serializable]
    public class ObjProp
    {
        [Serializable]
        public class Location
        {
            public Location(float _time, Vector3 _pos, Vector3 _rot, Vector3 _scl)
            {
                time = _time;
                position = _pos;
                rotationEulur = _rot;
                scale = _scl;
            }
            public float time;
            public Vector3 position;
            public Vector3 rotationEulur;
            public Vector3 scale;
        }

        public ObjProp(Transform _obj, float _time, Vector3 _pos, Vector3 _rot, Vector3 _scl)
        {
            obj = _obj;
            locations.Add(new Location(_time, _pos, _rot, _scl));
        }

        public Transform obj;
        public List<Location> locations = new List<Location>();

        public void SetItem(float _time, Vector3 _pos, Vector3 _rot, Vector3 _scl)
        {
            locations.Add(new Location(_time, _pos, _rot, _scl));
        }
    }


    private void Awake()
    {
        instance = this;
    }

    public void Init(List<ObjectHandler> _list)
    {
        objProps.Clear();
        var objs = _list.ToArray();

        for (int i = 0; i < objs.Length; i++)
        {
            for (int j = 0; j < RecordManager.instance.GetAndSetRecordUnits.Count; j++)
            {
                for (int k = 0; k < RecordManager.instance.GetAndSetRecordUnits[j].objects.Count; k++)
                {
                    if (objs[i].name.Equals(RecordManager.instance.GetAndSetRecordUnits[j].objects[k].Name))
                    {
                        var check = objProps.Exists(x => x.obj.name == objs[i].name);
                        if (check)
                        {
                            int index = objProps.FindIndex(x => x.obj.name == objs[i].name);
                            objProps[index].SetItem(RecordManager.instance.GetAndSetRecordUnits[j].timeStamp, RecordManager.instance.GetAndSetRecordUnits[j].objects[k].Position, RecordManager.instance.GetAndSetRecordUnits[j].objects[k].RotationEuler, RecordManager.instance.GetAndSetRecordUnits[j].objects[k].Scale);
                        }
                        else
                        {
                            objProps.Add(new ObjProp(objs[i].transform, RecordManager.instance.GetAndSetRecordUnits[j].timeStamp, RecordManager.instance.GetAndSetRecordUnits[j].objects[k].Position, RecordManager.instance.GetAndSetRecordUnits[j].objects[k].RotationEuler, RecordManager.instance.GetAndSetRecordUnits[j].objects[k].Scale));
                        }
                    }
                }

                if (j.Equals(RecordManager.instance.GetAndSetRecordUnits.Count - 1))
                    totalRecordTime = RecordManager.instance.GetAndSetRecordUnits[j].timeStamp;
            }
        }
    }

    public void Loop(float _timestamp)
    {
        int[] testIndex = new int[objProps.Count];
        for (int i = 0; i < objProps.Count; i++)
        {
            for (int j = 0; j < objProps[i].locations.Count; j++)
            {
                if (_timestamp <= objProps[i].locations[j].time)
                {
                    testIndex[i] = j;
                    break;
                }
            }
        }

        for (int i = 0; i < objProps.Count; i++)
        {
            for (int j = 0; j < objProps[i].locations.Count; j++)
            {
                if (testIndex[i] != 0)
                {
                    var prvIndex = testIndex[i] - 1;
                    if (prvIndex <= 0)
                        prvIndex = 0;

                    float time = (_timestamp - objProps[i].locations[prvIndex].time) / (objProps[i].locations[testIndex[i]].time - objProps[i].locations[prvIndex].time);
                    if (objProps[i].locations[testIndex[i]].time == 0)
                        time = 1;
                    // print(objProps[i].obj.name + " " + _timestamp + " " + testIndex[i] + " " + prvIndex + " " + objProps[i].locations[testIndex[i]].time + " " + objProps[i].locations[prvIndex].time);
                    objProps[i].obj.position = Vector3.Lerp(objProps[i].locations[prvIndex].position, objProps[i].locations[testIndex[i]].position, time);
                    objProps[i].obj.rotation = Quaternion.Lerp(Quaternion.Euler(objProps[i].locations[prvIndex].rotationEulur), Quaternion.Euler(objProps[i].locations[testIndex[i]].rotationEulur), time);
                    objProps[i].obj.localScale = Vector3.Lerp(objProps[i].locations[prvIndex].scale, objProps[i].locations[testIndex[i]].scale, time);
                }
            }
        }
    }

    public float GetTotalTime()
    {
        return totalRecordTime;
    }
}
