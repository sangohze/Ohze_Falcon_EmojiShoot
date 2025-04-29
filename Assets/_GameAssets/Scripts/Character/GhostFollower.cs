using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class GhostFollower : MonoBehaviour
{
    private Transform _target;
    private Vector3 _followOffset = new Vector3(0, -2f, 0); // Ghost đi theo phía dưới nhân vật
    private float _followSpeed = 1.5f;
    private bool _isFollowing = false;
    [SerializeField] Animator _animator;
    private NavMeshAgent _agent;
    [SerializeField] private float _followDistance = 3f;
    [SerializeField] private float _rotationSpeed = 3f;      // Tốc độ xoay mặt
    private int _indexOffset = 0;
    private float _offsetSpacing = 2.5f; // Khoảng cách ngang giữa các ghost

    private void OnEnable()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (_agent != null)
        {
            _agent.updateRotation = false;  // Không tự xoay
            _agent.updateUpAxis = false;    // Không ảnh hưởng trục Y
        }
    }
    public void Init(Vector3 endPos, Transform target, int indexOffset = 0)
    {
        _indexOffset = indexOffset;

        // Tính toán endPos mới với một chút offset cho mỗi ghost
        Vector3 offsetDirection = (Vector3.right * Mathf.Sin(indexOffset * Mathf.PI / 2) + Vector3.forward * Mathf.Cos(indexOffset * Mathf.PI / 2)) * 2f;
        Vector3 finalEndPos = endPos + offsetDirection;

        transform.DOMove(finalEndPos, 1.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                _isFollowing = target != null;
                StartFollow(target);
            });
    }


    private void StartFollow(Transform target)
    {
        _target = target;

        if (gameObject.activeInHierarchy)
        {
            PlayRunAnimation();
            _isFollowing = true;
        }
    }

    private void Update()
    {
        if (_isFollowing && _target != null)
        {
            Vector3 behindTarget = _target.position - _target.forward * _followDistance;

            // Thêm offset trái/phải dựa trên index để tránh đè nhau
            Vector3 right = _target.right;
            behindTarget += right * (_indexOffset * _offsetSpacing - _offsetSpacing); // offset -1.5, 0, 1.5 cho index 0-2
            behindTarget.y = transform.position.y;

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.SetDestination(behindTarget);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, behindTarget, _followSpeed * Time.deltaTime);
            }

            // Xoay cùng hướng với target
            Quaternion targetRotation = Quaternion.Euler(0f, _target.eulerAngles.y + 180f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }








    public void StopFollowAndDespawn()
    {
        _isFollowing = false;
        PlayIdleAnimation();
        // Trượt xuống rồi despawn
        Vector3 downPos = transform.position + new Vector3(0, -3f, 0);
        transform.DOMove(downPos, 1.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                if (_agent != null)
                {
                    _agent.updateRotation = false;  // Không tự xoay
                    _agent.updateUpAxis = false;    // Không ảnh hưởng trục Y
                }
                Lean.Pool.LeanPool.Despawn(gameObject);
            });
    }
    private void PlayRunAnimation()
    {
        if (_animator != null)
        {
            _animator.CrossFade("Walk_02", 0.2f); // Blend mượt sang anim Run
        }
    }

    private void PlayIdleAnimation()
    {
        if (_animator != null)
        {
            _animator.CrossFade("Idle", 0.2f); // Blend về Idle trước khi biến mất
        }
    }
}
