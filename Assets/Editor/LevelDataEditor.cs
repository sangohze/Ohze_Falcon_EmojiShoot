
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static LevelData;
using System.Reflection;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor // ✅ Kế thừa từ OdinEditor thay vì Editor
{
    private List<LevelData> allLevelData = new List<LevelData>();
    private string[] levelOptions;
    private int selectedIndex = -1;

    private string levelDataFolderPath = "Assets/_GameAssets/LevelSO/LevelTest";

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI(); // ✅ Rất quan trọng: để giữ lại PreviewField, Button, v.v...
        EditorGUILayout.Space(20);
        DrawUpdatePreviewButton();
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
        var levelTest = (LevelData)target;
        var data = allLevelData[index];

        bool isSameMap = levelTest.map == data.map;
        bool isSameCam =
            levelTest.cameraPosition != null &&
            levelTest.cameraPosition == data.cameraPosition &&
            levelTest.cameraRotation == data.cameraRotation;

        if (isSameMap && isSameCam)
        {
            Debug.Log("Map và camera đã đúng. Không cần thay đổi.");
            return;
        }

        levelTest.map = data.map;

        if (levelTest.cameraPosition != null)
        {
            levelTest.cameraPosition = data.cameraPosition;
            levelTest.cameraRotation = data.cameraRotation;
        }
        else
        {
            Debug.LogWarning("cameraTransform chưa được gán trong LevelTest.");
        }

        EditorUtility.SetDirty(levelTest);
        Debug.Log($"Đã gán dữ liệu từ LevelData: {data.name}");


    }

    private void DrawUpdatePreviewButton()
    {
        LevelData levelData = (LevelData)target;

        if (GUILayout.Button("Cập Nhật Sprite Cho CharacterTarget"))
        {
            if (levelData._characterTarget != null)
            {
                foreach (var ct in levelData._characterTarget)
                {
                    if (levelData.playerWeapon == WeaponType.Pistol)
                    {
                        AssignPistolSprite(ct); // 👈 Gán sprite vào field ẩn
                    }
                    if (ct != null)
                    {
                        ct.UpdatePreviewSprites(levelData.playerWeapon);
                    }
                }

                // Đánh dấu object đã thay đổi để lưu lại
                EditorUtility.SetDirty(levelData);
                AssetDatabase.SaveAssets();
            }
        }
    }
    private void AssignPistolSprite(CharacterTarget ct)
    {
        string path = "Assets/_GameAssets/Image/emoji_image/Mission_emoji/Crowd.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (sprite == null)
        {
            Debug.LogWarning("Không tìm thấy sprite tại: " + path);
            return;
        }

        var field = typeof(CharacterTarget).GetField("PistolPreviewAva", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(ct, sprite);
        }
       
    }
}
