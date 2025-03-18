using UnityEditor;
using UnityEngine;

public class MaterialConverter : EditorWindow
{
    [MenuItem("Tools/Convert All Model Materials to Standard")]
    public static void ConvertAllModelMaterialsToStandard()
    {
        string[] modelGuids = AssetDatabase.FindAssets("t:Model");
        foreach (string guid in modelGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (model != null)
            {
                Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat != null && mat.shader.name.Contains("Hidden/InternalErrorShader"))
                        {
                            mat.shader = Shader.Find("Standard");
                            Debug.Log($"Converted: {mat.name} in model: {model.name}");
                        }
                        mat.shader = Shader.Find("Standard");
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
    }
}




