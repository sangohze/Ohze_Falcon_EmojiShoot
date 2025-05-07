
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(LevelTest))]
public class LevelTestEditor : Editor // ✅ Kế thừa từ OdinEditor thay vì Editor
{
    private List<LevelData> allLevelData = new List<LevelData>();
    private string[] levelOptions;
    private int selectedIndex = -1;

    private string levelDataFolderPath = "Assets/_GameAssets/LevelSO/LevelTest";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // ✅ Rất quan trọng: để giữ lại PreviewField, Button, v.v...

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Map Camera Selector", EditorStyles.boldLabel);

        LoadAllLevelData();

        if (levelOptions.Length > 0)
        {
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup("Select Map Camera", selectedIndex, levelOptions);
            if (EditorGUI.EndChangeCheck())
            {
                ApplySelectedLevelData(selectedIndex);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Không tìm thấy LevelData trong: " + levelDataFolderPath, MessageType.Warning);
        }
    }

    private void LoadAllLevelData()
    {
        allLevelData.Clear();
        string[] guids = AssetDatabase.FindAssets("t:LevelData", new[] { levelDataFolderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (data != null)
                allLevelData.Add(data);
        }

        levelOptions = allLevelData.Select(d => $"{d.map?.name}_camera_lv{d.level}").ToArray();
    }

    private void ApplySelectedLevelData(int index)
    {
        var levelTest = (LevelTest)target;
        var data = allLevelData[index];

        bool isSameMap = levelTest.currentMap == data.map;
        bool isSameCam =
            levelTest.cameraTransform != null &&
            levelTest.cameraTransform.position == data.cameraPosition &&
            levelTest.cameraTransform.rotation == data.cameraRotation;

        if (isSameMap && isSameCam)
        {
            Debug.Log("Map và camera đã đúng. Không cần thay đổi.");
            return;
        }

        levelTest.currentMap = data.map;

        if (levelTest.cameraTransform != null)
        {
            levelTest.cameraTransform.position = data.cameraPosition;
            levelTest.cameraTransform.rotation = data.cameraRotation;
        }
        else
        {
            Debug.LogWarning("cameraTransform chưa được gán trong LevelTest.");
        }

        EditorUtility.SetDirty(levelTest);
        Debug.Log($"Đã gán dữ liệu từ LevelData: {data.name}");
    }
}
