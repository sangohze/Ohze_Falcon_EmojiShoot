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
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.ResetPath();
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }     
        //StopAllCoroutines();
        animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
    }

   IEnumerator MoveRandomly()
{
    while (isCharacterMove)
    {
        Vector3 randomPosition = GetValidRandomPosition();
            animator.CrossFade(Characteranimationkey.Walking, 0.1f, 0);
            if (randomPosition != Vector3.zero)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(randomPosition);

            bool hasStartedMoving = false;

            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.isOnNavMesh)
                {
                    yield break;
                }
                if (!hasStartedMoving && navMeshAgent.velocity.magnitude > 0.1f)
                {
                    animator.CrossFade(Characteranimationkey.Walking, 0.1f, 0);
                    hasStartedMoving = true;
                }

                yield return null;
            }

            animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
        }

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
    public void MoveTowardsEnemy(CharacterMove otherEnemy, System.Action onComplete = null)
    {
        StartCoroutine(MoveTowardsEachOther(otherEnemy, onComplete));
    }

    private IEnumerator MoveTowardsEachOther(CharacterMove otherEnemy, System.Action onComplete)
    {
        Debug.LogError("sangMoveTowardsEachOther");
        Vector3 midpoint = (transform.position + otherEnemy.transform.position) / 2;

        navMeshAgent.isStopped = false;
        otherEnemy.navMeshAgent.isStopped = false;

        navMeshAgent.SetDestination(midpoint);
        otherEnemy.navMeshAgent.SetDestination(midpoint);

        while (Vector3.Distance(transform.position, otherEnemy.transform.position) > navMeshAgent.stoppingDistance * 2)
        {
            if (navMeshAgent.velocity.magnitude > 0.1f)
            {
                animator.CrossFade(Characteranimationkey.Walking, 0.1f, 0);
            }
            else
            {
                animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
            }

            if (otherEnemy.navMeshAgent.velocity.magnitude > 0.1f)
            {
                otherEnemy.animator.CrossFade(Characteranimationkey.Walking, 0.1f, 0);
            }
            else
            {
                otherEnemy.animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
            }

            yield return null;
        }

        navMeshAgent.isStopped = true;
        otherEnemy.navMeshAgent.isStopped = true;
        transform.LookAt(otherEnemy.transform);
        otherEnemy.transform.LookAt(transform);
        animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
        onComplete?.Invoke();
    }





}


