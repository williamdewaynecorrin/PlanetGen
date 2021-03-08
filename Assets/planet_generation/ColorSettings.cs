using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public enum PlanetColorType
    {
        HEIGHT_GRADIENT,
        BIOMES
    }

    [System.Serializable]
    public class BiomeColorSettings
    {
        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0,1)]
            public float startheight;
            [Range(0,1)]
            public float tintpercent;
        }

        public Biome[] biomes;
        public NoiseSettings noise;
        public float noiseoffset;
        public float noisestrength;
        [Range(0,1)]
        public float blendamount;
        public Gradient oceancolor;
    }

    public Material planetmaterial;
    public PlanetColorType planettype;

    [ConditionalHide("planettype", 0)]
    public Gradient planetgradient;
    [ConditionalHide("planettype", 1)]
    public BiomeColorSettings biomesettings;

}
