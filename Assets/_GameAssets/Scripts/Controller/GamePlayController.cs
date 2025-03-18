using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : Singleton<GamePlayController>
{
    [SerializeField] private List<CharacterController> enemyTargets = new List<CharacterController>();
    private int hitCount = 0;
    private bool isWaitingForSecondHit = false;

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
        // Add your win game logic here
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
                Debug.Log("enemysa" + enemyTargets.Count);
                OnGameWin();
                return;
            }
            if (hitCount == 1)
            {
                StartCoroutine(WaitForSecondHit());
            }
            else if (hitCount == 2)
            {

                StartCoroutine(MoveEnemiesTogetherAndWin());
            }
        }
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
    private IEnumerator MoveEnemiesTogetherAndWin()
    {
        if (enemyTargets.Count < 2)
        {
            yield break;
        }

        CharacterController enemy1 = enemyTargets[0];
        CharacterController enemy2 = enemyTargets[1];

        Vector3 midpoint = (enemy1.transform.position + enemy2.transform.position) / 2;
        Vector3 direction1 = (midpoint - enemy1.transform.position).normalized;
        Vector3 direction2 = (midpoint - enemy2.transform.position).normalized;

        float moveSpeed = 2f;
        float distanceThreshold = 1f;

        while (Vector3.Distance(enemy1.transform.position, midpoint) > distanceThreshold &&
               Vector3.Distance(enemy2.transform.position, midpoint) > distanceThreshold)
        {
            enemy1.transform.position = Vector3.MoveTowards(enemy1.transform.position, midpoint, moveSpeed * Time.deltaTime);
            enemy2.transform.position = Vector3.MoveTowards(enemy2.transform.position, midpoint, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Make enemies face each other
        enemy1.transform.LookAt(enemy2.transform);
        enemy2.transform.LookAt(enemy1.transform);

        // Wait for a moment before winning the game
        yield return new WaitForSeconds(3f);

        OnGameWin();
    }
}


