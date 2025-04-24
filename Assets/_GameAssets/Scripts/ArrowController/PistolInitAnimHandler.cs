using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PistolInitAnimHandler : MonoBehaviour
{

    private Vector3 GetMidpointFromCamera(Vector3 pos1, Vector3 pos2)
    {
        var cam = Camera.main;
        if (cam == null) return (pos1 + pos2) / 2f;

        Transform camTransform = cam.transform;

        // Chuyển sang local space của camera
        Vector3 local1 = camTransform.InverseTransformPoint(pos1);
        Vector3 local2 = camTransform.InverseTransformPoint(pos2);

        // Tính trung điểm theo x (camera nhìn) và z nếu muốn
        float midLocalX = (local1.x + local2.x) / 2f;
        float midLocalZ = (local1.z + local2.z) / 2f;

        // Giữ nguyên độ cao y
        float midY = (pos1.y + pos2.y) / 2f;

        Vector3 localMid = new Vector3(midLocalX, local1.y, midLocalZ);
        Vector3 worldMid = camTransform.TransformPoint(localMid);
        worldMid.y = midY;

        return worldMid;
    }



    public void SetupEnvironmentOnce(ComboEffectConfig config, Vector3 midpoint)
    {
        if (config.dimLight && LightController.I != null)
        {
            LightController.I.SetDarkSkybox();
        }

        if (config.fireEffect && EffectManager.I != null)
        {
            SpecialPistolLevelManager.I._EffectFireSpecial =
                EffectManager.I.PlayEffect(TypeEffect.Eff_FireSpecial, midpoint);
        }
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

        Vector3 midpoint = GetMidpointFromCamera(currentListEnemies[0].transform.position, currentListEnemies[1].transform.position);
        StartCoroutine(PlayComboSequentially(currentListEnemies, midpoint, config));
    }

    private IEnumerator PlayComboSequentially(List<CharacterController> enemies, Vector3 midpoint, ComboEffectConfig config)
    {
        SetupEnvironmentOnce(config, midpoint);
        bool hasPlayedSound = false;
        foreach (var enemy in enemies)
        {
            yield return MoveAndPlay(enemy, midpoint, config, !hasPlayedSound);
            hasPlayedSound = true;
            SpecialPistolLevelManager.I.originalCombos[enemy] = config;
        }
    }

    private IEnumerator MoveAndPlay(CharacterController character, Vector3 midpoint, ComboEffectConfig config, bool playSound)
    {
        bool finished = false;

        character.characterMove.MoveTowardsPositionSpecialLevel(midpoint, config.emoji, (move) =>
        {
            character.HideEffOne();

            var movedChar = move.GetComponent<CharacterController>();
            if (movedChar == null) return;

            movedChar.animator.CrossFade(config.animName, 0, 0);
            movedChar.SpawnEmojiEffectSingle(config.emoji);

            if (config.playMidPointEffect)
                movedChar.PlayEffectComboMidPoint(midpoint, config.emoji);
            if (playSound) // chỉ play sound một lần
                movedChar.PlaySoundFXCombo(config.emoji, movedChar);

            if (config.playEffectCombo)
                movedChar.PlayEffectCombo(movedChar, config.emoji);
            if (config.isMove)
                movedChar.characterMove.RestartMovement(Characteranimationkey.DevilRemaining);
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
                    isMove = true,
                    fireEffect = true,
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
                    isMove = true,
                    dimLight = true,
                    emoji = EmojiType.Sad,
                    animName = "running scare",
                    playMidPointEffect = false,
                    playEffectCombo = false
                };
            case EmojiType.Devil:
                return new ComboEffectConfig
                {
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
    public bool fireEffect = false;
    public bool isMove = false;
   
    public ComboEffectConfig(EmojiType emoji)
    {
        this.emoji = emoji;
        this.animName = $"{emoji}2";
    }

    public ComboEffectConfig() { }
}
