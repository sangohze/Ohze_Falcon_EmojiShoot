using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : Singleton<GamePlayController>
{
    [SerializeField] private List<CharacterController> enemyTargets ;
    private int hitCount = 0;
    private bool isWaitingForSecondHit = false;
    public EmojiType EmojiTypeTarget;
  

    private void OnEnable()
    {
        // _onGameInit.OnEventRaised += OnGameInit;
        // _onGameActive.OnEventRaised += OnGameActive;
        // _onGameWin.OnEventRaised += OnGameWin;
        
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
        // _onGameInit.OnEventRaised -= OnGameInit;
        // _onGameActive.OnEventRaised -= OnGameActive;
        // _onGameWin.OnEventRaised -= OnGameWin;
    }

    private void OnGameInit()
    {
        // Initialize game logic here
    }

    private void OnGameActive()
    {
        // Activate game logic here
    }

    private void OnGameWin()
    {
        Debug.Log("You win the game!");
        UIManager.I.Hide<PanelGamePlay>();
        UIManager.I.Show<PanelGameWin>();
    }

 

    public void OnEnemyHit(CharacterController enemy)
    {
        Debug.Log("You win" + enemy.name);
        if (enemyTargets.Contains(enemy))
        {
            Debug.Log("You win the game!0");
            hitCount++;
            if (enemyTargets.Count == 1)
            {
                StartCoroutine(WaiGameWin());
                return;
            }
            if (hitCount == 1)
            {
                StartCoroutine(WaitForSecondHit());
            }
            else if (hitCount == 2)
            {

                StartCoroutine(WaiGameWin());
            }
        }
    }
    private IEnumerator WaiGameWin()
    {
        Debug.LogError("YouWIN");
        yield return new WaitForSeconds(3f);
        OnGameWin();
    }

    private IEnumerator WaitForSecondHit()
    {
        isWaitingForSecondHit = true;
        yield return new WaitForSeconds(3f);
        isWaitingForSecondHit = false;

        if (hitCount < 2)
        {
            hitCount = 0;
            Debug.Log("Failed to hit the second enemy within the time limit.");
        }
    }
   
}


