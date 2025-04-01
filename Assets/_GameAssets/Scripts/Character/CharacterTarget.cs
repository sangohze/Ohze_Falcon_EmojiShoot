using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
   

    [PreviewField] public Sprite PreviewCharaterTarget;  // Sprite của EnemyTarget[0]
    [PreviewField] public Sprite PreviewEmojiTarget;     // Sprite của EmojiTypeTarget
    [PreviewField] public Sprite PreviewCharaterTarget2; // Sprite của EnemyTarget[1]
    private Dictionary<EmojiType, Sprite> emojiSpriteMapSingle = new Dictionary<EmojiType, Sprite>();
    private Dictionary<EmojiType, Sprite> emojiSpriteMapCombo = new Dictionary<EmojiType, Sprite>();

    [Button("Cập Nhật Sprite")]
    
    public void UpdatePreviewSprites()
    {
        InitializeEmojiMap();
        if (EnemyTarget.Count > 0 && EnemyTarget[0] != null)
        {
            PreviewCharaterTarget = EnemyTarget[0].Avatar;
        }

        if (EnemyTarget.Count > 1 && EnemyTarget[1] != null)
        {
            PreviewCharaterTarget2 = EnemyTarget[1].Avatar;
        }
        Dictionary<EmojiType, Sprite> selectedEmojiMap = (EnemyTarget.Count > 1) ? emojiSpriteMapCombo : emojiSpriteMapSingle;
        if (selectedEmojiMap.TryGetValue(EmojiTypeTarget, out Sprite sprite))
        {
            PreviewEmojiTarget = sprite;
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
