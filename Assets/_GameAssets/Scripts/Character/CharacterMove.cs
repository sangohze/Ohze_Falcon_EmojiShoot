using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CharacterMove : MonoBehaviour
{
    [SerializeField] float moveRadius = 5f;
    [SerializeField] float moveInterval = 3f;
    [SerializeField] float waitTime = 3f;
    [SerializeField] float avoidDistance = 1f;
    public Animator animator;
    private Vector3 startPosition;
    private bool isCharacterMove;
    private Camera mainCamera;
    public Coroutine moveCoroutine;


    [SerializeField] float moveSpeed = 2f;
    private NavMeshAgent navMeshAgent;
    private float originalWaitime;

    void Awake()
    {
        originalWaitime = waitTime;
        animator.applyRootMotion = false;
        isCharacterMove = true;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        startPosition = transform.position;
        mainCamera = Camera.main;
        //moveCoroutine = StartCoroutine(MoveRandomly(Characteranimationkey.Walking));
    }
    public void InitBowLevel()
    {
        moveCoroutine = StartCoroutine(MoveRandomly(Characteranimationkey.Walking));
    }

    public void RestartMovement(string animwalking)
    {
        isCharacterMove = true;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveRandomly(animwalking));
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
        //animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
    }

    IEnumerator MoveRandomly(string animwalking)
    {
        while (isCharacterMove)
        {
            Vector3 randomPosition = GetValidRandomPosition();
            animator.CrossFade(animwalking, 0.5f, 0);

            if (randomPosition != Vector3.zero)
            {
                while (!navMeshAgent.isOnNavMesh)
                    yield return null;

                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(randomPosition);

                bool hasStartedMoving = false;

                while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    // Xoay nhân vật thủ công
                    Vector3 direction = navMeshAgent.desiredVelocity;
                    if (direction.sqrMagnitude > 0.01f)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                    }

                    if (!hasStartedMoving && navMeshAgent.velocity.magnitude > 0.1f && !navMeshAgent.isStopped)
                    {
                        animator.CrossFade(animwalking, 0.1f, 0);
                        hasStartedMoving = true;
                    }

                    yield return null;
                }

                animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
            }
            if (animwalking == Characteranimationkey.DevilRemaining)
            {
                waitTime = 0;

            }
            else
            {
                waitTime = originalWaitime;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }



    Vector3 GetValidRandomPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * moveRadius;
            randomDirection += startPosition;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
            {
                Vector3 finalPosition = hit.position;
                if (/*IsInView(finalPosition) &&*/ !IsNearOtherEnemies(finalPosition))
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
    public void MoveTowardsEnemy(CharacterMove otherEnemy, EmojiType emojitype, System.Action<Vector3> onComplete = null)
    {
        StartCoroutine(MoveTowardsEachOther(otherEnemy, emojitype, onComplete));
    }

    private IEnumerator MoveTowardsEachOther(CharacterMove otherEnemy, EmojiType emojitype, System.Action<Vector3> onComplete)
    {
        Vector3 midpoint = (transform.position + otherEnemy.transform.position) / 2;

        var distance = new Dictionary<EmojiType, float>
    {
        { EmojiType.Love, 2.2f },
        { EmojiType.Sad,  3 },
        { EmojiType.Angry,3},
        { EmojiType.Pray, 3 },
        { EmojiType.Devil, 3 },
        { EmojiType.Dance, 6},
        { EmojiType.Vomit, 5 },
        { EmojiType.Talkative, 4 },
        { EmojiType.Scared, 4 },
        { EmojiType.Shit, 6 },
    };
        float multiplier = distance[emojitype];
        float stoppingRange = navMeshAgent.stoppingDistance * multiplier;

        float currentDistance = Vector3.Distance(transform.position, otherEnemy.transform.position);

        // Nếu đang quá gần, thì lùi ra trước
        if (currentDistance < stoppingRange)
        {
            Vector3 awayDir = (transform.position - otherEnemy.transform.position).normalized;
            Vector3 selfTarget = transform.position + awayDir * (stoppingRange - currentDistance + 0.5f); // +0.5 để chắc chắn đủ khoảng cách
            Vector3 otherTarget = otherEnemy.transform.position - awayDir * (stoppingRange - currentDistance + 0.5f);

            navMeshAgent.SetDestination(selfTarget);
            otherEnemy.navMeshAgent.SetDestination(otherTarget);

            animator.CrossFade(Characteranimationkey.Walking, 0f, 0);
            otherEnemy.animator.CrossFade(Characteranimationkey.Walking, 0f, 0);

            // Chờ đến khi cả hai đã cách xa nhau
            while (Vector3.Distance(transform.position, otherEnemy.transform.position) < stoppingRange)
            {
                yield return null;
            }
        }

        // Bắt đầu di chuyển lại gần midpoint nếu chưa gần đủ
        float selfToMid = Vector3.Distance(transform.position, midpoint);
        float otherToMid = Vector3.Distance(otherEnemy.transform.position, midpoint);

        if (selfToMid <= stoppingRange && otherToMid <= stoppingRange)
        {
            navMeshAgent.isStopped = true;
            otherEnemy.navMeshAgent.isStopped = true;

            transform.LookAt(otherEnemy.transform);
            otherEnemy.transform.LookAt(transform);

            onComplete?.Invoke(midpoint);
            yield break;
        }

        navMeshAgent.isStopped = false;
        otherEnemy.navMeshAgent.isStopped = false;

        StopMoving();
        otherEnemy.StopCoroutine(moveCoroutine); // Đảm bảo không bị conflict

        navMeshAgent.SetDestination(midpoint);
        otherEnemy.navMeshAgent.SetDestination(midpoint);

        animator.CrossFade(Characteranimationkey.Walking, 0f, 0);
        otherEnemy.animator.CrossFade(Characteranimationkey.Walking, 0f, 0);

        while (Vector3.Distance(transform.position, otherEnemy.transform.position) > stoppingRange)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        otherEnemy.navMeshAgent.isStopped = true;

        transform.LookAt(otherEnemy.transform);
        otherEnemy.transform.LookAt(transform);

        onComplete?.Invoke(midpoint);
    }


    //
    public void MoveTowardsPositionSpecialLevel(Vector3 targetPosition, EmojiType emojiType, Action<CharacterMove> onComplete = null)
    {
        StartCoroutine(MoveToPositionCoroutine(targetPosition, emojiType, onComplete));
    }

    private IEnumerator MoveToPositionCoroutine(Vector3 targetPosition, EmojiType emojiType, Action<CharacterMove> onComplete)
    {
        var distance = new Dictionary<EmojiType, float>
    {
        { EmojiType.Love, 2.2f },
        { EmojiType.Sad,  3 },
        { EmojiType.Angry,3 },
        { EmojiType.Pray, 3 },
        { EmojiType.Devil, 3 },
        { EmojiType.Dance, 6 },
        { EmojiType.Vomit, 5 },
        { EmojiType.Talkative, 4 },
        { EmojiType.Scared, 4 },
        { EmojiType.Shit, 10 },
    };

        float multiplier = distance.ContainsKey(emojiType) ? distance[emojiType] : 3f;
        float stoppingRange = navMeshAgent.stoppingDistance * multiplier;
        float safeDistance = 3f;
        float adjustedStoppingRange = stoppingRange + safeDistance;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= adjustedStoppingRange)
        {
            navMeshAgent.isStopped = true;
            transform.LookAt(targetPosition);
            onComplete?.Invoke(this);
            yield break;
        }

        StopMoving();
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
        animator.CrossFade(Characteranimationkey.Walking, 0f, 0);

        while (Vector3.Distance(transform.position, targetPosition) > stoppingRange)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        transform.LookAt(targetPosition);

        onComplete?.Invoke(this);
    }


}


