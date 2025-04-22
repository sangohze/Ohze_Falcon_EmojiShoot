using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialPistolLevelManager : Singleton<SpecialPistolLevelManager> , IWeaponGameWinHandler
{
    
    public List<CharacterController> hitCharacters = new List<CharacterController>();
    private Vector3 lastMidpoint;
    private EmojiType? groupEmoji = null;
    public void AddCharacter(CharacterController character, EmojiType currentEmoji)
    {
        if (groupEmoji != null && groupEmoji != currentEmoji)
        {
            // Reset thằng vừa bắn (nếu từng bị dính trước đó)
            if (hitCharacters.Contains(character))
            {
                character.characterMove.RestartMovement(Characteranimationkey.Walking);
                character.HideEffOne();
                hitCharacters.Remove(character);
            }

            // Clear list và cập nhật emoji mới
            hitCharacters.Clear();
            groupEmoji = null;
        }

        // Set emoji đầu tiên
        if (groupEmoji == null)
        {
            groupEmoji = currentEmoji;
        }
        if (hitCharacters.Contains(character)) return;

        hitCharacters.Add(character);
        character.StopCoroutineResetMovement();
        character.characterMove.StopMoving();
        character.StopCoroutineResetAfterDelayPistol();
        //character.StartCoroutine(character.PlayEffectCombo(character, currentEmoji));
        if (hitCharacters.Count == 1)
        {

            character.HandleFirstHit(currentEmoji.ToString(), currentEmoji);
        }
        else if (hitCharacters.Count == 2)
        {
            HandleSecondHit(character, currentEmoji);
        }
        else
        {
            PlayGroupCombo(currentEmoji);
        }

        // Reset after 10s
        character.ResetAfterDelayPistol = character.StartCoroutine(ResetAfterDelay(character));
        CheckWinCondition();
    }

    private IEnumerator ResetAfterDelay(CharacterController character)
    {
        if (hitCharacters.Contains(character))
        {
        yield return new WaitForSeconds(character.timeEndAnim);
            character.characterMove.RestartMovement(Characteranimationkey.Walking);
            character.HideEffOne();
            hitCharacters.Remove(character);
        }
    }

    private void HandleSecondHit(CharacterController newCharacter, EmojiType emoji)
    {
        CharacterController first = hitCharacters[0];
        newCharacter.characterMove.MoveTowardsEnemy(first.characterMove, emoji, (midpoint) =>
        {
            lastMidpoint = midpoint; // 👉 Lưu lại để dùng cho >= 3 người

            first.HideEffOne();
            newCharacter.HideEffOne();

            first.animator.CrossFade($"{emoji}2", 0, 0);
            newCharacter.animator.CrossFade($"{emoji}2", 0, 0);

            newCharacter.PlayEffectComboMidPoint(midpoint, emoji);
            newCharacter.PlaySoundFXCombo(emoji, newCharacter);
            newCharacter.SpawnEmojiEffectSingle(emoji);
            newCharacter.PlayEffectCombo(newCharacter, emoji);
            first.SpawnEmojiEffectSingle(emoji);
            first.PlayEffectCombo(first, emoji);


            if (newCharacter.isEnemyTarget && emoji == GamePlayController.I.EmojiTypeTarget)
            {
                HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                GamePlayController.I.OnEnemyTargetHit(newCharacter);
            }
        });
    }


    private void PlayGroupCombo(EmojiType emoji)
    {
        foreach (var c in hitCharacters)
        {
            c.HideEffOne();

            c.characterMove.MoveTowardsPosition(lastMidpoint, emoji, (move) =>
            {
                var character = move.GetComponent<CharacterController>();
                if (character == null) return;

                character.animator.CrossFade($"{emoji}2", 0, 0);
                character.SpawnEmojiEffectSingle(emoji);
                character.PlayEffectComboMidPoint(lastMidpoint, emoji);
                character.PlayEffectCombo(character, emoji);
            });
        }
        CheckWinCondition();
    }

    private bool hasTriggeredWin = false;

    private void CheckWinCondition()
    {
        if (hasTriggeredWin) return;

        var allEnemies = GamePlayController.I.CurrentListEnemy;
        bool allMatched = allEnemies.All(e => hitCharacters.Contains(e));

        EmojiType currentEmoji = EmojiController.I != null ? EmojiController.I.currentEmoji : EmojiType.Love;
        bool correctEmoji = currentEmoji == GamePlayController.I.EmojiTypeTarget;

        if (allMatched && correctEmoji)
        {
            hasTriggeredWin = true;
            GamePlayController.I.WaitGameWin();
        }
    }
}
