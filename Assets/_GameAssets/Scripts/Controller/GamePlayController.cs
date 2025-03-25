using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : Singleton<GamePlayController>
{
    [SerializeField] private List<CharacterController> enemyTargets ;
    private int hitCount = 0;
    private bool isWaitingForSecondHit = false;
    public EmojiType EmojiTypeTarget;
    public EmojiType firstHitEmoji ;
    public CharacterController firstHitEnemy = null;
    public CharacterController secondHitEnemy = null;
    private float hitResetTime = 5f;
    public Coroutine resetHitCoroutine;

    private void OnEnable()
    {
            
    }

    private void Start()
    {
        SetUpLeveLGamePlay();
    }
    public void SetUpLeveLGamePlay()
    {
        enemyTargets = LevelManager.I.CurrentEnemyTargets;
        EmojiTypeTarget = LevelManager.I.emojiTypeTarget;
        foreach (var enemy in enemyTargets)
        {
            enemy.SetAsEnemyTarget();
        }
    }    


    private void OnDisable()
    {
       
    }

    private void OnGameWin()
    {
        UIManager.I.Hide<PanelGamePlay>();
        UIManager.I.Show<PanelGameWin>();
    }

    public void OnEnemyTargetHit(CharacterController enemy)
    {
        if (enemyTargets.Contains(enemy))
        {
            hitCount++;
            if (enemyTargets.Count == 1)
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
        yield return new WaitForSeconds(3.5f);
        OnGameWin();
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


