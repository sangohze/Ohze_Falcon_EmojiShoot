#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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
        //setup
        // Pool dàn đều
        List<EmojiType> emojiTypePool = System.Enum.GetValues(typeof(EmojiType)).Cast<EmojiType>().ToList();
        List<EmojiType> excludeList = new List<EmojiType>
{
    EmojiType.Vomit,
    EmojiType.Shit,
    EmojiType.Scared,
    EmojiType.Talkative,
};

        List<EmojiType> emojiTypePoolFiltered = emojiTypePool
            .Where(e => !excludeList.Contains(e))
            .ToList();

        // Shuffle hàm
        void ShuffleEmojiPool(List<EmojiType> pool)
        {
            for (int k = pool.Count - 1; k > 0; k--)
            {
                int rnd = UnityEngine.Random.Range(0, k + 1);
                (pool[k], pool[rnd]) = (pool[rnd], pool[k]);
            }
        }

        ShuffleEmojiPool(emojiTypePool);
        ShuffleEmojiPool(emojiTypePoolFiltered);

        // Pool index
        int emojiPoolIndex = 0;
        int emojiPoolNoVomitIndex = 0;

        // Thống kê xuất hiện
        Dictionary<EmojiType, int> emojiTypeStats = new();

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
                isLevel2Pistol = UnityEngine.Random.value < 0.5f;
            }

            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.level = i;

            // 1. Random map + camera
            var reference = allLevelDataWithMaps.OrderBy(_ => UnityEngine.Random.value).First();
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
                1 => UnityEngine.Random.Range(3, 5),
                2 => UnityEngine.Random.Range(4, 6),
                _ => UnityEngine.Random.Range(5, 7),
            };
            var selectedChars = allCharacters.OrderBy(c => UnityEngine.Random.value).Take(totalCharCount).ToList();
            level.characters = selectedChars;

            // 4. Gán CharacterTarget
            level._characterTarget = new CharacterTarget[targetCount];
            for (int j = 0; j < targetCount; j++)
            {
                var ct = new CharacterTarget();

                // === EmojiType dàn đều ===
                // Chọn từ pool theo index
                if (isPistolLevel && targetCount == 1)
                {
                    ct.EmojiTypeTarget = emojiTypePoolFiltered[emojiPoolNoVomitIndex];
                    emojiPoolNoVomitIndex++;
                    if (emojiPoolNoVomitIndex >= emojiTypePoolFiltered.Count)
                    {
                        ShuffleEmojiPool(emojiTypePoolFiltered);
                        emojiPoolNoVomitIndex = 0;
                    }
                }
                else
                {
                    ct.EmojiTypeTarget = emojiTypePool[emojiPoolIndex];
                    emojiPoolIndex++;
                    if (emojiPoolIndex >= emojiTypePool.Count)
                    {
                        ShuffleEmojiPool(emojiTypePool);
                        emojiPoolIndex = 0;
                    }
                }

                // === Gán enemy ngẫu nhiên ===
                int enemyCount = UnityEngine.Random.Range(1, Mathf.Min(3, selectedChars.Count + 1));
                ct.EnemyTarget = selectedChars.OrderBy(c => UnityEngine.Random.value).Take(enemyCount).ToList();
                ct.UpdatePreviewSprites(level.playerWeapon);
                level._characterTarget[j] = ct;

                // === Thống kê EmojiType ===
                if (!emojiTypeStats.ContainsKey(ct.EmojiTypeTarget))
                    emojiTypeStats[ct.EmojiTypeTarget] = 0;
                emojiTypeStats[ct.EmojiTypeTarget]++;

            }



            level.quantityEmojiRandom = UnityEngine.Random.Range(1, 4);
            level.GenerateEmojiTypeList();

            string path = $"{folderPath}/Level_{i}.asset";
            AssetDatabase.CreateAsset(level, path);
            batch.generatedLevels.Add(level);
        }
        Debug.Log("=== EmojiType xuất hiện tổng kết ===");
        foreach (var kvp in emojiTypeStats.OrderBy(k => k.Key.ToString()))
        {
            Debug.Log($"{kvp.Key}: {kvp.Value} lần");
        }

        AssetDatabase.CreateAsset(batch, $"{folderPath}/LevelDataBatch.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Tạo xong 100 level theo quy tắc!");
    }
#endif
}
