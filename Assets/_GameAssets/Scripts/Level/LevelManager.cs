using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<LevelData> levels;
    private CharacterController enemyTarget;

    void Start()
    {
        // Initialize the level (for example, level 0)
        InitializeLevel(0);
    }

    public void InitializeLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError("Invalid level index");
            return;
        }

        LevelData levelData = levels[levelIndex];

        // Instantiate characters for the level
        foreach (CharacterController character in levelData.characters)
        {
            Instantiate(character, character.transform.position, character.transform.rotation);
        }

        // Select a random character as the enemy target
        if (levelData.characters.Count > 0)
        {
            int randomIndex = Random.Range(0, levelData.characters.Count);
            enemyTarget = levelData.characters[randomIndex];
            enemyTarget.SetAsEnemyTarget();
        }
    }
}
