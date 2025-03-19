using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : Singleton<GamePlayController>
{
    [SerializeField] private List<CharacterController> enemyTargets = new List<CharacterController>();
    private int hitCount = 0;
    private bool isWaitingForSecondHit = false;
    public EmojiType EmojiTypeTarget;

    private void OnEnable()
    {
        // _onGameInit.OnEventRaised += OnGameInit;
        // _onGameActive.OnEventRaised += OnGameActive;
        // _onGameWin.OnEventRaised += OnGameWin;
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

    void Start()
    {
        // Set all characters in enemyTargets as enemy targets
        foreach (var enemy in enemyTargets)
        {
            enemy.SetAsEnemyTarget();
        }
    }

    public void OnEnemyHit(CharacterController enemy)
    {
        if (enemyTargets.Contains(enemy))
        {
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


