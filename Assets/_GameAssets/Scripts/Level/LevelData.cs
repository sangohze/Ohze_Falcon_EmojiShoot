using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public int level;
    public List<CharacterController> characters;
    public List<CharacterController> charactersTarget;
    public GameObject map;
    public GameObject playerWeapon; 
    public EmojiType EmojiTypeTarget;
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;
}