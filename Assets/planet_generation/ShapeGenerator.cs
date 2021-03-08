using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] filters;
    public MinMax elevationminmax;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        filters = new INoiseFilter[settings.noiselayers.Length];
        for (int i = 0; i < filters.Length; ++i)
        {
           filters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiselayers[i].settings);
        }

        elevationminmax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 unitspherepoint)
    {
        float firstlayervalue = 0;
        float elevation = 0;

        if (filters.Length > 0)
        {
            firstlayervalue = filters[0].Evaluate(unitspherepoint);
            if (settings.noiselayers[0].enabled)
                elevation = firstlayervalue;
        }

        for (int i = 1; i < filters.Length; ++i)
        {
            if (settings.noiselayers[i].enabled)
            {
                float mask = (settings.noiselayers[i].usefirstlayerasmask) ? firstlayervalue : 1;
                elevation += filters[i].Evaluate(unitspherepoint) * mask;
            }
        }

        elevationminmax.AddValue(elevation); // keep track of min/max elevation
        return elevation;
    }

    public float GetScaledElevation(float unscaledelevation)
    {
        float elevation = Mathf.Max(0, unscaledelevation);
        elevation = settings.planetradius * (1 + elevation);
        return elevation;
    }
}
