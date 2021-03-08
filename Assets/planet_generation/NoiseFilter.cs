using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{ 
    NoiseSettings.SimpleNoiseSettings settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noisevalue = 0;
        float frequency = settings.baseroughness;
        float amplitude = 1;

        for (int i = 0; i < settings.layers; ++i)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            noisevalue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noisevalue = noisevalue - settings.minvalue;

        return noisevalue * settings.strength;
    }
}
