// #define Modthod_1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationPlayer : MonoBehaviour
{
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

    public static AnimationPlayer instance;

    [SerializeField]
    private List<ObjProp> objProps = new List<ObjProp>();

    private ObjectHandler[] objs;

    private void Awake()
    {
        instance = this;
    }

    public void Init(List<ObjectHandler> _list)
    {
        objProps.Clear();
        objs = _list.ToArray();

        for (int i = 0; i < objs.Length; i++)
        {
            for (int j = 0; j < RecordManager.instance.recordUnits.Count; j++)
            {
                for (int k = 0; k < RecordManager.instance.recordUnits[j].objects.Count; k++)
                {
                    if (objs[i].name.Equals(RecordManager.instance.recordUnits[j].objects[k].Name))
                    {
                        var check = objProps.Exists(x => x.obj.name == objs[i].name);
                        if (check)
                        {
                            int index = objProps.FindIndex(x => x.obj.name == objs[i].name);
                            objProps[index].SetItem(RecordManager.instance.recordUnits[j].timeStamp, RecordManager.instance.recordUnits[j].objects[k].Position, RecordManager.instance.recordUnits[j].objects[k].RotationEuler, RecordManager.instance.recordUnits[j].objects[k].Scale);
                        }
                        else
                        {
                            objProps.Add(new ObjProp(objs[i].transform, RecordManager.instance.recordUnits[j].timeStamp, RecordManager.instance.recordUnits[j].objects[k].Position, RecordManager.instance.recordUnits[j].objects[k].RotationEuler, RecordManager.instance.recordUnits[j].objects[k].Scale));
                        }
                    }
                }
            }
        }
    }

#if Modthod_1
    private int FindItemIndex(int preIndex, string name, int timeIndex)
    {
        int findItemPreIndex = RecordManager.instance.recordUnits[preIndex].objects.FindIndex(a => a.Name == name);
        if (findItemPreIndex == -1)
        {
            for (int i = timeIndex - 1; i > 0; i--)
            {
                var tmp = RecordManager.instance.recordUnits[i].objects.FindIndex(a => a.Name == name);
                if (tmp != -1)
                {
                    // print("[Tmp] " + findItemPreIndex);
                    findItemPreIndex = tmp;
                }
            }
            // print("[Return] " + findItemPreIndex);
            return findItemPreIndex;
        }
        else
            return findItemPreIndex;
    }

    private int FindPreIndex(int timeIndex, string name)
    {
        int findpreindex = 0;
        for (int i = timeIndex - 1; i > 0; i--)
        {
            var tmp = RecordManager.instance.recordUnits[i].objects.FindIndex(a => a.Name == name);
            if (tmp != -1)
            {
                findpreindex = i;
                break;
            }
        }
        return findpreindex;
    }
#endif

    public void Loop(float _timestamp)
    {
        // if (RecordManager.instance.GetRecordingStatus)
        //     return;

#if Modthod_1
        // Method 1: 分段處裡
        int timeIndex = 0;
        for (int i = 0; i < RecordManager.instance.recordUnits.Count; i++)
        {
            if (_timestamp < RecordManager.instance.recordUnits[i].timeStamp)
            {
                timeIndex = i;
                break;
            }
        }

        for (int i = 0; i < objs.Length; i++)
        {
            if (timeIndex != 0)
            {
                for (int j = 0; j < RecordManager.instance.recordUnits[timeIndex].objects.Count; j++)
                {
                    if (RecordManager.instance.recordUnits[timeIndex].objects[j].Name == objs[i].name)
                    {
                        var preTimeIndex = timeIndex - 1;
                        if (preTimeIndex < 0)
                            preTimeIndex = 0;

                        int findPreIndex = FindPreIndex(timeIndex, objs[i].name);
                        float total = RecordManager.instance.recordUnits[timeIndex].timeStamp - RecordManager.instance.recordUnits[preTimeIndex].timeStamp;
                        float t = (_timestamp - RecordManager.instance.recordUnits[preTimeIndex].timeStamp) / total;
                        if (total == 0)
                            t = 1;
                        int findItemPreIndex = FindItemIndex(findPreIndex, objs[i].name, timeIndex);
                        // print($"{t.ToString("0.00")} {_timestamp} {RecordManager.instance.recordUnits[findPreIndex].timeStamp} {RecordManager.instance.recordUnits[timeIndex].timeStamp}");
                        // print($"{timeIndex} {findPreIndex} {i} {j} {findItemPreIndex}");
                        // print($"{RecordManager.instance.recordUnits[timeIndex].objects[j].Name}");
                        // print($"{RecordManager.instance.recordUnits[findPreIndex].objects[findItemPreIndex].Name}");
                        objs[i].transform.position = Vector3.Lerp(RecordManager.instance.recordUnits[findPreIndex].objects[findItemPreIndex].Position, RecordManager.instance.recordUnits[timeIndex].objects[j].Position, t);
                    }
                }
            }
        }
#endif


#if true
        // Method 2: 
        int[] testIndex = new int[objProps.Count];
        for (int i = 0; i < objProps.Count; i++)
        {
            for (int j = 0; j < objProps[i].locations.Count; j++)
            {
                if (_timestamp < objProps[i].locations[j].time)
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
                    if (prvIndex < 0)
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
#endif
    }

}
