using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels; 
    private int currentLevelIndex = 0; 

    private GameObject currentMap;
    private GameObject currentWeapon;

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Length) return;
        // Xóa level cũ nếu có
        if (currentMap) Destroy(currentMap);
        if (currentWeapon) Destroy(currentWeapon);

        // Lấy dữ liệu level mới
        LevelData level = levels[index];

        // Spawn map
        currentMap = Instantiate(level.map, Vector3.zero, Quaternion.identity);

        // Spawn enemy
        foreach (CharacterController charactersPrefab in level.characters)
        {
            Instantiate(charactersPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        }

        // Đặt vũ khí vào camera
        // Set vị trí camera
        Camera.main.transform.position = level.cameraPosition;
        Camera.main.transform.rotation = level.cameraRotation;
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("Game Over - No more levels");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)); // Random vị trí enemy
    }
}
