using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LevelTest : Singleton<LevelTest>
{
    [Header("For Test")]
    public int currentLevelIndex;
    public Transform cameraTransform;
    //[HideInInspector]
    public GameObject currentMap;
    public List<CharacterController> CurrentListEnemy;
    //[HideInInspector]
    public List<CharacterController> currentEnemyTargets = new List<CharacterController>();
    //[HideInInspector]
    public EmojiType currentEmojiTypeTarget;
    
    public CharacterTarget[] _characterTarget;
    public int currentTargetIndex;
    public int quantityEmojiRandom;
    public List<EmojiType> selectedEmojiTypesPerCharacter = new List<EmojiType>();

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
        currentEmojiTypeTarget = _characterTarget[currentTargetIndex].EmojiTypeTarget;
    }
    public void SetUpEnemyTarget()
    {
        currentEmojiTypeTarget = _characterTarget[currentTargetIndex].EmojiTypeTarget;
        currentEnemyTargets.Clear();

        if (currentTargetIndex < 0 || currentTargetIndex >= _characterTarget.Length)
        {
            Debug.LogWarning("currentTargetIndex out of range!");
            return;
        }
        foreach (var enemy in CurrentListEnemy)
        {
            enemy.isEnemyTarget = false;
        }
        List<CharacterController> enemyTargetList = _characterTarget[currentTargetIndex].EnemyTarget;

        foreach (CharacterController enemy in CurrentListEnemy)
        {
            // So sánh dựa trên prefab gốc
            if (enemyTargetList.Any(prefab => prefab.name == enemy.name.Replace("(Clone)", "").Trim()))
            {
                currentEnemyTargets.Add(enemy);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 forward = cameraTransform.forward.normalized;
        float randomDistance = UnityEngine.Random.Range(6f, 9f);
        Vector3 spawnPosition = cameraTransform.position + forward * randomDistance;
        spawnPosition += cameraTransform.right * UnityEngine.Random.Range(-5f, 5f);
        Vector3[] offsets = {
        Vector3.zero, Vector3.right * 2.5f, Vector3.left * 2.5f,
        Vector3.forward * 2.5f, Vector3.back * 2.5f
    };

        RaycastHit[] hits = new RaycastHit[1]; 
        foreach (var offset in offsets)
        {
            Vector3 adjustedSpawn = spawnPosition + offset;
            Vector3 topPosition = adjustedSpawn + Vector3.up * 10f;

            if (Physics.RaycastNonAlloc(topPosition, Vector3.down, hits, 25f) > 0)
            {
                if (NavMesh.SamplePosition(hits[0].point, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas))
                {
                    return navMeshHit.position;  
                }
            }
        }

        Debug.LogWarning("Không tìm thấy vị trí hợp lệ trên NavMesh!");
        return spawnPosition; 
    }

    [Button]
    public void GenerateEmojiTypeList()
    {
        selectedEmojiTypesPerCharacter = new List<EmojiType>();
        HashSet<EmojiType> usedEmojiTypes = new HashSet<EmojiType>();

        // B1: Add emoji chính từ từng character
        foreach (var character in _characterTarget)
        {
            if (usedEmojiTypes.Add(character.EmojiTypeTarget))
            {
                selectedEmojiTypesPerCharacter.Add(character.EmojiTypeTarget);
            }
        }

        // B2: Tính số emoji cần random thêm
        int totalTarget = _characterTarget.Length;
        int totalNeeded = totalTarget + quantityEmojiRandom;
        int toAdd = totalNeeded - selectedEmojiTypesPerCharacter.Count;

        // B3: Lấy danh sách emoji chưa dùng
        List<EmojiType> availableTypes = Enum.GetValues(typeof(EmojiType))
                                             .Cast<EmojiType>()
                                             .Where(e => !usedEmojiTypes.Contains(e))
                                             .ToList();

        // B4: Random đúng số lượng cần thiết
        for (int i = 0; i < toAdd && availableTypes.Count > 0; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, availableTypes.Count);
            EmojiType randomEmoji = availableTypes[randIndex];

            selectedEmojiTypesPerCharacter.Add(randomEmoji);
            usedEmojiTypes.Add(randomEmoji);
            availableTypes.RemoveAt(randIndex);
        }
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

      
        newLevelData.selectedEmojiTypesPerCharacter = selectedEmojiTypesPerCharacter;
        newLevelData.quantityEmojiRandom = quantityEmojiRandom;
        // Lưu scriptable object
#if UNITY_EDITOR
        string path = "Assets/_GameAssets/LevelSO/LevelTest/Level" + currentLevelIndex + ".asset";
        UnityEditor.AssetDatabase.CreateAsset(newLevelData, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("Level data saved at: " + path);
#endif
    }
}



