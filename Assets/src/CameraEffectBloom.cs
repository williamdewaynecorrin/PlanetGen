using UnityEngine;
using System;

[ExecuteInEditMode]//, ImageEffectAllowedInSceneView]
public class CameraEffectBloom : MonoBehaviour
{
    [SerializeField]
    private Shader bloomshader;
    [Range(1, 16)]
    [SerializeField]
    private int iterations = 1;
    [Range(0f, 20f)]
    [SerializeField]
    private float intensity = 1;
    [Range(0.5f, 20f)]
    [SerializeField]
    private float threshold = 1;
    [Range(0f, 1f)]
    [SerializeField]
    private float softthreshold = 0.5f;
    [Range(0.1f, 3f)]
    [SerializeField]
    private float downsampledelta = 1f;
    [Range(0.1f, 3f)]
    [SerializeField]
    private float upsampledelta = 0.5f;
    [SerializeField]
    private bool visualize = false;

    private Material bloommat;
    private RenderTexture[] textures;
    private const int prefilterpass = 0;
    private const int downsamplepass = 1;
    private const int upsamplepass = 2;
    private const int applybloompass = 3;
    private const int debugpass = 4;

    private void Awake()
    {
        textures = new RenderTexture[16];
        ConstructMaterial();
    }

    private void ConstructMaterial()
    {
        if (bloommat == null)
        {
            bloommat = new Material(bloomshader);
            bloommat.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    // -- use a Vector4 to hold our pre-calculated values for optimization purposes
    private void PassThresholdVariables()
    {
        float knee = threshold * softthreshold;
        Vector4 filter;
        filter.x = threshold;
        filter.y = threshold - knee;
        filter.z = 2f * knee;
        filter.w = 0.25f / (knee + 0.00001f);
        bloommat.SetVector("_Filter", filter);
        bloommat.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));
        bloommat.SetVector("_SampleDelta", new Vector2(downsampledelta, upsampledelta));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ConstructMaterial();
        PassThresholdVariables();

        RenderTextureFormat format = source.format;
        int w = source.width;
        int h = source.height;

        RenderTexture currentdest = RenderTexture.GetTemporary(w, h, 0, format);
        textures[0] = currentdest;
        Graphics.Blit(source, currentdest, bloommat, prefilterpass);
        RenderTexture currentsource = currentdest;

        int i = 1;
        for (; i < iterations; ++i)
        {
            w /= 2;
            h /= 2;
            if (h < 2 || w < 2)
                break;

            currentdest = RenderTexture.GetTemporary(w, h, 0, format);
            textures[i] = currentdest;

            Graphics.Blit(currentsource, currentdest, bloommat, downsamplepass);
            currentsource = currentdest;
        }

        for(i -= 2; i >= 0; --i)
        {
            currentdest = textures[i];
            textures[i] = null;
            Graphics.Blit(currentsource, currentdest, bloommat, upsamplepass);
            RenderTexture.ReleaseTemporary(currentsource);
            currentsource = currentdest;
        }

        if(visualize)
            Graphics.Blit(currentsource, destination, bloommat, debugpass);
        else
        {
            bloommat.SetTexture("_SourceTex", currentsource);
            Graphics.Blit(source, destination, bloommat, applybloompass);
        }
        
        RenderTexture.ReleaseTemporary(currentsource);
    }
}
