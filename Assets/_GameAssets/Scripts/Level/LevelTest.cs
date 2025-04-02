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
    public GameObject currentMap;
    public List<CharacterController> CurrentListEnemy;
    public List<CharacterController> currentEnemyTargets = new List<CharacterController>();
    public EmojiType currentEmojiTypeTarget;
    public NavMeshSurface groud;
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
        float randomDistance = Random.Range(6f, 9f);

        // Tạo vị trí spawn dự kiến
        Vector3 spawnPosition = cameraTransform.position + forward * randomDistance;

        // Thêm ngẫu nhiên sang trái/phải
        spawnPosition += cameraTransform.right * Random.Range(-5f, 5f);

        // Tìm vị trí hợp lệ trên NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 25f, NavMesh.AllAreas))
        {
            return hit.position;  // Trả về vị trí hợp lệ trên NavMesh
        }
        else
        {
            Debug.LogWarning("Không tìm thấy vị trí hợp lệ trên NavMesh!");
            return spawnPosition;  // Trả về vị trí gốc nếu không tìm thấy
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
        newLevelData.groud = groud;

        // Lưu scriptable object
#if UNITY_EDITOR
        string path = "Assets/_GameAssets/LevelSO/LevelTest/Level" + currentLevelIndex + ".asset";
        UnityEditor.AssetDatabase.CreateAsset(newLevelData, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("Level data saved at: " + path);
#endif
    }
}



