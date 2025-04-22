using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisionHandler
{
    private CharacterController characterController;

    public BulletCollisionHandler(CharacterController controller)
    {
        this.characterController = controller;
    }

    public void HandleCollision()
    {
        if (EmojiController.I == null) return;

        EmojiType currentEmoji = EmojiController.I.currentEmoji;

        // Giao toàn bộ xử lý cho SpecialPistolLevelManager
        SpecialPistolLevelManager.I.AddCharacter(characterController, currentEmoji);

        GamePlayController.I.SetTickPreviewByEnemy(currentEmoji);
    }
}
