using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RandomPlanetSettings : ScriptableObject
{
    [Header("Basic Settings")]
    public RandomInt resolution;
    public RandomInt planetradius;

    [Header("Noise Settings")]
    public RandomInt noiselayers;
    public RandomSimpleNoiseSettings terrainnoisesettings;

    [Header("Biome Settings")]
    public RandomColor planetoceandepththeme;
    public RandomColor planetoceansurfacetheme;
    public RandomColor planetgroundtheme;
    public RandomColor planetclifftheme;
    public RandomColor planetclifftoptheme;
    public RandomInt biomescount;
    public RandomSimpleNoiseSettings biomenoisesettings;
    public RandomFloat biometintpct;
    public RandomFloat biomenoiseoffset;
    public RandomFloat biomenoisestrength;
    public RandomFloat biomeblendamount;

    [Header("Water Settings")]
    public RandomFloat watersmoothness;
    public RandomFloat waterbumpstrength;
    public RandomVector2 waterscale;
    public RandomVector2 waterscroll;
}
