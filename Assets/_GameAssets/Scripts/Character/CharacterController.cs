using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public CharacterMove characterMove;
    public bool isEnemyTarget;
    private Quaternion characterRotationDefault = Quaternion.Euler(0, 90, 0);
    private Dictionary<EmojiType, ParticleSystem> emojiToEffectMap = new Dictionary<EmojiType, ParticleSystem>();
    private Coroutine resetMovementCoroutine;
    public bool isMan;
    void Start()
    {
        InitializeEmojiEffects();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            HandleCollision();
        }
    }

    private void InitializeEmojiEffects()
    {
        for (int i = 0; i < EmojiController.I.emojiEffects.Count; i++)
        {
            if (i < System.Enum.GetValues(typeof(EmojiType)).Length)
            {
                emojiToEffectMap[(EmojiType)i] = EmojiController.I.emojiEffects[i];
            }
        }
    }

    private void HandleCollision()
    {
        characterMove.StopMoving();
        if (EmojiController.I == null) return;
        EmojiType currentEmoji = EmojiController.I.currentEmoji;
        string animStateSingle = currentEmoji.ToString();
        string animStateDouble = $"{currentEmoji}2";
        if (GamePlayController.I.firstHitEnemy == null || GamePlayController.I.firstHitEnemy == this || GamePlayController.I.firstHitEmoji != currentEmoji)
        {
            HandleFirstHit(animStateSingle, currentEmoji);
        }
        else if (GamePlayController.I.firstHitEmoji == currentEmoji)
        {
            HandleSecondHit(animStateDouble, currentEmoji);
        }
        if (isEnemyTarget && currentEmoji == GamePlayController.I.EmojiTypeTarget)
        {
            GamePlayController.I.OnEnemyTargetHit(this);
        }
        GamePlayController.I.ResetHitState();
    }

    private void HandleFirstHit(string animState, EmojiType currentEmoji)
    {
        transform.DORotateQuaternion(characterRotationDefault, 0.5f);
        animator.CrossFade(animState, 0, 0);
        PlayEmojiEffectSingle(currentEmoji);
        GamePlayController.I.firstHitEnemy = this;
        GamePlayController.I.firstHitEmoji = currentEmoji;
        resetMovementCoroutine = StartCoroutine(ResetCharacterState());
    }

    private void HandleSecondHit(string animState, EmojiType currentEmoji)
    {
        if (GamePlayController.I.firstHitEnemy != this)
        {
            var previousFirstHit = GamePlayController.I.firstHitEnemy;
            GamePlayController.I.firstHitEnemy = this;
            GamePlayController.I.firstHitEmoji = currentEmoji;
            GamePlayController.I.secondHitEnemy = previousFirstHit;
            if (previousFirstHit != null && previousFirstHit.resetMovementCoroutine != null)
            {
                previousFirstHit.StopCoroutine(previousFirstHit.resetMovementCoroutine);
                previousFirstHit.resetMovementCoroutine = null;
            }

            if (GamePlayController.I.secondHitEnemy != null)
            {
                GamePlayController.I.secondHitEnemy.characterMove.MoveTowardsEnemy(characterMove, () =>
                {
                    animator.CrossFade(animState, 0, 0);
                    PlayEmojiEffectSingle(currentEmoji);
                    GamePlayController.I.secondHitEnemy.animator.CrossFade(animState, 0, 0);
                    GamePlayController.I.secondHitEnemy.PlayEmojiEffectSingle(currentEmoji);
                    PlayEffectCombo("TearCry");
                    StopAllCharaterMoving();
                    ResetAllCharacters();
                    PlayAnimationForRemainingEnemies(currentEmoji, () =>
                        {
                        });
                });
            }
        }
        else
        {
            transform.DORotateQuaternion(characterRotationDefault, 0.5f);
            animator.CrossFade(animState, 0, 0);
            PlayEmojiEffectSingle(currentEmoji);
            StartCoroutine(ResetCharacterState());
        }
    }

    private void ResetAllCharacters()
    {
        foreach (var enemy in LevelManager.I.CurrentListEnemy)
        {
            enemy.resetMovementCoroutine = enemy.StartCoroutine(ResetCharacterState(enemy.characterMove));
        }
    }

    private IEnumerator ResetCharacterState(CharacterMove character)
    {
        yield return new WaitForSeconds(5f);
        character.RestartMovement();
    }

    private void StopAllCharaterMoving()
    {
        foreach (var enemy in LevelManager.I.CurrentListEnemy)
        {
            if (enemy != (GamePlayController.I.secondHitEnemy || GamePlayController.I.firstHitEnemy) && enemy.resetMovementCoroutine != null)
            {           
                enemy.characterMove.StopCoroutine(resetMovementCoroutine);
            }
        }
    }
    private void PlayEmojiEffectSingle(EmojiType currentEmoji)
    {
        if (currentEmoji == EmojiType.Love)
        {
            if (EmojiController.I.emojiEffects.Count > 0)
            {
                SpawnEmojiEffect(EmojiController.I.emojiEffects[0], EmojiController.I.effectPositions[0]);
                SpawnEmojiEffect(EmojiController.I.emojiEffects[1], EmojiController.I.effectPositions[1]);
            }
        }
        else
        {
            int index = (int)currentEmoji;
            if (index >= 1 && index < EmojiController.I.emojiEffects.Count)
            {
                SpawnEmojiEffect(EmojiController.I.emojiEffects[index + 1], EmojiController.I.effectPositions[index + 1]);
            }
        }
    }

    private void SpawnEmojiEffect(ParticleSystem effectPrefab, Vector3 position)
    {
        ParticleSystem effectInstance = LeanPool.Spawn(effectPrefab, transform);
        effectInstance.transform.localPosition = position;
        effectInstance.Play();
    }

    public void PlayEffectCombo(string effectName)
    {
        if (EmojiController.I.emojiEffectsDictComBo.TryGetValue(effectName, out var effectData))
        {
            Debug.LogWarning($"sangdev: {effectName}");
            // Gọi SpawnEmojiEffect với dữ liệu lấy từ dictionary
            SpawnEmojiEffect(effectData.effect, effectData.position);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy hiệu ứng: {effectName}");
        }
    }


    private IEnumerator ResetCharacterState()
    {
        yield return new WaitForSeconds(5f);
        characterMove.RestartMovement();
    }

    public void SetAsEnemyTarget()
    {
        isEnemyTarget = true;
    }

    private void PlayAnimationForRemainingEnemies(EmojiType emojitype, System.Action onComplete)
    {
        int completedAnimations = 0;
        int totalEnemies = LevelManager.I.CurrentListEnemy.Count(enemy => enemy != this && enemy != GamePlayController.I.firstHitEnemy);

        foreach (var enemy in LevelManager.I.CurrentListEnemy)
        {
            if (enemy == GamePlayController.I.secondHitEnemy || enemy == GamePlayController.I.firstHitEnemy) continue;
            string animationKey = emojitype == EmojiType.Pray ? Characteranimationkey.PrayRemaining :
                                  emojitype == EmojiType.Devil ? Characteranimationkey.DevilRemaining :
                                   (emojitype == EmojiType.Love && GamePlayController.I.firstHitEnemy != null
                       && GamePlayController.I.secondHitEnemy != null
                       && GamePlayController.I.firstHitEnemy.isMan == GamePlayController.I.secondHitEnemy.isMan)
                      ? Characteranimationkey.Vomit :
                                  Characteranimationkey.Cherring;

            enemy.PlayVFXCharaterRemaining(animationKey);
            //if (emojitype != EmojiType.Devil)
            //{
            //    enemy.characterMove.StopMoving();
            //}
            enemy.characterMove.StopMoving();
            enemy.animator.CrossFade(animationKey, 0, 0);

            StartCoroutine(WaitForAnimation(enemy, () =>
            {
                completedAnimations++;
                if (completedAnimations >= totalEnemies)
                {
                    onComplete?.Invoke();

                }
            }));
        }
    }

    private IEnumerator WaitForAnimation(CharacterController enemy, System.Action onComplete)
    {
        yield return new WaitForSeconds(enemy.animator.GetCurrentAnimatorStateInfo(0).length);
        onComplete?.Invoke();
    }

    private void PlayVFXCharaterRemaining(string animationKey)
    {
        Debug.Log("sangdevmoving" + animationKey);
        switch (animationKey)
        {
            case "Praying":
                SpawnEmojiEffect(EmojiController.I.emojiEffects[4], EmojiController.I.effectPositions[4]);
                break;
            case "running scare":
                SpawnEmojiEffect(EmojiController.I.emojiEffects[2], EmojiController.I.effectPositions[2]);
                break;
            case "Vomit":
                SpawnEmojiEffect(EmojiController.I.emojiEffects[7], EmojiController.I.effectPositions[7]);
                break;
            case "Cherring":
                SpawnEmojiEffect(EmojiController.I.emojiEffects[6], EmojiController.I.effectPositions[6]);
                break;
        }
    }
}







