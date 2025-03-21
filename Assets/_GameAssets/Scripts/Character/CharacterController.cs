using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator;

    public CharacterMove characterMove;
    public bool isEnemyTarget;
    [SerializeField] private List<ParticleSystem> emojiEffects = new List<ParticleSystem>();

    private Dictionary<EmojiType, ParticleSystem> emojiToEffectMap;




    void Start()
    {
        emojiToEffectMap = new Dictionary<EmojiType, ParticleSystem>();
        for (int i = 0; i < emojiEffects.Count; i++)
        {
            if (i < System.Enum.GetValues(typeof(EmojiType)).Length)
            {
                emojiToEffectMap[(EmojiType)i] = emojiEffects[i];
            }
        }


    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            Debug.Log("Target anim: ");
            characterMove.StopMoving();

            if (EmojiController.I == null) return;

            
            string animState = EmojiController.I.currentEmoji.ToString();
            animator.CrossFade(animState, 0, 0);
            if (EmojiController.I.currentEmoji == EmojiType.Love )
            {
                if (emojiEffects.Count > 0)
                {
                    emojiEffects[0].Play();
                    emojiEffects[1].Play();
                }
            }
            else
            {
                int index = (int)EmojiController.I.currentEmoji;
                if (index >= 1 && index < emojiEffects.Count)
                {
                    emojiEffects[index+1].Play();
                }
            }
           

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
