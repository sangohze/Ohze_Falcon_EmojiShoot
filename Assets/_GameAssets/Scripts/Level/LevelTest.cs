using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelTest : Singleton<LevelTest>
{
    [Header("For Test")]
    public int currentLevelIndex;
    public Transform cameraTransform; 
    public GameObject currentMap;
    public List<CharacterController> CurrentListEnemy;
    public List<CharacterController> currentEnemyTargets = new List<CharacterController>();
    public EmojiType currentEmojiTypeTarget;
    public CharacterTarget[] _characterTarget;




    public void LoadLevelTest()
    {
        if (cameraTransform == null || currentMap == null) return;
        
        Camera.main.transform.position = cameraTransform.position;
        Camera.main.transform.rotation = cameraTransform.rotation;
        currentMap = Instantiate(currentMap, currentMap.transform.position, Quaternion.identity);
        List<CharacterController> TempCurrentListEnemy = new List<CharacterController>();
        List<CharacterController> TempEnemyTarget = new List<CharacterController>();
        foreach (CharacterController charactersPrefab in CurrentListEnemy)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Quaternion spawnRot = Quaternion.LookRotation(Camera.main.transform.forward);
            CharacterController enemy = Instantiate(charactersPrefab, spawnPosition, spawnRot);
            TempCurrentListEnemy.Add(enemy);
            bool isTarget = _characterTarget[0].EnemyTarget.Any(target => target.name == charactersPrefab.name);
            if (isTarget)
            {
                TempEnemyTarget.Add(enemy);
            }
        }
        currentEnemyTargets.Clear();
        CurrentListEnemy.Clear();
        currentEnemyTargets = TempEnemyTarget;
        CurrentListEnemy = TempCurrentListEnemy;
    }


    private Vector3 GetRandomSpawnPosition()
    {
        // Hướng camera đang nhìn
        Vector3 forward = cameraTransform.forward.normalized;

        // Khoảng cách ngẫu nhiên trước mặt camera
        float randomDistance = Random.Range(6f, 9f);  // Enemy cách camera từ 6 đến 9 đơn vị

        // Tạo vị trí spawn trước mặt camera
        Vector3 spawnPosition = cameraTransform.position + forward * randomDistance;

        // Điều chỉnh Y để enemy thấp hơn camera một chút
        spawnPosition.y = cameraTransform.position.y  ;

        // Thêm random sang hai bên (trái/phải)
        spawnPosition += cameraTransform.right * Random.Range(-5f, 5f);

        return spawnPosition;
    }

    [Button]
    public void SaveLevelData()
    {
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        newLevelData.level = currentLevelIndex;
        newLevelData.map = currentMap;
        newLevelData.cameraPosition = cameraTransform.position;
        newLevelData.cameraRotation = cameraTransform.rotation;
        newLevelData.characters = new List<CharacterController>(CurrentListEnemy);
        newLevelData._characterTarget = _characterTarget;


        // Lưu scriptable object
#if UNITY_EDITOR
        string path = "Assets/_GameAssets/LevelSO/LevelTest/Level" + currentLevelIndex + ".asset";
        UnityEditor.AssetDatabase.CreateAsset(newLevelData, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("Level data saved at: " + path);
#endif
    }
}



