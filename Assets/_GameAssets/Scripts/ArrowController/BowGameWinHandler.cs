using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BowGameWinHandler : WeaponGameWinHandlerBase
{
    private GamePlayController controller;
  
  

    public BowGameWinHandler(GamePlayController controller)
    {
        this.controller = controller;
    }

    public override void SetTickPreviewByEnemy(EmojiType emojiType)
    {
        if (controller.isTransitioningMission) return;

        if (controller.currentTargetIndex >= controller._characterTarget.Length) return;

        var currentTarget = controller._characterTarget[controller.currentTargetIndex];

        // 1. Emoji không khớp -> tắt tick
        if (emojiType != currentTarget.EmojiTypeTarget)
        {
            SetTickActive(false, false);
            return;
        }

        // 2. Không có target enemy -> tắt tick
        if (currentTarget.EnemyTarget == null || currentTarget.EnemyTarget.Count == 0)
        {
            SetTickActive(false, false);
            return;
        }

        // 3. Kiểm tra match
        bool match1 = IsMatchedEnemy(currentTarget.EnemyTarget, 0, controller.firstHitEnemy, controller.secondHitEnemy);
        bool match2 = IsMatchedEnemy(currentTarget.EnemyTarget, 1, controller.firstHitEnemy, controller.secondHitEnemy);

        SetTickActive(match1, match2);
    }

    public override void OnEnemyTargetHit(CharacterController enemy)
    {
        if (controller.currentTargetIndex >= controller._characterTarget.Length) return;

        var currentTarget = controller._characterTarget[controller.currentTargetIndex];
        int enemyIndex = currentTarget.EnemyTarget.FindIndex(e => e.characterID == enemy.characterID);

        if (enemyIndex < 0) return; // Không phải enemy trong danh sách target

        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        controller.hitCount++;

        if (currentTarget.EnemyTarget.Count == 1)
        {
            SetTickPreviewByEnemy(EmojiController.I.currentEmoji);
            GamePlayController.I.WaitGameWin();
            return;
        }

        if (controller.hitCount == 1)
        {
            controller.WaitForSecondHit = controller.StartCoroutine(controller.IEWaitForSecondHit(enemyIndex));
        }
        else if (controller.hitCount == 2)
        {
            CheckEnemyTargetGameWin(enemyIndex);
        }

    }

    public  void CheckEnemyTargetGameWin(int enemyIndex)
    {
        controller.hitCount = 1;

        if (controller.WaitForSecondHit != null)
        {
            controller.StopCoroutine(controller.WaitForSecondHit);
        }    

        controller.WaitForSecondHit = controller.StartCoroutine(controller.IEWaitForSecondHit(enemyIndex));

        if (!controller.firstHitEnemy.isEnemyTarget || !controller.secondHitEnemy.isEnemyTarget)
            return;

        controller.StartCoroutine(controller.IEWaitGameWin());
    }

  

    private bool IsMatchedEnemy(List<CharacterController> enemyList, int index, CharacterController first, CharacterController second)
    {
        if (enemyList.Count <= index) return false;
        var target = enemyList[index];
        return (first != null && first.characterID == target.characterID) ||
               (second != null && second.characterID == target.characterID);
    }

    private void SetTickActive(bool tick1, bool tick2)
    {
        if (controller.tickPreview1) controller.tickPreview1.SetActive(tick1);
        if (controller.tickPreview2) controller.tickPreview2.SetActive(tick2);
    }
}
