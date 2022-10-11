using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraEffect : MonoBehaviour
{
    public static CameraEffect Inst { get; private set; }
    void Awake() => Inst = this;

    Material cameraMaterial;
    public float intensity;

    void Start()
    {
        cameraMaterial = new Material(Shader.Find("Hidden/GrayscaleImageEffectShader"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if(intensity == 0)
        {
            Graphics.Blit(src, dest);
            return;
        }
        else
        {
            cameraMaterial.SetFloat("_bwBelnd", intensity);
            Graphics.Blit(src, dest, cameraMaterial);
            return;
        }
    }
}
