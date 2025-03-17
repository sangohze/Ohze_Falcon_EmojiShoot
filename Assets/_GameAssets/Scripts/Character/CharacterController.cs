using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public EmojiController emojiController;


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
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            if (emojiController != null)
            {
                switch (emojiController.currentEmoji)
                {
                    case EmojiType.Happy:
                        // Trigger Happy animation
                        animator.SetTrigger("Happy");
                        Debug.Log("Playing Happy animation for character");
                        break;
                    case EmojiType.Sad:
                        // Trigger Sad animation
                        animator.SetTrigger("Sad");
                        Debug.Log("Playing Sad animation for character");
                        break;
                    case EmojiType.Angry:
                        // Trigger Angry animation
                        animator.SetTrigger("Angry");
                        Debug.Log("Playing Angry animation for character");
                        break;
                    case EmojiType.Laughing:
                        // Trigger Laughing animation
                        animator.SetTrigger("Laughing");
                        Debug.Log("Playing Laughing animation for character");
                        break;
                    case EmojiType.Surprised:
                        // Trigger Surprised animation
                        animator.SetTrigger("Surprised");
                        Debug.Log("Playing Surprised animation for character");
                        break;
                    default:
                        Debug.Log("Unknown EmojiType for character");
                        break;
                }
            }
        }
    }
}