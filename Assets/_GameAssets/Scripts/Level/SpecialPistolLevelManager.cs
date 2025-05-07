using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class SpecialPistolLevelManager : WeaponGameWinHandlerBase
{
    private GamePlayController controller;
    public static SpecialPistolLevelManager I;
    public GameObject _EffectFireSpecial;
    public void Init(GamePlayController controller)
    {
        this.controller = controller;
        I = this;
    }
    private ComboEffectConfig config;


    public List<CharacterController> hitCharacters = new List<CharacterController>();
    public Vector3 lastMidpoint;
    private EmojiType? groupEmoji = null;
    private bool hasTriggeredWin = false;
    public Dictionary<CharacterController, ComboEffectConfig> originalCombos = new();
    private HashSet<EmojiType> playedCombos = new HashSet<EmojiType>();
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

            HandleFirstHit(currentEmoji.ToString(), currentEmoji);
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

            character.HideEffOne();
            // Lấy lại combo ban đầu nếu có
            if (originalCombos.TryGetValue(character, out var originalConfig))
            {
                character.animator.CrossFade(originalConfig.animName, 0, 0);
                character.SpawnEmojiEffectSingle(originalConfig.emoji);

                if (originalConfig.playMidPointEffect)
                {
                    character.PlayEffectComboMidPoint(character.transform.position, originalConfig.emoji);

                }

                character.PlaySoundFXCombo(originalConfig.emoji, character);

                if (originalConfig.playEffectCombo)
                    character.PlayEffectCombo(character, originalConfig.emoji);

                if (originalConfig.isMove)
                    character.characterMove.RestartMovement(Characteranimationkey.DevilRemaining);
            }
            else
            {
                // Nếu không có combo ban đầu, reset mặc định
                character.characterMove.RestartMovement(Characteranimationkey.Walking);
                character.HideEffOne();
            }

            //hitCharacters.Remove(character);
        }
    }

    public void HandleFirstHit(string animState, EmojiType currentEmoji)
    {
        CharacterController c = hitCharacters[0];
        c.transform.DORotateQuaternion(c.characterRotationDefault, 0.5f);
        c.animator.CrossFade(animState, 0, 0);
        c.HideEffOne();
        c.PlayEffectCombo(c, currentEmoji);
        c.SpawnEmojiEffectSingle(currentEmoji);
        c.PlaySoundFXSingle(currentEmoji, c);
    }
    private void HandleSecondHit(CharacterController newCharacter, EmojiType emoji)
    {
        CharacterController first = hitCharacters[0];
        newCharacter.characterMove.MoveTowardsEnemy(first.characterMove, emoji, (midpoint) =>
        {
            

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

            }
        });
    }
    

    private void PlayGroupCombo(EmojiType emoji)
    {
        foreach (var c in hitCharacters)
        {
            
            c.characterMove.MoveTowardsPositionSpecialLevel(lastMidpoint, emoji, (move) =>
            {
                c.HideEffOne();
                var character = move.GetComponent<CharacterController>();
                if (character == null) return;

                character.animator.CrossFade($"{emoji}2", 0, 0);
                character.SpawnEmojiEffectSingle(emoji);
                character.PlayEffectComboMidPoint(lastMidpoint, emoji);
                if (!playedCombos.Contains(emoji))
                {
                    character.PlaySoundFXCombo(emoji, character);
                    playedCombos.Add(emoji);
                }
                character.PlayEffectCombo(character, emoji);
            });
        }
        CheckWinCondition();
    }


    private void CheckWinCondition()
    {
        if (hasTriggeredWin) return;

        var allEnemies = GamePlayController.I.CurrentListEnemy;
        bool allMatched = allEnemies.All(e => hitCharacters.Contains(e));

        EmojiType currentEmoji = EmojiController.I != null ? EmojiController.I.currentEmoji : EmojiType.Love;
        bool correctEmoji = currentEmoji == GamePlayController.I.EmojiTypeTarget;

        SetTickPreviewByEnemy(currentEmoji);
        if (allMatched && correctEmoji)
        {
            hasTriggeredWin = true;
            RunSpecialEmojiAction(currentEmoji);
            Invoke(nameof(TriggerGameWin), 1.5f);
        }
    }

    private void RunSpecialEmojiAction(EmojiType emoji)
    {
        switch (emoji)
        {
            case EmojiType.Sad:

                ExtinguishFire();
                break;
            case EmojiType.Pray:
                ResetLight();
                break;
            default:
                break;
        }
    }


    private void ExtinguishFire()
    {
        EffectManager.I.HideEffectOne(TypeEffect.Eff_FireSpecial, _EffectFireSpecial);
    }
    private void ResetLight()
    {
        LightController.I.RestoreDefaultSkybox();
    }
    private void TriggerGameWin()
    {
        controller.tickTextPistolLevel.SetActive(true);
        GamePlayController.I.WaitGameWin();
        hasTriggeredWin = false;
    }
    public override void OnEnemyTargetHit(CharacterController enemy)
    {
        throw new System.NotImplementedException();
    }

    public override void SetTickPreviewByEnemy(EmojiType emoji)
    {
        var currentTarget = controller._characterTarget[controller.currentTargetIndex];
        if (emoji != currentTarget.EmojiTypeTarget)
        {
            SetTickActive(false, false);
            return;
        }
        bool allHit = true;

        foreach (var enemy in controller.CurrentListEnemy)
        {
            if (!hitCharacters.Contains(enemy))
            {
                allHit = false;
                break;
            }
        }

        SetTickActive(allHit, allHit);

    }
    private void SetTickActive(bool tick1, bool tick2)
    {
        if (controller.tickPreview1) controller.tickPreview1.SetActive(tick1);
        if (controller.tickPreview2) controller.tickPreview2.SetActive(tick2);
    }
}
