using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
    // -- material
    public static void SetBool(this Material m, string property, bool value)
    {
        int ivalue = value ? 1 : 0;
        m.SetInt(property, ivalue);
    }

    // -- vector 3
    public static float Product(this Vector3 v)
    {
        return v.x * v.y * v.z;
    }

    public static Vector3 NoY(this Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    public static Vector3 SetX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3Int ToVector3IntRound(this Vector3 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }

    public static Vector3Int ToVector3IntRound(this Vector3 v, RoundType yround)
    {
        int vy = Mathf.RoundToInt(v.y);
        if (yround == RoundType.Ceil)
            vy = Mathf.CeilToInt(v.y);
        else if (yround == RoundType.Floor)
            vy = Mathf.FloorToInt(v.y);

        return new Vector3Int(Mathf.RoundToInt(v.x), vy, Mathf.RoundToInt(v.z));
    }

    public static Vector3Int ToVector3IntRound(this Vector3 v, RoundType xround, RoundType yround, RoundType zround)
    {
        // -- x
        int vx = Mathf.RoundToInt(v.x);
        if (xround == RoundType.Ceil)
            vx = Mathf.CeilToInt(v.x);
        else if (xround == RoundType.Floor)
            vx = Mathf.FloorToInt(v.x);

        // -- y
        int vy = Mathf.RoundToInt(v.y);
        if (yround == RoundType.Ceil)
            vy = Mathf.CeilToInt(v.y);
        else if (yround == RoundType.Floor)
            vy = Mathf.FloorToInt(v.y);

        // -- z
        int vz = Mathf.RoundToInt(v.z);
        if (zround == RoundType.Ceil)
            vz = Mathf.CeilToInt(v.z);
        else if (zround == RoundType.Floor)
            vz = Mathf.FloorToInt(v.z);

        return new Vector3Int(vx, vy, vz);
    }

    // -- vector4
    public static Vector3 ToVector3(this Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    // -- vector3int
    public static int Sum(this Vector3Int v)
    {
        return v.x + v.y + v.z;
    }

    public static int AbsSum(this Vector3Int v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
    }

    public static int Product(this Vector3Int v)
    {
        return v.x * v.y * v.z;
    }

    public static Vector3 ToVector3(this Vector3Int v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static Vector3 UniformScaleF(this Vector3Int v, float scale)
    {
        Vector3 v3 = new Vector3(v.x, v.y, v.z);
        return v3 * scale;
    }

    public static Vector3Int Divide(this Vector3Int v, Vector3Int other)
    {
        return new Vector3Int(v.x / other.x, v.y / other.y, v.z / other.z);
    }

    public static Vector3Int DivideF(this Vector3Int v, Vector3Int other)
    {
        return new Vector3Int(Mathf.CeilToInt(v.x / (float)other.x),
                              Mathf.CeilToInt(v.y / (float)other.y),
                              Mathf.CeilToInt(v.z / (float)other.z));
    }

    public static int[] Arr(this Vector3Int v)
    {
        return new int[] { (int)v.x, (int)v.y, (int)v.z };
    }

    public static float[] ArrF(this Vector3Int v)
    {
        return new float[] { (float)v.x, (float)v.y, (float)v.z };
    }

    // -- quat
    public static Quaternion ClampRotation(this Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

    public static float QuaternionAngleX(this Quaternion q)
    {
        q.x /= q.w;
        return 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
    }

    public static Rect Indent(this Rect r, float xindent)
    {
        return new Rect(r.x + xindent, r.y, r.width, r.height);
    }

    public static void LogPos(this float[] arr, int sizex, int sizey)
    {
        int hd = sizex * sizey;
        for(int i = 0; i < arr.Length; ++i)
        {
            int x = i / hd;
            int y = (i / sizex) - ((i / hd) * sizey);
            int z = i % sizex;

            Vector3Int index = new Vector3Int(x, y, z);
            Debug.Log("Pos: " + index + ", Val:" + arr[i]);
        }
    }

    public static int IndexFrom3D(Vector3Int coord, int sizex, int sizey)
    {
        int a = sizex * sizey;
        int b = sizex;
        int c = 1;
        int d = 0;

        return a * coord.x + b * coord.y + c * coord.z + d;
    }
}

public static class Vector3IntXT
{
    public static Vector3Int forward = new Vector3Int(0, 0, 1);
    public static Vector3Int back = new Vector3Int(0, 0, -1);
}

public static class MathXT
{
    public static Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.x));
    }
}

public static class RandomXT
{
    public static Vector2 RandomVector2(Vector2 min, Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x),
                           Random.Range(min.y, max.y));
    }

    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x),
                           Random.Range(min.y, max.y),
                           Random.Range(min.z, max.z));
    }

    public static Vector3 RandomUnitVector3()
    {
        return new Vector3(Random.Range(-1f, 1f),
                           Random.Range(-1f, 1f),
                           Random.Range(-1f, 1f)).normalized;
    }

    public static bool RandomBool()
    {
        return Random.Range(0f, 1.0f) > 0.5f;
    }

    public static Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1.0f);
    }

    public static Gradient RandomGradient(Color[] colors)
    {
        GradientAlphaKey[] alphakeys = new GradientAlphaKey[colors.Length];
        GradientColorKey[] colorkeys = new GradientColorKey[colors.Length];

        float time = 0f;
        float increment = 1f / (float)colors.Length;
        for (int i = 0; i < colors.Length; ++i)
        {
            alphakeys[i] = new GradientAlphaKey(colors[i].a, time);
            colorkeys[i] = new GradientColorKey(colors[i], time);
            time += increment;
        }

        //float biomeindex = 0;
        //.......//
        //biomeindex *= (1 - weight);
        //biomeindex += i * weight;

        Gradient gradient = new Gradient();
        gradient.alphaKeys = alphakeys;
        gradient.colorKeys = colorkeys;

        return gradient;
    }
}

public enum RoundType
{
    Round,
    Floor,
    Ceil
}