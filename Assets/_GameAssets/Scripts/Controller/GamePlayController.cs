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
            tickPreview1.SetActive(true);
            tickPreview2.SetActive(false);
            StartCoroutine(WaitGameWin());
            return;
        }

        if (hitCount == 1)
        {
            firstHitEnemy = enemy;
            WaitForSecondHit = StartCoroutine(IEWaitForSecondHit(enemyIndex));
        }
        else if (hitCount == 2)
        {
            secondHitEnemy = enemy;

            // So sánh object với EnemyTarget[0] và [1]
            var ids = new HashSet<int>
{
    currentTarget.EnemyTarget[0].characterID,
    currentTarget.EnemyTarget[1].characterID
};

            tickPreview1.SetActive(ids.Contains(firstHitEnemy.characterID));
            tickPreview2.SetActive(ids.Contains(secondHitEnemy.characterID));

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

    private IEnumerator WaitGameWin()
    {
        if (WaitForSecondHit != null) StopCoroutine(WaitForSecondHit);
        CountdownTimer.InvokeStop();
        currentTargetIndex++;
        yield return new WaitForSeconds(1.5f);
        if (currentTargetIndex >= _characterTarget.Length)
        {
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
            GameManager.Instance.clickArrow = true;
            groupPreview.HideThenShow();
            LevelManager.I.currentTargetIndex = currentTargetIndex;
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


