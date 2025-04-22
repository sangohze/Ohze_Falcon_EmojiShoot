using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public int level;
    public List<CharacterController> characters;
    public GameObject map;
    public WeaponType playerWeapon;  
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;
    public CharacterTarget[] _characterTarget;
    public List<EmojiType> selectedEmojiTypesPerCharacter;
    public int quantityEmojiRandom;


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

    public enum WeaponType
    {
        Bow,
        Pistol,
        Sniper,
    }
}

