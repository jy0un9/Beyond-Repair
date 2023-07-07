using UnityEngine;

public class WaterColorController : MonoBehaviour
{
    public Material waterMaterial;
    public Light directionalLight;

    [Header("Day Colors")]
    public Color dayIntersectionColor;

    [Header("Night Colors")]
    public Color nightIntersectionColor;

    private void Update()
    {
        float intensityFactor = Mathf.Clamp01(directionalLight.intensity / 4.5f);
        Color intersectionColor = Color.Lerp(nightIntersectionColor, dayIntersectionColor, intensityFactor);
        waterMaterial.SetColor("_IntersectionColor", intersectionColor);
    }
}