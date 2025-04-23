using UnityEngine;
using DG.Tweening;

public class LightController : Singleton<LightController>
{
    [SerializeField] private Material defaultSkybox;
    [SerializeField] private Material darkSkybox;

    public void SetDarkSkybox()
    {
        if (darkSkybox != null)
        {
            RenderSettings.skybox = darkSkybox;
            DynamicGI.UpdateEnvironment(); // Cập nhật ánh sáng môi trường
        }
    }

    public void RestoreDefaultSkybox()
    {
        if (defaultSkybox != null)
        {
            RenderSettings.skybox = defaultSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }
}
