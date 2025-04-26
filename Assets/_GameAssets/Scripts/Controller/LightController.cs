using UnityEngine;
using UnityEngine.SceneManagement;

public class LightController : Singleton<LightController>
{
    [SerializeField] private Material defaultSkybox;
    [SerializeField] private Material darkSkybox;

    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    // Chỉ đổi Skybox nếu chưa có
       
    //        RenderSettings.skybox = defaultSkybox;
    //        DynamicGI.UpdateEnvironment();
        
    //}

    public void SetDarkSkybox()
    {
        if (darkSkybox != null)
        {
            RenderSettings.skybox = darkSkybox;
            DynamicGI.UpdateEnvironment();
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
