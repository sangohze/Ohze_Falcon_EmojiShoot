using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using static LevelData;

public class GamePlayController : Singleton<GamePlayController>
{
    public List<CharacterController> CurrentListEnemy;
    private int hitCount = 0;
    private bool isWaitingForSecondHit = false;
    public EmojiType EmojiTypeTarget;
    public EmojiType? firstHitEmoji = null;
    public CharacterController firstHitEnemy = null;
    public CharacterController secondHitEnemy = null;
    private float hitResetTime = 5f;
    public Coroutine resetHitCoroutine;
    private Coroutine WaitForSecondHit;
    public CharacterTarget[] _characterTarget;
    public int currentTargetIndex = 0;
    public int currentLevelIndexText;
    [SerializeField] GameObject tickPreview1;
    [SerializeField] GameObject tickPreview2;
    private float timeToTarget = 10f;
    [SerializeField] UIMoveNew groupMissionShow;
    [SerializeField] DotGroupController groupDOT;

    [SerializeField] GameObject _Effects;
    public bool isTransitioningMission = false;

    private IWeaponGameWinHandler weaponGameWinHandler;
    public void InitWeaponLogic(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Bow:
                weaponGameWinHandler = new BowGameWinHandler(this);
                break;
            case WeaponType.Pistol:
                weaponGameWinHandler = new SpecialPistolLevelManager(this);
                break;
        }
    }
    private void OnGameWin()
    {
        UIManager.I.Show<PanelGameWin>();
    }

    public void OnEnemyTargetHit(CharacterController enemy)
    {
        if (currentTargetIndex >= _characterTarget.Length) return;

        var currentTarget = _characterTarget[currentTargetIndex];
        int enemyIndex = currentTarget.EnemyTarget.FindIndex(e => e.characterID == enemy.characterID);

        if (enemyIndex < 0) return; // Không phải enemy trong danh sách target

        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        hitCount++;

        if (currentTarget.EnemyTarget.Count == 1)
        {
            SetTickPreviewByEnemy(EmojiController.I.currentEmoji); 
            StartCoroutine(IEWaitGameWin());
            return;
        }

        if (hitCount == 1)
        {
            WaitForSecondHit = StartCoroutine(IEWaitForSecondHit(enemyIndex));
        }
        else if (hitCount == 2)
        {
            CheckEnemyTargetGameWin(enemyIndex);
        }

    }


    public void SetTickPreviewByEnemy(EmojiType emojiType)
    {
        if (isTransitioningMission) return;

        if (currentTargetIndex >= _characterTarget.Length) return;

        var currentTarget = _characterTarget[currentTargetIndex];

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
        bool match1 = IsMatchedEnemy(currentTarget.EnemyTarget, 0, firstHitEnemy, secondHitEnemy);
        bool match2 = IsMatchedEnemy(currentTarget.EnemyTarget, 1, firstHitEnemy, secondHitEnemy);

        SetTickActive(match1, match2);
    }

    private void SetTickActive(bool tick1, bool tick2)
    {
        if (tickPreview1) tickPreview1.SetActive(tick1);
        if (tickPreview2) tickPreview2.SetActive(tick2);
    }

    private void CheckEnemyTargetGameWin(int enemyIndex)
    {
        hitCount = 1;

        if (WaitForSecondHit != null)
            StopCoroutine(WaitForSecondHit);

        WaitForSecondHit = StartCoroutine(IEWaitForSecondHit(enemyIndex));

        if (!firstHitEnemy.isEnemyTarget || !secondHitEnemy.isEnemyTarget)
            return;

        StartCoroutine(IEWaitGameWin());
    }

    private bool IsMatchedEnemy(List<CharacterController> enemyList, int index, CharacterController first, CharacterController second)
    {
        if (enemyList.Count <= index) return false;
        var target = enemyList[index];
        if (target == null) return false;

        return (first != null && first.characterID == target.characterID) ||
               (second != null && second.characterID == target.characterID);
    }

    public void WaitGameWin()
    {
        StartCoroutine(IEWaitGameWin());
    }    

    private IEnumerator IEWaitGameWin()
    {
        isTransitioningMission = true;
        currentTargetIndex++;
        if (currentTargetIndex >= _characterTarget.Length)
        {
            _Effects.SetActive(true);
        }
        else
        {
            firstHitEmoji = null;
            secondHitEnemy = null;
        }
        if (WaitForSecondHit != null) StopCoroutine(WaitForSecondHit);
        yield return new WaitForSeconds(1.5f);
        if (currentTargetIndex >= _characterTarget.Length)
        {
            CountdownTimer.InvokeStop();
            GameManager.Instance.clickArrow = false;
            UIManager.I.Get<PanelGamePlay>().gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        hitCount = 0;
        if (currentTargetIndex >= _characterTarget.Length)
        {
            OnGameWin();
        }
        else
        {
            tickPreview1.SetActive(false);
            tickPreview2.SetActive(false);
            groupMissionShow.Hide();
            yield return new WaitForSeconds(0.2f);
            LevelManager.I.SetUpLeveLGamePlay();
            GameManager.Instance.clickArrow = true;
            if (WaitForSecondHit != null)
                StopCoroutine(WaitForSecondHit);
        }
        groupDOT.SetCurrentTargetIndex(currentTargetIndex);
        isTransitioningMission = false;
    }

    private IEnumerator IEWaitForSecondHit(int enemyIndex)
    {
        GameObject go = null;
        if (enemyIndex == 0)
        {
            tickPreview1.SetActive(true);
            go = tickPreview1;
        }
        else if (enemyIndex == 1)
        {
            tickPreview2.SetActive(true);
            go = tickPreview2;
        }
        isWaitingForSecondHit = true;
        yield return new WaitForSeconds(timeToTarget);
       tickPreview1.SetActive(false);
        tickPreview2.SetActive(false);

        isWaitingForSecondHit = false;
        if (hitCount < 2)
        {
            hitCount = 0;
        }
    }

}


