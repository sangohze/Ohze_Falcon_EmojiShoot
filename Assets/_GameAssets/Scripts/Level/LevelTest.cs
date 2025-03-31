using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelTest : Singleton<LevelTest>
{
    [Header("For Test")]
    public int currentLevelIndex;
    public Transform cameraTransform; 
 
    public GameObject currentMap;
    public List<CharacterController> CurrentEnemyTargets = new List<CharacterController>();
    public List<CharacterController> CurrentListEnemy;
    public EmojiType emojiTypeTarget;

    

    public void LoadLevelTest()
    {
        if (cameraTransform == null || currentMap == null) return;
        
        currentMap = Instantiate(currentMap, currentMap.transform.position, Quaternion.identity);
        Camera.main.transform.position = cameraTransform.position;
        Camera.main.transform.rotation = cameraTransform.rotation;
        foreach (CharacterController charactersPrefab in CurrentListEnemy)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Quaternion spawnRot = Quaternion.Euler(0, 90, 0);
            CharacterController enemy = Instantiate(charactersPrefab, spawnPosition, spawnRot);      
        }
    }


    private Vector3 GetRandomSpawnPosition()
    {
        float enemyY = currentMap.transform.position.y - 0.1f; // Enemy luôn thấp hơn map 0.1 đơn vị
        float enemyX = cameraTransform.position.x + Random.Range(-6, -9); // X cách camera một khoảng
        float enemyZ = Random.Range(-10, 10); // Luôn trong khoảng -10 đến 10 trên trục Z

        return new Vector3(enemyX, enemyY, enemyZ);
    }

    [Button]
    public void SaveLevelData()
    {
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        newLevelData.level = currentLevelIndex;
        newLevelData.map = currentMap;
        newLevelData.cameraPosition = cameraTransform.position;
        newLevelData.cameraRotation = cameraTransform.rotation;
        newLevelData.EmojiTypeTarget = emojiTypeTarget;
        newLevelData.characters = new List<CharacterController>(CurrentListEnemy);
        newLevelData.charactersTarget = new List<CharacterController>(CurrentEnemyTargets);

        // Lưu scriptable object
#if UNITY_EDITOR
        string path = "Assets/_GameAssets/LevelSO/LevelTest/Level" + currentLevelIndex + ".asset";
        UnityEditor.AssetDatabase.CreateAsset(newLevelData, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("Level data saved at: " + path);
#endif
    }
}