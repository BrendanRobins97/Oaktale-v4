using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPostProcess : MonoBehaviour {

    [Range(0,1)] public float intensity;
    [SerializeField] private Material blackAndWhiteMaterial;

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
        } else
        {
            blackAndWhiteMaterial.SetFloat("_bwBlend", intensity);
            Graphics.Blit(source, destination, blackAndWhiteMaterial);
        }

    }
}