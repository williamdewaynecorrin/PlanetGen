using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [Header("Planet Generation Settings")]
    public RandomPlanetSettings planetsettings;
    [HideInInspector]
    public bool planetsettingsfoldout = false;

    [Header("Other Attributes")]
    public Material copymaterial;
    public Transform[] spawnlocations;
    public bool parenttospawns = false;

    [Header("Atmosphere Attributes")]
    public GameObject atmosphereprefab;

    private GameObject[] planetobjects;
    private Planet[] planets = null;
    private GameObject[] atmospheres = null;

    // Start is called before the first frame update

    void Awake()
    {
        if(spawnlocations== null || spawnlocations.Length == 0)
        {
            spawnlocations = new Transform[] { this.transform };
        }

        // -- initialize arrays
        planetobjects = new GameObject[spawnlocations.Length];
        planets = new Planet[spawnlocations.Length];
        atmospheres = new GameObject[spawnlocations.Length];

        // -- create and randomize new planets
        for (int i = 0; i < spawnlocations.Length; ++i)
        {
            planetobjects[i] = new GameObject("Generated Planet");
            planets[i] = planetobjects[i].AddComponent<Planet>();

            // -- randomize planet
            RandomizePlanet(planets[i]);

            // -- randomize atmosphere
            RandomizeAtmosphere(i);

            planetobjects[i].transform.position = spawnlocations[i].position;
            planetobjects[i].transform.rotation = Quaternion.identity;
            planetobjects[i].transform.localScale = Vector3.one;

            if (parenttospawns)
            {
                planetobjects[i].transform.SetParent(spawnlocations[i]);
                planetobjects[i].transform.localPosition = Vector3.zero;
                planetobjects[i].transform.localRotation = Quaternion.identity;
            }


            Material atmospheremat = atmospheres[i].GetComponent<MeshRenderer>().material;
            atmospheremat.SetColor("_AtmosphereOuter", planets[i].colorsettings.biomesettings.biomes[0].gradient.Evaluate(Random.Range(0f, 1f)));
            atmospheremat.SetColor("_AtmosphereInner", planets[i].colorsettings.biomesettings.oceancolor.Evaluate(Random.Range(0f, 1f)));
            atmospheremat.SetFloat("_Density", Random.Range(0.8f, 1.2f));
            atmospheremat.SetFloat("_Fresnel", Random.Range(0.8f, 1.4f));
            atmospheremat.SetFloat("_Emission", Random.Range(0.7f, 1.2f));

            RotateComponent rotatescript = planets[i].gameObject.AddComponent<RotateComponent>();
            rotatescript.rotaxis = RandomXT.RandomUnitVector3();
            rotatescript.rotspeed = Random.Range(0.2f, 0.4f);
        }
    }

    void Start()
    {

    }

    public void RandomizePlanet(Planet planet)
    {
        // -- fill in shape settings randomly using our random ranges
        ShapeSettings ssettings = new ShapeSettings();
        ssettings.planetradius = planetsettings.planetradius.PickRandomValue();
        ssettings.noiselayers = new ShapeSettings.NoiseLayer[planetsettings.noiselayers.PickRandomValue()];
        for (int i = 0; i < ssettings.noiselayers.Length; ++i)
        {
            ShapeSettings.NoiseLayer layer = new ShapeSettings.NoiseLayer();
            layer.enabled = true;
            layer.usefirstlayerasmask = i == 0 ? false : RandomXT.RandomBool();

            NoiseSettings tns = new NoiseSettings();
            tns.filtertype = NoiseSettings.FilterType.SIMPLE;
            tns.simplesettings = planetsettings.terrainnoisesettings.PickRandomValueClamped();
            layer.settings = tns;

            ssettings.noiselayers[i] = layer;
        }

        // -- fill in the color settings randomly using our random ranges
        ColorSettings csettings = new ColorSettings();
        csettings.planetmaterial = new Material(copymaterial);
        csettings.planetmaterial.SetFloat("_Smoothness", planetsettings.watersmoothness.PickRandomValue());
        csettings.planetmaterial.SetVector("_WaterNoiseScale", planetsettings.waterscale.PickRandomValue());
        csettings.planetmaterial.SetVector("_WaterScroll", planetsettings.waterscroll.PickRandomValue());
        csettings.planetmaterial.SetFloat("_WaterStrength", planetsettings.waterbumpstrength.PickRandomValue());
        csettings.planettype = ColorSettings.PlanetColorType.BIOMES;

        // -- main biome settings
        ColorSettings.BiomeColorSettings biomesettings = new ColorSettings.BiomeColorSettings();
        biomesettings.noiseoffset = planetsettings.biomenoiseoffset.PickRandomValue();
        biomesettings.noisestrength = planetsettings.biomenoisestrength.PickRandomValue();
        biomesettings.blendamount = planetsettings.biomeblendamount.PickRandomValue();
        biomesettings.oceancolor = RandomXT.RandomGradient(new Color[] { planetsettings.planetoceandepththeme.PickRandomValue(),
                                                                         planetsettings.planetoceansurfacetheme.PickRandomValue() });

        // -- biome noise settings
        NoiseSettings biomenoise = new NoiseSettings();
        biomenoise.filtertype = NoiseSettings.FilterType.SIMPLE;
        biomenoise.simplesettings = planetsettings.biomenoisesettings.PickRandomValue();

        biomesettings.noise = biomenoise;

        // -- biome color settings
        ColorSettings.BiomeColorSettings.Biome[] biomes = new ColorSettings.BiomeColorSettings.Biome[planetsettings.biomescount.PickRandomValue()];
        float startheight = 0;
        float increment = 1f / (float)biomes.Length;
        for (int i = 0; i < biomes.Length; ++i)
        {
            biomes[i] = new ColorSettings.BiomeColorSettings.Biome();
            biomes[i].gradient = RandomXT.RandomGradient(new Color[] { planetsettings.planetgroundtheme.PickRandomValue(),
                                                                       planetsettings.planetclifftheme.PickRandomValue(),
                                                                       planetsettings.planetclifftoptheme.PickRandomValue() });
            biomes[i].startheight = startheight;
            biomes[i].tint = biomes[i].gradient.Evaluate(Mathf.Clamp01(Random.Range(0f, 0.8f)));
            biomes[i].tintpercent = planetsettings.biometintpct.PickRandomValue();

            startheight += increment;
        }

        biomesettings.biomes = biomes;

        // -- finally apply all of the color settings
        csettings.biomesettings = biomesettings;

        // -- finally construct our planet with our randomly generated values
        planet.ConstructPlanetWithValues(planetsettings.resolution.PickRandomValue(), csettings, ssettings);
    }

    public void RandomizeAtmosphere(int i)
    {
        atmospheres[i] = GameObject.Instantiate(atmosphereprefab, planetobjects[i].transform.position, planetobjects[i].transform.rotation);
        atmospheres[i].transform.localScale = Vector3.one;
        atmospheres[i].transform.SetParent(planetobjects[i].transform);
        atmospheres[i].transform.localPosition = Vector3.zero;
        atmospheres[i].transform.localRotation = Quaternion.identity;
        atmospheres[i].transform.localScale = Vector3.one * planetsettings.planetradius.lastpicked * 4f * Random.Range(1f, 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
    