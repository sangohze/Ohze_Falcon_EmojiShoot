using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static RootMotion.Demos.CharacterThirdPerson;

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
        //animator.CrossFade(Characteranimationkey.Idel, 0.1f, 0);
    }

    IEnumerator MoveRandomly()
    {
        while (isCharacterMove)
        {
            Vector3 randomPosition = GetValidRandomPosition();
            animator.CrossFade(Characteranimationkey.Walking, 0.5f, 0);

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
    public void MoveTowardsEnemy(CharacterMove otherEnemy, EmojiType emojitype, System.Action onComplete = null)
    {
        StartCoroutine(MoveTowardsEachOther(otherEnemy, emojitype, onComplete));
    }

    private IEnumerator MoveTowardsEachOther(CharacterMove otherEnemy,EmojiType emojitype, System.Action onComplete)
    {
        Vector3 midpoint = (transform.position + otherEnemy.transform.position) / 2;
        navMeshAgent.isStopped = false;
        otherEnemy.navMeshAgent.isStopped = false;
        StopMoving();
        //otherEnemy.StopMoving();
        otherEnemy.StopCoroutine(moveCoroutine);

        navMeshAgent.SetDestination(midpoint);
        otherEnemy.navMeshAgent.SetDestination(midpoint);

        // Bắt đầu animation đi ngay lập tức
        animator.CrossFade(Characteranimationkey.Walking, 0f, 0);
        otherEnemy.animator.CrossFade(Characteranimationkey.Walking, 0f, 0);

        var distance = new Dictionary<EmojiType, float>
    {
        { EmojiType.Love, 2 },
        { EmojiType.Sad,  3 },
        { EmojiType.Angry,3},
        { EmojiType.Pray, 3 },
        { EmojiType.Devil, 3 },
        { EmojiType.Dance, 6},
        { EmojiType.Vomit, 5 },
    };
        float multiplier = distance[emojitype];
        while (Vector3.Distance(transform.position, otherEnemy.transform.position) > navMeshAgent.stoppingDistance * multiplier) 
        {
            yield return null;
        }
        // Khi đến nơi, spawn effect midpoint
        navMeshAgent.isStopped = true;
        otherEnemy.navMeshAgent.isStopped = true;
        transform.LookAt(otherEnemy.transform);
        otherEnemy.transform.LookAt(transform);
        PlayEffectComboMidPoint(midpoint, EmojiController.I.currentEmoji);
        onComplete?.Invoke();
    }

    private void PlayEffectComboMidPoint(Vector3 pos, EmojiType emojiType)
    {
        GameObject eff;
        switch (emojiType)
        {
            case EmojiType.Devil:               
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Devil, pos);
                break;
            case EmojiType.Angry:
               
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Smoke, pos);
                break;
            case EmojiType.Dance:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Dance, pos);
                break;
            case EmojiType.Pray:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Pray, pos);
                break;
            case EmojiType.Sad:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Sad, pos);
                break;
            default:
                return;
        }     
        //eff.GetComponent<ParticleSystem>().Play();
        StartCoroutine(StopEffectAfterTime(emojiType, 5f));
    }

    private IEnumerator StopEffectAfterTime(EmojiType emojiType, float delay)
    {
        yield return new WaitForSeconds(delay);
        switch (emojiType)
        {
            case EmojiType.Devil:
                EffectManager.I.HideEffectOne(TypeEffect.Eff_Devil);
                break;
            case EmojiType.Angry:

                EffectManager.I.HideEffectOne(TypeEffect.Eff_Smoke);
                break;
            case EmojiType.Dance:
                EffectManager.I.HideEffectOne(TypeEffect.Eff_Dance);
                break;

        }
    }

}


