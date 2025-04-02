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
    public EmojiType firstHitEmoji;
    public CharacterController firstHitEnemy = null;
    public CharacterController secondHitEnemy = null;
    private float hitResetTime = 5f;
    public Coroutine resetHitCoroutine;
    public CharacterTarget[] _characterTarget;
    public int currentTargetIndex = 0;
    [SerializeField] AnimatedObject groupPreview;



    private void OnGameWin()
    {
        UIManager.I.Hide<PanelGamePlay>();
        UIManager.I.Show<PanelGameWin>();
    }

    public void OnEnemyTargetHit(CharacterController enemy)
    {
        if (currentTargetIndex >= _characterTarget.Length) return;
        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        var currentTarget = _characterTarget[currentTargetIndex];
        if (currentTarget.EnemyTarget.Any(e => e.characterID == enemy.characterID))
        {
            hitCount++;
            if (currentTarget.EnemyTarget.Count == 1)
            {
                StartCoroutine(WaitGameWin());
                return;
            }
            if (hitCount == 1)
            {
                StartCoroutine(WaitForSecondHit());
            }
            else if (hitCount == 2)
            {
                StartCoroutine(WaitGameWin());
            }
        }
    }
    private IEnumerator WaitGameWin()
    {
        yield return new WaitForSeconds(6.5f);

        currentTargetIndex++;
        hitCount = 0;
        if (currentTargetIndex >= _characterTarget.Length)
        {
            OnGameWin();
        }
        else
        {
            groupPreview.HideThenShow();
            LevelManager.I.currentTargetIndex = currentTargetIndex;
            LevelManager.I.SetUpLeveLGamePlay();
        }
    }

    private IEnumerator WaitForSecondHit()
    {
        isWaitingForSecondHit = true;
        yield return new WaitForSeconds(5f);
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


