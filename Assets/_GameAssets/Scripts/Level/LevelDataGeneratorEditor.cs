#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelDataGeneratorEditor
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate 100 LevelData With Rules")]
    public static void Generate100Levels()
    {
        // Tạo folder nếu chưa có
        string rootFolder = "Assets/_GameAssets/LevelSO";
        string folderPath = rootFolder + "/LevelAuto";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(rootFolder, "LevelAuto");
        }

        // Load các LevelData mẫu có map
        var allLevelDataWithMaps = AssetDatabase.FindAssets("t:LevelData", new[] { "Assets/_GameAssets/LevelSO/LevelTest" })
            .Select(guid => AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(ld => ld != null && ld.map != null)
            .ToList();

        // Load tất cả CharacterController từ folder
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_GameAssets/Prefab/Prefabchar" });

        var allCharacters = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(go => go != null && go.GetComponent<CharacterController>() != null)
            .Select(go => go.GetComponent<CharacterController>())
            .ToList();
        Debug.Log($"Tìm thấy {allCharacters.Count} CharacterController trong folder Assets/_GameAssets/Prefab/Prefabchar");

        var batch = ScriptableObject.CreateInstance<LevelDataBatch>();
        batch.generatedLevels = new List<LevelData>();
        bool isLevel2Pistol = false;

        for (int i = 1; i <= 100; i++)
        {
            if ((i - 1) % 5 == 0)
            {
                // Đầu mỗi cụm 5 level → chọn random 1 trong 2 level là pistol
                isLevel2Pistol = Random.value < 0.5f;
            }

            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.level = i;

            // 1. Random map + camera
            var reference = allLevelDataWithMaps.OrderBy(_ => Random.value).First();
            level.map = reference.map;
            level.cameraPosition = reference.cameraPosition;
            level.cameraRotation = reference.cameraRotation;

            // 2. Target count & Weapon logic
            int targetCount;
            bool isPistolLevel = false;

            if (i % 5 == 2) // level 2
            {
                if (isLevel2Pistol)
                {
                    isPistolLevel = true;
                    targetCount = 1;
                }
                else
                {
                    isPistolLevel = false;
                    targetCount = 2;
                }
            }
            else if (i % 5 == 4) // level 4
            {
                if (!isLevel2Pistol)
                {
                    isPistolLevel = true;
                    targetCount = 1;
                }
                else
                {
                    isPistolLevel = false;
                    targetCount = 2;
                }
            }
            else
            {
                // Các level còn lại dùng công thức bình thường
                int difficultyIndex = (i - 1) % 5;
                targetCount = difficultyIndex switch
                {
                    0 => 1,
                    2 => 2,
                    4 => 3,
                    _ => 1
                };

                isPistolLevel = (i % 5 == 2 || i % 5 == 4) && difficultyIndex != 1;
            }

            level.playerWeapon = isPistolLevel ? LevelData.WeaponType.Pistol : LevelData.WeaponType.Bow;

            // 3. Chọn character
            int totalCharCount = targetCount switch
            {
                1 => Random.Range(3, 5),
                2 => Random.Range(4, 6),
                _ => Random.Range(5, 7),
            };
            var selectedChars = allCharacters.OrderBy(c => Random.value).Take(totalCharCount).ToList();
            level.characters = selectedChars;

            // 4. Gán CharacterTarget
            level._characterTarget = new CharacterTarget[targetCount];
            for (int j = 0; j < targetCount; j++)
            {
                var ct = new CharacterTarget();
                ct.EmojiTypeTarget = (EmojiType)Random.Range(0, System.Enum.GetValues(typeof(EmojiType)).Length);
                int enemyCount = Random.Range(1, Mathf.Min(3, selectedChars.Count + 1));
                ct.EnemyTarget = selectedChars.OrderBy(c => Random.value).Take(enemyCount).ToList();
                ct.UpdatePreviewSprites(level.playerWeapon);
                level._characterTarget[j] = ct;
            }

            level.quantityEmojiRandom = Random.Range(1, 4);
            level.GenerateEmojiTypeList();

            string path = $"{folderPath}/Level_{i}.asset";
            AssetDatabase.CreateAsset(level, path);
            batch.generatedLevels.Add(level);
        }


        AssetDatabase.CreateAsset(batch, $"{folderPath}/LevelDataBatch.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Tạo xong 100 level theo quy tắc!");
    }
#endif
}
