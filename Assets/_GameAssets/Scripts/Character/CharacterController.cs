using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator;

    public CharacterMove characterMove;
    public bool isEnemyTarget;






    void Start()
    {



    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            characterMove.StopMoving();

            if (EmojiController.I == null) return;

            
            string animState = EmojiController.I.currentEmoji.ToString();
            animator.CrossFade(animState, 0, 0);
            Debug.Log("Target anim: " + animState);

            StartCoroutine(ResetCharacterState());

            if (isEnemyTarget)
            {
                if(EmojiController.I.currentEmoji != GamePlayController.I.EmojiTypeTarget) return;
                GamePlayController.I.OnEnemyHit(this);
            }

        }
    }


    private IEnumerator ResetCharacterState()
    {
        yield return new WaitForSeconds(5f);
        // Reset character movement and animation

        characterMove.RestartMovement(); // Gọi phương thức mới để khởi động lại việc di chuyển
    }

    public void SetAsEnemyTarget()
    {
        isEnemyTarget = true;

    }
}
