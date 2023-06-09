using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AnimationPlayer : MonoBehaviour
{
    public static AnimationPlayer instance;

    private ObjectHandler[] objs;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    public void Init(List<ObjectHandler> _list)
    {
        objs = _list.ToArray();
    }


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

    public void Loop(float _timestamp)
    {
        if (RecordManager.instance.GetRecordingStatus)
            return;

        int timeIndex = 0;
        for (int i = 0; i < RecordManager.instance.recordUnits.Count; i++)
        {
            if (_timestamp <= RecordManager.instance.recordUnits[i].timeStamp)
            {
                timeIndex = i;
                break;
            }
        }


        for (int i = 0; i < objs.Length; i++)
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

}
