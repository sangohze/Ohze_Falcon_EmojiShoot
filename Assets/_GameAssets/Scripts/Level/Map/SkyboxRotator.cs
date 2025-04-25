using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("Tốc độ quay (độ mỗi giây)")]
    [SerializeField] float rotationSpeed = 1f;

    [Tooltip("Material của Skybox")]
    [SerializeField] Material skyboxMaterial;
    [SerializeField] Material skyboxMaterialDark;
    private float currentRotation = 0f;

    void Update()
    {
        if (skyboxMaterial != null)
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            currentRotation %= 360f; 
            skyboxMaterial.SetFloat("_Rotation", currentRotation);
            skyboxMaterialDark.SetFloat("_Rotation", currentRotation);
        }
    }
}
