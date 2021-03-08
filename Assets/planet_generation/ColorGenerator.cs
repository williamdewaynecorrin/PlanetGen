using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureresolution = 50;
    INoiseFilter biomenoisefilter;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if(texture == null || texture.height != settings.biomesettings.biomes.Length)
            texture = new Texture2D(textureresolution * 2, settings.biomesettings.biomes.Length, TextureFormat.RGBA32, false);
        biomenoisefilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomesettings.noise);
    }

    public void UpdateElevation(MinMax elevation)
    {
        settings.planetmaterial.SetVector("_ElevationMinMax", new Vector2(elevation.Min, elevation.Max));
    }

    public float BiomePercentFromPoint(Vector3 unitspherepoint)
    {
        float heightpct = (unitspherepoint.y + 1) / 2f;
        heightpct += (biomenoisefilter.Evaluate(unitspherepoint) - settings.biomesettings.noiseoffset) * settings.biomesettings.noisestrength;
        float biomeindex = 0;
        int numbiomes = settings.biomesettings.biomes.Length;
        float blendrange = settings.biomesettings.blendamount / 2.0f + Mathf.Epsilon;

        for (int i = 0; i < numbiomes; ++i)
        {
            float dist = heightpct - settings.biomesettings.biomes[i].startheight;
            float weight = Mathf.InverseLerp(-blendrange, blendrange, dist);
            biomeindex *= (1 - weight);
            biomeindex += i * weight;
        }

        return biomeindex / Mathf.Max(1, numbiomes - 1);
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];

        int colorindex = 0;
        foreach (var biome in settings.biomesettings.biomes)
        {
            for (int i = 0; i < textureresolution * 2; ++i)
            {
                Color gradientcolor;
                if (i < textureresolution) // sample ocean
                    gradientcolor = settings.biomesettings.oceancolor.Evaluate(i / (textureresolution - 1f));
                else // sample biome
                    gradientcolor = biome.gradient.Evaluate((i - textureresolution) / (textureresolution - 1f));

                Color tintcolor = biome.tint;
                colors[colorindex++] = gradientcolor * (1 - biome.tintpercent) + tintcolor * biome.tintpercent;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        settings.planetmaterial.SetTexture("_MainTex", texture);
    }
}
