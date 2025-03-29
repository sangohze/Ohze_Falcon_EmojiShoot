using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData[] levels; 
    public int currentLevelIndex ; 

    private GameObject currentMap;
    private GameObject currentWeapon;
    public List<CharacterController> CurrentEnemyTargets = new List<CharacterController>();  
    public List<CharacterController> CurrentListEnemy;
    public EmojiType emojiTypeTarget;
    private Vector3 mappositation = new Vector3 (0,0.3f,0);
    private Vector3 mappositation2 = new Vector3(-48.82f, 0.3f, 861);



    void OnEnable()
    {
        currentLevelIndex = ES3.Load<int>("currentLevelIndex", 0);
        LoadLevel(currentLevelIndex);
    }

    public void  LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Length) return;
        CurrentEnemyTargets.Clear();
        CurrentListEnemy.Clear();

        LevelData level = levels[index];

       if(index == 1 || index == 4 || index == 7) {
            mappositation = mappositation2;
        }
        currentMap = Instantiate(level.map, mappositation, Quaternion.identity);

        // Spawn enemy
      
        emojiTypeTarget = level.EmojiTypeTarget;

        // Đặt vũ khí vào camera
        // Set vị trí camera
        Camera.main.transform.position = level.cameraPosition;
        Camera.main.transform.rotation = level.cameraRotation;
    
        Vector3 spawnPosition = new Vector3(0, 1, 0); // Chỉnh vị trí spawn phù hợp
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 2f, NavMesh.AllAreas))
        {
            foreach (CharacterController charactersPrefab in level.characters)
            {
                Quaternion spawnRot = Quaternion.Euler(0, 90, 0);
                CharacterController enemy = Instantiate(charactersPrefab, GetRandomSpawnPosition(), spawnRot);
                CurrentListEnemy.Add(enemy);
                if (level.charactersTarget.Contains(charactersPrefab))
                {
                    CurrentEnemyTargets.Add(enemy);
                }
            }      
           
        }
        else
        {
            Debug.LogError("Không tìm thấy vị trí hợp lệ trên NavMesh!");
        }
       
    }

    public void NextLevel()
    {
        currentLevelIndex++;    
        ES3.Save("currentLevelIndex", currentLevelIndex);
        if (currentLevelIndex < levels.Length)
        {
            //LoadLevel(currentLevelIndex);
        }
        else
        {
            currentLevelIndex = 0;
            //LoadLevel(currentLevelIndex);
            ES3.Save("currentLevelIndex", currentLevelIndex);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-21, -24), 0.2f, Random.Range(-10, 10)); // Random vị trí enemy
    }
}
