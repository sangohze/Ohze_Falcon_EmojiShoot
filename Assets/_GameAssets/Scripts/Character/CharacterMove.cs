using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour
{
    public float moveRadius = 10f;
    public float moveInterval = 3f;
    public Animator animator;
    private NavMeshAgent navMeshAgent;
    private Vector3 startPosition;
    public bool isCharacterMove;

    void Start()
    {
        isCharacterMove = true;
        navMeshAgent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        StartCoroutine(MoveRandomly());
    }

    public void RestartMovement()
    {
        isCharacterMove = true;
        StartCoroutine(MoveRandomly());
    }

    IEnumerator MoveRandomly()
    {
        while (isCharacterMove)
        {
            Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
            randomDirection += startPosition;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, moveRadius, 1);
            Vector3 finalPosition = hit.position;

            navMeshAgent.SetDestination(finalPosition);
            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.isOnNavMesh)
                {
                    Debug.LogError("NavMeshAgent is not on a valid NavMesh");
                    yield break;
                }
                yield return null;
            }
            yield return new WaitForSeconds(moveInterval);
        }
    }
}
