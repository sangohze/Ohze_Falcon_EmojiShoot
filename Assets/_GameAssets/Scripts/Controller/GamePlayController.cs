using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField] AnimatedObject groupPreview;
    public int currentLevelIndexText;
    [SerializeField] GameObject tickPreview1;
    [SerializeField] GameObject tickPreview2;
    private float timeToTarget = 10f;



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
            StartCoroutine(WaitGameWin());
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

    private void CheckEnemyTargetGameWin(int enemyIndex)
    {
        hitCount = 1;

        if (WaitForSecondHit != null)
            StopCoroutine(WaitForSecondHit);

        WaitForSecondHit = StartCoroutine(IEWaitForSecondHit(enemyIndex));

        if (!firstHitEnemy.isEnemyTarget || !secondHitEnemy.isEnemyTarget)
            return;

        StartCoroutine(WaitGameWin());
    }
    public void SetTickPreviewByEnemy(EmojiType emojiType)
    {
        var currentTarget = _characterTarget[currentTargetIndex];

        // Nếu emoji truyền vào không đúng -> tắt hết tick
        if (emojiType != currentTarget.EmojiTypeTarget)
        {
            tickPreview1.SetActive(false);
            tickPreview2.SetActive(false);
            return;
        }

        if (currentTarget.EnemyTarget == null || currentTarget.EnemyTarget.Count < 1)
        {
            tickPreview1.SetActive(false);
            tickPreview2.SetActive(false);
            return;
        }

        // Chỉ có 1 enemy target
        if (currentTarget.EnemyTarget.Count < 2)
        {
            bool isValid =
                firstHitEnemy != null &&
                currentTarget.EnemyTarget[0] != null &&
                firstHitEnemy.characterID == currentTarget.EnemyTarget[0].characterID;

            tickPreview1.SetActive(isValid);
            tickPreview2.SetActive(false);
            return;
        }

        if (firstHitEnemy == null && secondHitEnemy == null)
        {
            tickPreview1.SetActive(false);
            tickPreview2.SetActive(false);
            return;
        }

        bool match1 =
            (firstHitEnemy != null &&
             currentTarget.EnemyTarget[0].characterID == firstHitEnemy.characterID) ||
            (secondHitEnemy != null &&
             currentTarget.EnemyTarget[0].characterID == secondHitEnemy.characterID);

        bool match2 =
            (firstHitEnemy != null &&
             currentTarget.EnemyTarget[1].characterID == firstHitEnemy.characterID) ||
            (secondHitEnemy != null &&
             currentTarget.EnemyTarget[1].characterID == secondHitEnemy.characterID);

        tickPreview1.SetActive(match1);
        tickPreview2.SetActive(match2);
    }



    private IEnumerator WaitGameWin()
    {
        if (WaitForSecondHit != null) StopCoroutine(WaitForSecondHit);
        currentTargetIndex++;
        yield return new WaitForSeconds(1.5f);
        if (currentTargetIndex >= _characterTarget.Length)
        {
            CountdownTimer.InvokeStop();
            GameManager.Instance.clickArrow = false;
            UIManager.I.Get<PanelGamePlay>().gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1.5f);
        hitCount = 0;
        if (currentTargetIndex >= _characterTarget.Length)
        {
            OnGameWin();
        }
        else
        {
            tickPreview1.SetActive(false);
            tickPreview2.SetActive(false);
            groupPreview.HideThenShow();
            yield return new WaitForSeconds(0.2f);
            GameManager.Instance.clickArrow = true;

            //LevelManager.I.currentTargetIndex = currentTargetIndex;
            LevelManager.I.SetUpLeveLGamePlay();
        }
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


