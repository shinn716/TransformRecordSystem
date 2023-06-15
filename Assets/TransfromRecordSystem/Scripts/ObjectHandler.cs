using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectHandler : MonoBehaviour
{
    [HideInInspector]
    public ObjectTransfromBase objBase;
    public event Action<ObjectTransfromBase> transformHandler;

    private void Start()
    {
        objBase = new ObjectTransfromBase(name, "", transform.position, transform.rotation.eulerAngles, transform.localScale);
        objBase.Name = name;
        objBase.Position = transform.position;
        objBase.RotationEuler = transform.rotation.eulerAngles;
        objBase.Scale = transform.localScale;
    }

    private void Update()
    {
        if (!RecordManager.instance.GetRecordingStatus)
            return;

        if (transform.hasChanged)
        {
            objBase.Position = transform.position;
            objBase.RotationEuler = transform.rotation.eulerAngles;
            objBase.Scale = transform.localScale;
            transformHandler.Invoke(objBase);
            transform.hasChanged = false;
        }
    }
}