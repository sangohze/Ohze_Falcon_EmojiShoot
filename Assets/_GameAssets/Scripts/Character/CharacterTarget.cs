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
    [SerializeField] private EmojiDatabase emojiDatabase;

    [PreviewField] public Sprite PreviewCharaterTarget;  // Sprite của EnemyTarget[0]
    [PreviewField] public Sprite PreviewCharaterTarget2; // Sprite của EnemyTarget[1]
    [PreviewField] public Sprite PreviewEmojiTarget;     // Sprite của EmojiTypeTarget

    [Button("Cập Nhật Sprite")]
    public void UpdatePreviewSprites()
    {
        if (EnemyTarget.Count > 0 && EnemyTarget[0] != null)
        {
            PreviewCharaterTarget = EnemyTarget[0].Avatar;
        }

        if (EnemyTarget.Count > 1 && EnemyTarget[1] != null)
        {
            PreviewCharaterTarget2 = EnemyTarget[1].Avatar;
        }

        // Nếu có EmojiTypeTarget, cập nhật PreviewEmojiTarget
        PreviewEmojiTarget = EmojiSpriteDatabase.GetSprite(EmojiTypeTarget);
    }
}
