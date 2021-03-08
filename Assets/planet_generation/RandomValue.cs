using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomValue<T>
{
    [SerializeField]
    protected T min;
    [SerializeField]
    protected T max;
    [HideInInspector]
    public T lastpicked;

    public RandomValue(T min, T max)
    {
        this.min = min;
        this.max = max;
    }

    public virtual T PickRandomValue()
    {
        return min;
    }
}

[System.Serializable]
public class RandomFloat : RandomValue<float>
{
    public RandomFloat(float min, float max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override float PickRandomValue()
    {
        return lastpicked = Random.Range(min, max);
    }
}

[System.Serializable]
public class RandomVector2 : RandomValue<Vector2>
{
    public RandomVector2(Vector2 min, Vector2 max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override Vector2 PickRandomValue()
    {
        return lastpicked = RandomXT.RandomVector2(min, max);
    }
}

[System.Serializable]
public class RandomInt : RandomValue<int>
{
    public RandomInt(int min, int max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override int PickRandomValue()
    {
        return lastpicked = Random.Range(min, max + 1);
    }
}

[System.Serializable]
public class RandomColor : RandomValue<Color>
{
    public RandomColor(Color min, Color max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override Color PickRandomValue()
    {
        float r = Random.Range(min.r, max.r);
        float g = Random.Range(min.g, max.g);
        float b = Random.Range(min.b, max.b);
        float a = Random.Range(min.a, max.a);

        return lastpicked = new Color(r, g, b, a);
    }
}

[System.Serializable]
public class RandomSimpleNoiseSettings : RandomValue<NoiseSettings.SimpleNoiseSettings>
{
    public RandomSimpleNoiseSettings(NoiseSettings.SimpleNoiseSettings min, NoiseSettings.SimpleNoiseSettings max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override NoiseSettings.SimpleNoiseSettings PickRandomValue()
    {
        NoiseSettings.SimpleNoiseSettings settings = new NoiseSettings.SimpleNoiseSettings();
        settings.baseroughness = Random.Range(min.baseroughness, max.baseroughness);
        settings.center = RandomXT.RandomVector3(min.center, max.center);
        settings.layers = Random.Range(min.layers, max.layers);
        settings.minvalue = Random.Range(min.minvalue, max.minvalue);
        settings.persistence = Random.Range(min.persistence, max.persistence);
        settings.roughness = Random.Range(min.roughness, max.roughness);
        settings.strength = Random.Range(min.strength, max.strength);

        return lastpicked = settings;
    }

    public NoiseSettings.SimpleNoiseSettings PickRandomValueClamped()
    {
        NoiseSettings.SimpleNoiseSettings settings = PickRandomValue();

        // -- clamp persistance to avoid gigantic juts 
        if (settings.persistence * 4 < settings.strength)
            settings.persistence = settings.strength * 0.5f;

        // -- clamp minvalue to always be less than strength + persistance to avoid more juts
        if (settings.strength + settings.persistence < settings.minvalue)
            settings.minvalue = (settings.strength + settings.persistence) * Random.Range(0.75f, 0.95f);

        return lastpicked = settings;
    }
}