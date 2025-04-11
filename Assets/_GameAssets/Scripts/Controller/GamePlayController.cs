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
        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        var currentTarget = _characterTarget[currentTargetIndex];
        int enemyIndex = currentTarget.EnemyTarget.FindIndex(e => e.characterID == enemy.characterID);
        if (currentTarget.EnemyTarget.Any(e => e.characterID == enemy.characterID))
        {
            hitCount++;
            if (currentTarget.EnemyTarget.Count == 1)
            {
                tickPreview1.SetActive(true);
                StartCoroutine(WaitGameWin());
                return;
            }
            if (hitCount == 1)
            {
                WaitForSecondHit = StartCoroutine(IEWaitForSecondHit(enemyIndex));
            }
            else if (hitCount == 2 )
            {
               
                if (enemyIndex == 0)
                {
                    tickPreview1.SetActive(true);
                    
                }
                else if (enemyIndex == 1)
                {
                    tickPreview2.SetActive(true);
                   
                }
                if (secondHitEnemy == null)
                {
                    if (WaitForSecondHit != null) StopCoroutine(WaitForSecondHit);
                    WaitForSecondHit = StartCoroutine(IEWaitForSecondHit(enemyIndex));
                    hitCount = 1;
                    return;
                }
                Debug.Log("Sangdev_CharacterReset " + secondHitEnemy + firstHitEnemy);
                StartCoroutine(WaitGameWin());
            }
        }
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
        go.SetActive(false);
       
        isWaitingForSecondHit = false;
        if (hitCount < 2)
        {
            hitCount = 0;
        }
    }
    public void ResetHitState()
    {
        if (resetHitCoroutine != null) StopCoroutine(resetHitCoroutine);
        resetHitCoroutine = StartCoroutine(IEResetHitState());
    }
    private IEnumerator IEResetHitState()
    {
        yield return new WaitForSeconds(hitResetTime);
        firstHitEnemy = null;
        secondHitEnemy = null;
    }
}


