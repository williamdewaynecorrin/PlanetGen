using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // -- nested classes
    public enum FaceRenderType
    {
        All, Top, Bot, Left, Right, Front, Back
    }

    // -- properties

    [Range(2, 64)]
    public int resolution = 10;
    public bool autoupdate = true;
    public FaceRenderType rendertype = FaceRenderType.All;

    public ColorSettings colorsettings;
    public ShapeSettings shapesettings;

    [HideInInspector]
    public bool shapesettingsfoldout;
    [HideInInspector]
    public bool colorsettingsfoldout;

    ShapeGenerator shapegenerator = new ShapeGenerator();
    ColorGenerator colorgenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    private MeshFilter[] meshfilters;
    private TerrainFace[] terrainfaces;

    private bool proceduallygenerated = false;

    void OnValidate()
    {
        //GeneratePlanet();
    }

    void Start()
    {
        if(!proceduallygenerated)
            GeneratePlanet();
    }

    public void ConstructPlanetWithValues(int resolution, ColorSettings csettings, ShapeSettings ssettings)
    {
        this.resolution = resolution;
        this.colorsettings = csettings;
        this.shapesettings = ssettings;
        proceduallygenerated = true;

        GeneratePlanet();
    }

    void Init()
    {
        shapegenerator.UpdateSettings(shapesettings);
        colorgenerator.UpdateSettings(colorsettings);

        if(meshfilters == null || meshfilters.Length == 0)
            meshfilters = new MeshFilter[6];
        terrainfaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < meshfilters.Length; ++i)
        {
            // -- generate mesh if we haven't yet
            if (meshfilters[i] == null)
            {
                GameObject mesh = new GameObject("mesh" + i.ToString());
                mesh.transform.parent = transform;

                mesh.AddComponent<MeshRenderer>();
                meshfilters[i] = mesh.AddComponent<MeshFilter>();
                meshfilters[i].mesh = new Mesh();
            }

            meshfilters[i].GetComponent<MeshRenderer>().material = colorsettings.planetmaterial;

            // -- generate terrain face
            terrainfaces[i] = new TerrainFace(shapegenerator, meshfilters[i].sharedMesh, resolution, directions[i]);
            bool renderface = rendertype == FaceRenderType.All || (int)rendertype - 1 == i;
            meshfilters[i].gameObject.SetActive(renderface);
        }
    }

    public void GeneratePlanet()
    {
        Init();
        GenerateMesh();
        GenerateColors();
    }

    void GenerateMesh()
    {
        for(int i = 0; i < terrainfaces.Length; ++i)
        {
            if(meshfilters[i].gameObject.activeSelf)
                terrainfaces[i].ConstructMesh(proceduallygenerated);
        }

        colorgenerator.UpdateElevation(shapegenerator.elevationminmax);
    }

    public void OnShapeSettingsUpdate()
    {
        if (!autoupdate)
            return;

        Init();
        GenerateMesh();
    }

    public void OnColorSettingsUpdated()
    {
        if (!autoupdate)
            return;

        Init();
        GenerateColors();
    }

    void GenerateColors()
    {
        colorgenerator.UpdateColors();

        // -- only update UVs if we are using biomes, since a regular gradient doesn't need them
        //if(colorsettings.planettype == ColorSettings.PlanetColorType.BIOMES)
        //{
            for (int i = 0; i < terrainfaces.Length; ++i)
            {
                terrainfaces[i].UpdateUVs(colorgenerator);
            }
        //}
    }
}
