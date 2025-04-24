using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataBatch", menuName = "Game/LevelDataBatch")]
public class LevelDataBatch : ScriptableObject
{
    public List<LevelData> generatedLevels;
}