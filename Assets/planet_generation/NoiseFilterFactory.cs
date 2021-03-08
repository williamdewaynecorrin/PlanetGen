using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        switch(settings.filtertype)
        {
            case NoiseSettings.FilterType.SIMPLE:
                return new SimpleNoiseFilter(settings.simplesettings);
            case NoiseSettings.FilterType.RIGID:
                return new RigidNoiseFilter(settings.rigidsettings);
        }

        return null;
    }
}
