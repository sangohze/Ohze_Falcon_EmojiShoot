#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static RootMotion.FinalIK.GrounderQuadruped;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LevelDataGeneratorEditor
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate 100 LevelData With Rules")]
    public static void Generate100Levels()
    {
        // Tạo folder nếu chưa có
        string folderPath = "Assets/_GameAssets/LevelSO/LevelAuto";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedLevels");
        }

        // Load các bản đồ có chứa camera preset
        var allMaps = AssetDatabase.FindAssets("t:LevelData", new[] { "Assets/_GameAssets/LevelSO/LevelTest" })
     .Select(guid => AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid)))
     .Where(ld => ld != null && ld.map != null)
     .OrderBy(_ => Random.value)
     .First()
     .map;

        // Load các character từ Resources/Characters/
        var allCharacters = AssetDatabase.FindAssets("t:CharacterController", new[] { "Assets/_GameAssets/Prefab/Prefab char" })
    .Select(guid => AssetDatabase.LoadAssetAtPath<CharacterController>(AssetDatabase.GUIDToAssetPath(guid)))
    .Where(c => c != null)
    .ToList();
    var batch = ScriptableObject.CreateInstance<LevelDataBatch>();
        batch.generatedLevels = new List<LevelData>();

        for (int i = 1; i <= 10; i++)
        {
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.level = i;

            // 1. Random map
            GameObject map = allMaps;
            level.map = map;

            // 2. Lấy camera pos & rot từ map
            Transform camTransform = map.transform.Find("CameraPoint");
            if (camTransform != null)
            {
                level.cameraPosition = camTransform.position;
                level.cameraRotation = camTransform.rotation;
            }
            // 3. WeaponType logic

            bool isPistolLevel = (i % 5 == 1 || i % 5 == 3); // Level 2 và 4 trong nhóm 5 (index 1, 3)
            level.playerWeapon = isPistolLevel ? LevelData.WeaponType.Pistol : LevelData.WeaponType.Bow;

            // 4. CharacterTarget logic
            int difficultyIndex = i % 5; // 0 -> Easy, 2 -> Medium, 4 -> Hard
            int targetCount = 1;

            if (difficultyIndex == 0) targetCount = 1;           // Easy
            else if (difficultyIndex == 2) targetCount = 2;      // Medium
            else if (difficultyIndex == 4) targetCount = 3;      // Hard

            // Nếu là level Pistol thì hoán đổi Easy-Medium nếu cần
            if (isPistolLevel)
            {
                if (targetCount == 1) targetCount = 2;
                else if (targetCount == 2) targetCount = 1;
            }

            level._characterTarget = new CharacterTarget[targetCount];

            for (int j = 0; j < targetCount; j++)
            {
                var ct = new CharacterTarget();

                // Random emoji target
                ct.EmojiTypeTarget = (EmojiType)Random.Range(0, System.Enum.GetValues(typeof(EmojiType)).Length);

                // Random từ danh sách character
                int charCount = targetCount switch
                {
                    1 => Random.Range(3, 5), // Easy
                    2 => Random.Range(4, 6), // Medium
                    _ => Random.Range(5, 7), // Hard
                };

                var selectedChars = allCharacters.OrderBy(c => Random.value).Take(charCount).ToList();
                level.characters = selectedChars;

                // EnemyTarget: random 1-2 char trong selected
                int enemyCount = Random.Range(1, 3);
                ct.EnemyTarget = selectedChars.OrderBy(c => Random.value).Take(enemyCount).ToList();

                // Gán emoji preview + mission text
                ct.UpdatePreviewSprites(level.playerWeapon);

                level._characterTarget[j] = ct;
            }

            // Emoji random thêm
            level.quantityEmojiRandom = Random.Range(1, 4);
            level.GenerateEmojiTypeList();

            // Lưu asset riêng
            string path = $"{folderPath}/Level_{i}.asset";
            AssetDatabase.CreateAsset(level, path);
            batch.generatedLevels.Add(level);
        }

        // Lưu batch chứa 100 level
        AssetDatabase.CreateAsset(batch, $"{folderPath}/LevelDataBatch.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Tạo xong 100 level theo quy tắc!");
    }
#endif
}
