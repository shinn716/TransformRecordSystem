using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectTransfromBase
{
    public ObjectTransfromBase(string _name, string _bom, Vector3 _pos, Vector3 _rot, Vector3 _scl)
    {
        Name = _name;
        Bom = _bom;
        Position = _pos;
        RotationEuler = _rot;
        Scale = _scl;
    }

    public string Name;
    public string Bom;
    public Vector3 Position;
    public Vector3 RotationEuler;
    public Vector3 Scale;
}
