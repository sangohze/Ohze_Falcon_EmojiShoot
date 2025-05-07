using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelData;

[System.Serializable]
public class CharacterTarget
{
    public EmojiType EmojiTypeTarget;

    [SerializeField]
    private List<CharacterController> _enemyTarget = new List<CharacterController>();
    public List<CharacterController> EnemyTarget
    {
        get => _enemyTarget;
        set => _enemyTarget = value;
    }


     public Sprite PreviewCharaterTarget;  // Sprite của EnemyTarget[0]
     public Sprite PreviewEmojiTarget;     // Sprite của EmojiTypeTarget
     public Sprite PreviewCharaterTarget2; // Sprite của EnemyTarget[1]
    private Dictionary<EmojiType, Sprite> emojiSpriteMapSingle = new Dictionary<EmojiType, Sprite>();
    private Dictionary<EmojiType, Sprite> emojiSpriteMapCombo = new Dictionary<EmojiType, Sprite>();
    private Sprite PistolPreviewAva;
    public string PistolLevelTextMission;

    private Dictionary<EmojiType, string> emojiTypeToMissionText = new Dictionary<EmojiType, string>()
{
    { EmojiType.Love, "Make love, not war." },
    { EmojiType.Angry, "Make war, not love." },
    { EmojiType.Sad, "Fire...! Help!" },
    { EmojiType.Pray, "In god, we trust" },
    { EmojiType.Devil, "No more heaven" },
    { EmojiType.Dance, "More passion, more footwork, more energy." },

    // Thêm các emoji khác tương ứng
};
    public void UpdatePreviewSprites(WeaponType weaponType)
    {
        InitializeEmojiMap();
        Dictionary<EmojiType, Sprite> selectedEmojiMap = (EnemyTarget.Count > 1 || EnemyTarget.Count == 0) ? emojiSpriteMapCombo : emojiSpriteMapSingle;
        if (selectedEmojiMap.TryGetValue(EmojiTypeTarget, out Sprite sprite))
        {
            PreviewEmojiTarget = sprite;
        }
        if (emojiTypeToMissionText.TryGetValue(EmojiTypeTarget, out string missionText))
        {
            PistolLevelTextMission = missionText;
        }
        if (weaponType == WeaponType.Pistol)
        {
            _enemyTarget = null;

            // Gán sprite đặc biệt
            PreviewCharaterTarget = PistolPreviewAva;
            PreviewCharaterTarget2 = PistolPreviewAva;

            // Emoji vẫn có thể được cập nhật nếu muốn


            return;
        }
        if (EnemyTarget.Count > 0 && EnemyTarget[0] != null)
        {
            PreviewCharaterTarget = EnemyTarget[0].Avatar;
        }

        if (EnemyTarget.Count > 1 && EnemyTarget[1] != null)
        {
            PreviewCharaterTarget2 = EnemyTarget[1].Avatar;
        }

    }
    void InitializeEmojiMap()
    {
        emojiSpriteMapSingle.Clear();
        emojiSpriteMapCombo.Clear();

        var singleSprites = EmojiController.I.spritesEmojiSingle;
        var comboSprites = EmojiController.I.spritesEmojiCombo;
        var types = (EmojiType[])Enum.GetValues(typeof(EmojiType));

        // Khởi tạo emojiSpriteMapSingle
        for (int i = 0; i < Mathf.Min(singleSprites.Count, types.Length); i++)
        {
            emojiSpriteMapSingle[types[i]] = singleSprites[i];
        }

        // Khởi tạo emojiSpriteMapCombo
        for (int i = 0; i < Mathf.Min(comboSprites.Count, types.Length); i++)
        {
            emojiSpriteMapCombo[types[i]] = comboSprites[i];
        }
    }


}
