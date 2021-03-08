using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetradius = 1.0f;
    public NoiseLayer[] noiselayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool usefirstlayerasmask = false;
        public NoiseSettings settings;
    }
}
