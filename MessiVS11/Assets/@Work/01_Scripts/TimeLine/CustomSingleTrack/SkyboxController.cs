using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    public Material skyboxMaterial;

    private void Awake()
    {
        skyboxMaterial.SetFloat("_Exposure", 0.7f);
    }

    void OnDisable()
    {
        skyboxMaterial.SetFloat("_Exposure", 0.7f);
    }
    public void SetExposure(float value)
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Exposure", value);
        }
    }
}
