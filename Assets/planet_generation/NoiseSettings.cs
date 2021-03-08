using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        SIMPLE,
        RIGID
    }

    public string description = "Basic Filter.";
    public FilterType filtertype;

    [ConditionalHide("filtertype", 0)]
    public SimpleNoiseSettings simplesettings;
    [ConditionalHide("filtertype", 1)]
    public RigidNoiseSettings rigidsettings;


    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strength = 1.0f;
        [Range(1, 10)]
        public int layers = 1;
        public float baseroughness = 1.0f;
        public float roughness = 1.0f;
        public float persistence = 0.5f;
        public Vector3 center;
        public float minvalue = 0.5f;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float weightmultiplier = 0.8f;
    }
}
