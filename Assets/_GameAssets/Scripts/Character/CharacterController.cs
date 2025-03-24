using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterController : MonoBehaviour
{
    public Animator animator;

    public CharacterMove characterMove;
    public bool isEnemyTarget;
    private Quaternion characterRotationDefault = Quaternion.Euler(0, 90, 0);

    private Dictionary<EmojiType, ParticleSystem> emojiToEffectMap = new Dictionary<EmojiType, ParticleSystem>();




    void Start()
    {
        for (int i = 0; i < EmojiController.I.emojiEffects.Count; i++)
        {
            if (i < System.Enum.GetValues(typeof(EmojiType)).Length)
            {
                emojiToEffectMap[(EmojiType)i] = EmojiController.I.emojiEffects[i];
            }
        }
        StartCoroutine(ResetCharacter());
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            Debug.Log("Target anim: ");
            transform.DORotateQuaternion(characterRotationDefault, 0.5f);
            characterMove.StopMoving();
            if (EmojiController.I == null) return;
            string animState = EmojiController.I.currentEmoji.ToString();
            animator.CrossFade(animState, 0, 0);
            if (EmojiController.I.currentEmoji == EmojiType.Love)
            {
                if (EmojiController.I.emojiEffects.Count > 0)
                {
                    SpawnEmojiEffect(EmojiController.I.emojiEffects[0], EmojiController.I.effectPositions[0]);
                    SpawnEmojiEffect(EmojiController.I.emojiEffects[1], EmojiController.I.effectPositions[1]);
                }
            }
            else
            {
                int index = (int)EmojiController.I.currentEmoji;
                if (index >= 1 && index < EmojiController.I.emojiEffects.Count)
                {
                    SpawnEmojiEffect(EmojiController.I.emojiEffects[index +1], EmojiController.I.effectPositions[index + 1]);
                }
            }
            StartCoroutine(ResetCharacterState());

            if (isEnemyTarget)
            {
                if (EmojiController.I.currentEmoji != GamePlayController.I.EmojiTypeTarget) return;
                GamePlayController.I.OnEnemyHit(this);
            }
        }
    }

    private IEnumerator ResetCharacterState()
    {
        yield return new WaitForSeconds(50f);
        // Reset character movement and animation

        characterMove.RestartMovement(); // Gọi phương thức mới để khởi động lại việc di chuyển
    }
    private IEnumerator ResetCharacter()
    {
        yield return new WaitForSeconds(0.1f);
        animator.applyRootMotion = true;
    }

    public void SetAsEnemyTarget()
    {
        isEnemyTarget = true;

    }
    private void SpawnEmojiEffect(ParticleSystem effectPrefab, Vector3 position)
    {
        ParticleSystem effectInstance = Instantiate(effectPrefab, transform);
        effectInstance.transform.localPosition = position; // Đặt vị trí tùy ý
        effectInstance.Play();
    }
}
