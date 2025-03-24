using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static RootMotion.Demos.CharacterThirdPerson;

public class CharacterMove : MonoBehaviour
{
    [SerializeField] float moveRadius = 10f;
    [SerializeField] float moveInterval = 3f;
    [SerializeField] float waitTime = 3f;
    [SerializeField] float avoidDistance = 2f;
    public Animator animator;
    private Vector3 startPosition;
    private bool isCharacterMove;
    private Camera mainCamera;
    private Coroutine moveCoroutine;

    [SerializeField] float moveSpeed = 2f;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        animator.applyRootMotion = false;
        isCharacterMove = true;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        startPosition = transform.position;
        mainCamera = Camera.main;
        moveCoroutine = StartCoroutine(MoveRandomly());
    }

    public void RestartMovement()
    {
        isCharacterMove = true;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveRandomly());
    }

    public void StopMoving()
    {
        isCharacterMove = false;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        navMeshAgent.isStopped = true; // Dừng NavMeshAgent
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.ResetPath();
        StopAllCoroutines();
        animator.CrossFade(Characteranimationkey.Idel, 0, 0);
    }

    IEnumerator MoveRandomly()
    {
        while (isCharacterMove)
        {
            animator.CrossFade(Characteranimationkey.Walking, 1f, 0);
            Vector3 randomPosition = GetValidRandomPosition();
            if (randomPosition != Vector3.zero)
            {
                navMeshAgent.isStopped = false; 
                navMeshAgent.SetDestination(randomPosition);

                while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.isOnNavMesh)
                    {
                        Debug.LogError("NavMeshAgent is not on a valid NavMesh");
                        yield break;
                    }

                    // Nếu bị chặn bởi obstacle, tìm đường khác
                    if (navMeshAgent.isPathStale || navMeshAgent.remainingDistance < 0.5f)
                    {
                    Debug.Log($"sangdevisPathStale: {navMeshAgent.isPathStale}, Remaining Distance: {navMeshAgent.remainingDistance}");
                        Debug.Log("Va chạm với vật thể khác, tìm hướng khác...");
                        randomPosition = GetValidRandomPosition();
                        navMeshAgent.SetDestination(randomPosition);
                    }

                    yield return null;
                }
            }

            animator.CrossFade(Characteranimationkey.Idel, 1f, 0);
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetValidRandomPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
            randomDirection += startPosition;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
            {
                Vector3 finalPosition = hit.position;
                if (IsInView(finalPosition) && !IsNearOtherEnemies(finalPosition))
                {
                    return finalPosition;
                }
            }
        }
        return Vector3.zero;
    }

    bool IsInView(Vector3 position)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Camera trong scene!");
        }
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(position);
        return screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0;
    }

    bool IsNearOtherEnemies(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, avoidDistance);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }
}


