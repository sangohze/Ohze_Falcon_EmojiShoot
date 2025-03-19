using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator;
    
    public CharacterMove characterMove;
    public bool isEnemyTarget;

    [System.Serializable]
    public struct EmojiAnimation
    {
        public EmojiType emojiType;
        public RuntimeAnimatorController animatorController;
    }

    [SerializeField] private List<EmojiAnimation> emojiAnimations;
 
    private Dictionary<EmojiType, RuntimeAnimatorController> emojiToControllerMap;

    void Start()
    {

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
            characterMove.isCharacterMove = false; // Dừng di chuyển

            if (EmojiController.I != null)
            {
                if (emojiToControllerMap.TryGetValue(EmojiController.I.currentEmoji, out var controller))
                {
                    if (controller != null)
                    {
                        characterMove.isCharacterMove = false;

                        // Lấy tên animation trực tiếp từ EmojiType bằng ToString()
                        string animState = EmojiController.I.currentEmoji.ToString();
                        animator.CrossFade(animState, 0, 0);
                        Debug.Log("Target anim: " + animState);

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
        yield return new WaitForSeconds(3f);
        // Reset character movement and animation

        characterMove.RestartMovement(); // Gọi phương thức mới để khởi động lại việc di chuyển
    }

    public void SetAsEnemyTarget()
    {
        isEnemyTarget = true;
    }
}
