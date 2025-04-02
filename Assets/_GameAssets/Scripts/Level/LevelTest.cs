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
    public List<NavMeshSurface> ground;
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
        Vector3 forward = cameraTransform.forward.normalized;
        float randomDistance = Random.Range(6f, 9f);
        Vector3 spawnPosition = cameraTransform.position + forward * randomDistance;
        spawnPosition += cameraTransform.right * Random.Range(-5f, 5f);

        // Danh sách offset kiểm tra
        Vector3[] offsets = {
        Vector3.zero, Vector3.right * 2.5f, Vector3.left * 2.5f,
        Vector3.forward * 1.5f, Vector3.back * 1.5f
    };

        RaycastHit[] hits = new RaycastHit[1]; // Dùng NonAlloc để tối ưu
        foreach (var offset in offsets)
        {
            Vector3 adjustedSpawn = spawnPosition + offset;
            Vector3 topPosition = adjustedSpawn + Vector3.up * 10f;

            if (Physics.RaycastNonAlloc(topPosition, Vector3.down, hits, 25f) > 0)
            {
                if (NavMesh.SamplePosition(hits[0].point, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas))
                {
                    return navMeshHit.position;  // Trả về vị trí hợp lệ trên NavMesh
                }
            }
        }

        Debug.LogWarning("Không tìm thấy vị trí hợp lệ trên NavMesh!");
        return spawnPosition; // Trả về vị trí gốc nếu không tìm thấy NavMesh
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
        newLevelData.ground = ground;

        // Lưu scriptable object
#if UNITY_EDITOR
        string path = "Assets/_GameAssets/LevelSO/LevelTest/Level" + currentLevelIndex + ".asset";
        UnityEditor.AssetDatabase.CreateAsset(newLevelData, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("Level data saved at: " + path);
#endif
    }
}



