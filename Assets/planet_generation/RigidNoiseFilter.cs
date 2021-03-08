using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    NoiseSettings.RigidNoiseSettings settings;
    Noise noise = new Noise();

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noisevalue = 0;
        float frequency = settings.baseroughness;
        float amplitude = 1;
        float weight = 1.0f;

        for (int i = 0; i < settings.layers; ++i)
        {
            // -- make rigid noise using 1 - abs
            float v = 1 - Mathf.Abs( noise.Evaluate(point * frequency + settings.center));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightmultiplier);

            noisevalue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noisevalue = noisevalue - settings.minvalue;

        return noisevalue * settings.strength;
    }
}
