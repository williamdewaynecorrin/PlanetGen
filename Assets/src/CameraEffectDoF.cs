using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]//, ImageEffectAllowedInSceneView]
public class CameraEffectDoF : MonoBehaviour
{
    [SerializeField]
    private Shader dofshader;
    private Material dofmaterial;

    [Range(0.001f, 50f)]
    [SerializeField]
    private float focusdistance = 10.0f;
    [Range(0.001f, 20f)]
    [SerializeField]
    private float focusrange = 3.0f;
    [Range(0.1f, 20f)]
    [SerializeField]
    private float bokehradius = 5f;
    [SerializeField]
    private bool visualize = false;

    private const int cocpass = 0;
    private const int prefilterpass = 1;
    private const int bokehpass = 2;
    private const int postfilterpass = 3;
    private const int combinepass = 4;
    private const int debugpass = 5;

    private void Awake()
    {
        ConstructMaterial();
    }

    private void ConstructMaterial()
    {
        if (dofmaterial == null)
        {
            dofmaterial = new Material(dofshader);
            dofmaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void PassShaderVariables()
    {
        dofmaterial.SetFloat("_FocusDistance", focusdistance);
        dofmaterial.SetFloat("_FocusRange", focusrange);
        dofmaterial.SetFloat("_BokehRadius", bokehradius);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ConstructMaterial();
        PassShaderVariables();

        int w = source.width;
        int h = source.height;
        RenderTextureFormat format = source.format;

        RenderTexture dof0 = RenderTexture.GetTemporary(w, h, 0, format);
        RenderTexture dof1 = RenderTexture.GetTemporary(w, h, 0, format);
        RenderTexture coctex = RenderTexture.GetTemporary(source.width, source.height, 0, 
                                                          RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

        // -- pass circle of confusion texture before downsample
        dofmaterial.SetTexture("_CocTex", coctex);
        dofmaterial.SetTexture("_DofTex", dof0);

        Graphics.Blit(source, coctex, dofmaterial, cocpass);

        if (visualize)
        {
            Graphics.Blit(source, dof0, dofmaterial, prefilterpass);
            Graphics.Blit(dof0, dof1, dofmaterial, bokehpass);
            Graphics.Blit(source, destination, dofmaterial, debugpass);
        }
        else
        {
            Graphics.Blit(source, dof0, dofmaterial, prefilterpass);
            Graphics.Blit(dof0, dof1, dofmaterial, bokehpass);
            Graphics.Blit(dof1, dof0, dofmaterial, postfilterpass);
            Graphics.Blit(source, destination, dofmaterial, combinepass);
        }

        RenderTexture.ReleaseTemporary(coctex);
        RenderTexture.ReleaseTemporary(dof0);
        RenderTexture.ReleaseTemporary(dof1);
    }
}
