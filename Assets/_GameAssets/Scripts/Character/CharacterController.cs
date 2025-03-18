using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public EmojiController emojiController;
    public CharacterMove characterMove;
    public bool isEnemyTarget;

    [System.Serializable]
    public struct EmojiAnimation
    {
        public EmojiType emojiType;
        public RuntimeAnimatorController animatorController;
    }

    [SerializeField] private List<EmojiAnimation> emojiAnimations;
    private RuntimeAnimatorController defaultAnimatorController;
    private Dictionary<EmojiType, RuntimeAnimatorController> emojiToControllerMap;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (emojiController == null)
        {
            emojiController = GetComponent<EmojiController>();
        }
        if (characterMove == null)
        {
            characterMove = GetComponent<CharacterMove>();
        }

        defaultAnimatorController = animator.runtimeAnimatorController;
        emojiToControllerMap = new Dictionary<EmojiType, RuntimeAnimatorController>();
        foreach (var emojiAnimation in emojiAnimations)
        {
            emojiToControllerMap[emojiAnimation.emojiType] = emojiAnimation.animatorController;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            characterMove.isCharacterMove = false;
            if (emojiController != null)
            {
                if (emojiToControllerMap.TryGetValue(emojiController.currentEmoji, out var controller))
                {
                    if (controller != null)
                    {
                        Debug.Log("Target anim");
                        characterMove.isCharacterMove = false;
                        animator.runtimeAnimatorController = controller;
                        StartCoroutine(ResetCharacterState());
                    }
                }
                if (isEnemyTarget)
                {
                    GamePlayController.I.OnEnemyHit(this);
                }
            }
        }
    }

    private IEnumerator ResetCharacterState()
    {
        yield return new WaitForSeconds(5f);
        // Reset character movement and animation
        animator.runtimeAnimatorController = defaultAnimatorController;
        characterMove.RestartMovement(); // Gọi phương thức mới để khởi động lại việc di chuyển
    }

    public void SetAsEnemyTarget()
    {
        isEnemyTarget = true;
    }
}
