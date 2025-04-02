using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public int level;
    public List<CharacterController> characters;
    public GameObject map;
    public GameObject playerWeapon;  
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;
    public CharacterTarget[] _characterTarget;
    public List<NavMeshSurface> ground;

}