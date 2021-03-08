using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator generator;
    Mesh mesh;
    int resolution;
    Vector3 localup;
    Vector3 axisa;
    Vector3 axisb;

    public TerrainFace(ShapeGenerator generator, Mesh mesh, int resolution, Vector3 localup)
    {
        this.generator = generator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localup = localup;

        axisa = new Vector3(localup.y, localup.z, localup.x);
        axisb = Vector3.Cross(localup, axisa);
    }

    public void ConstructMesh(bool proceduallygenerated)
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int rmin1 = resolution - 1;
        int[] triangles = new int[rmin1 * rmin1 * 6];
        int triindex = 0;
        Vector2[] uvs;

        // assign uvs
        if (proceduallygenerated)
        {
            uvs = new Vector2[vertices.Length];
        }
        else
        {
            if (mesh.uv.Length == vertices.Length)
            {
                uvs = mesh.uv;
            }
            else
            {
                uvs = new Vector2[vertices.Length];
            }
        }

        // -- fill out verts
        for(int y = 0; y < resolution; ++y)
        {
            for (int x = 0; x < resolution; ++x)
            {
                int index = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / rmin1;
                Vector3 unitcubepoint = localup + (percent.x - 0.5f) * 2 * axisa + (percent.y - 0.5f) * 2 * axisb;
                Vector3 unitspherepoint = unitcubepoint.normalized;
                float unscaledelevation = generator.CalculateUnscaledElevation(unitspherepoint);
                vertices[index] = unitspherepoint * generator.GetScaledElevation(unscaledelevation);
                uvs[index].y = unscaledelevation;

                if(x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triindex] = index;
                    triangles[triindex + 1] = index + resolution + 1;
                    triangles[triindex + 2] = index + resolution;

                    triangles[triindex + 3] = index;
                    triangles[triindex + 4] = index + 1;
                    triangles[triindex + 5] = index + resolution + 1;
                    triindex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.uv = uvs;
    }

    public void UpdateUVs(ColorGenerator generator)
    {
        int rmin1 = resolution - 1;
        Vector2[] uvs = mesh.uv;

        for (int y = 0; y < resolution; ++y)
        {
            for (int x = 0; x < resolution; ++x)
            {
                int index = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / rmin1;
                Vector3 unitcubepoint = localup + (percent.x - 0.5f) * 2 * axisa + (percent.y - 0.5f) * 2 * axisb;
                Vector3 unitspherepoint = unitcubepoint.normalized;

                uvs[index].x = generator.BiomePercentFromPoint(unitspherepoint);
            }
        }

        mesh.uv = uvs;
    }
}
