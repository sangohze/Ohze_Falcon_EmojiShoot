using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class PistolInitAnimHandler : MonoBehaviour
{
    private CharacterController Controller;

    public void Initialize(CharacterController controller)
    {
        this.Controller = controller;
    }

    public void InitCharacterHandler()
    {
        EmojiType currentEmoji = GamePlayController.I.EmojiTypeTarget;
        var config = GetComboConfig(currentEmoji);
        StartGroupCombo(config);
    }

    public void StartGroupCombo(ComboEffectConfig config)
    {
        var currentListEnemies = GamePlayController.I.CurrentListEnemy;
        if (currentListEnemies == null || currentListEnemies.Count < 2) return;

        Vector3 midpoint = (currentListEnemies[0].transform.position + currentListEnemies[1].transform.position) / 2;
        StartCoroutine(PlayComboSequentially(currentListEnemies, midpoint, config));
    }

    private IEnumerator PlayComboSequentially(List<CharacterController> enemies, Vector3 midpoint, ComboEffectConfig config)
    {
        foreach (var enemy in enemies)
        {
            yield return MoveAndPlay(enemy, midpoint, config);
            yield return new WaitForSeconds(0.5f); // delay giữa mỗi lượt combo
        }
    }

    private IEnumerator MoveAndPlay(CharacterController character, Vector3 midpoint, ComboEffectConfig config)
    {
        bool finished = false;

        Controller.characterMove.MoveTowardsPosition(midpoint, config.emoji, (move) =>
        {
            Controller.HideEffOne();

            var movedChar = move.GetComponent<CharacterController>();
            if (movedChar == null) return;

            movedChar.animator.CrossFade(config.animName, 0, 0);
            movedChar.SpawnEmojiEffectSingle(config.emoji);

            if (config.playMidPointEffect)
                movedChar.PlayEffectComboMidPoint(midpoint, config.emoji);

            movedChar.PlaySoundFXCombo(config.emoji, movedChar);

            if (config.playEffectCombo)
                movedChar.PlayEffectCombo(movedChar, config.emoji);
            if (config.dimLight && LightController.I != null)
            {
                LightController.I.SetDarkSkybox();
            }
            finished = true;
        });

        yield return new WaitUntil(() => finished);
    }

    private ComboEffectConfig GetComboConfig(EmojiType currentEmoji)
    {
        switch (currentEmoji)
        {
            case EmojiType.Love:
                return new ComboEffectConfig(EmojiType.Angry);
            case EmojiType.Sad:
                return new ComboEffectConfig
                {
                    emoji = EmojiType.Sad,
                    animName = "running scare",
                    playMidPointEffect = false,
                    playEffectCombo = false
                };
            case EmojiType.Angry:
                return new ComboEffectConfig(EmojiType.Love);
            case EmojiType.Pray:
                return new ComboEffectConfig
                {
                    emoji = EmojiType.Sad,
                    animName = "running scare",
                    playMidPointEffect = false,
                    playEffectCombo = false
                };
            case EmojiType.Devil:
                return new ComboEffectConfig
                {
                    dimLight = true,
                    emoji = EmojiType.Pray,
                    animName = "Pray",
                    playMidPointEffect = false,
                    playEffectCombo = false
                };
            case EmojiType.Dance:
                return new ComboEffectConfig(EmojiType.Sad);
            default:
                return new ComboEffectConfig(currentEmoji);
        }
    }
}
[Serializable]
public class ComboEffectConfig
{
    public EmojiType emoji;
    public string animName;
    public bool playMidPointEffect = true;
    public bool playEffectCombo = true;
    public bool dimLight = false;
    public ComboEffectConfig(EmojiType emoji)
    {
        this.emoji = emoji;
        this.animName = $"{emoji}2";
    }

    public ComboEffectConfig() { }
}
