using System;
using UnityEngine;

// -------------------------------------------
//  int => float
// -------------------------------------------
[Serializable]
public class DictStringInt : SerializableDict<string, int> { }

// -------------------------------------------
//  GameObject => float
// -------------------------------------------
[Serializable]
public class DictGameObjectFloat : SerializableDict<GameObject, float> { }