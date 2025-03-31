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
    private Transform headPosition;
    private Transform eyesPosition;
    private Transform mouthPosition;
    [SerializeField] private Transform parentPos;

    void Start()
    {
        InitializeEmojiEffects();
        SetUpPostionEffect();
    }

    void SetUpPostionEffect()
    {
        headPosition = new GameObject("Head").GetComponent<Transform>();
        eyesPosition = new GameObject("Eyes").GetComponent<Transform>();
        mouthPosition = new GameObject("Mouth").GetComponent<Transform>();

        headPosition.transform.SetParent(parentPos, worldPositionStays: false);
        headPosition.transform.localPosition = new Vector3(0.00015f, -0.00192f, 0.005f);
        headPosition.transform.localRotation = Quaternion.Euler(0, 0, 0);
        headPosition.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        eyesPosition.transform.SetParent(parentPos, worldPositionStays: false);
        eyesPosition.transform.localPosition = new Vector3(-0.00115f, -0.00064f, 0.0024f);
        eyesPosition.transform.localRotation = Quaternion.Euler(0, -0, -90);
        eyesPosition.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        mouthPosition.transform.SetParent(parentPos, worldPositionStays: false);
        mouthPosition.transform.localPosition = new Vector3(0.0001f, -0.0003f, 0.001f);
        mouthPosition.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

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
        if (resetMovementCoroutine != null)
        {
            StopCoroutine(resetMovementCoroutine);
        }
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
        PlayEffectCombo(this, currentEmoji);
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
        foreach (var enemy in GamePlayController.I.CurrentListEnemy)
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
        foreach (var enemy in GamePlayController.I.CurrentListEnemy)
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
            if (enemy == GamePlayController.I.secondHitEnemy || enemy == GamePlayController.I.firstHitEnemy)
            {
                PlayEffectCombo(enemy, emojitype);
            }
            else
            {
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
    }

    private IEnumerator WaitForAnimation(CharacterController enemy, System.Action onComplete)
    {
        yield return new WaitForSeconds(enemy.animator.GetCurrentAnimatorStateInfo(0).length);
        onComplete?.Invoke();
    }

    private void PlayVFXCharaterRemaining(string animationKey)
    {
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

    private void PlayEffectCombo(CharacterController enemy, EmojiType emojiType)
    {
        GameObject eff;
        Transform parentTransform = null;
        Debug.Log("sangdevmoving" + enemy.name);
        switch (emojiType)
        {
            case EmojiType.Sad:
                parentTransform = enemy.eyesPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_TearCry, transform.position);
                break;
            case EmojiType.Angry:
                parentTransform = enemy.headPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_FireAngry, transform.position);
                break;
            case EmojiType.Vomit:
                parentTransform = enemy.mouthPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Vomit, transform.position);
                break;
            default:
                return;
        }


        eff.transform.SetParent(parentTransform, worldPositionStays: false);
        // Set Parent nhưng giữ nguyên thông số Prefab
        eff.transform.localPosition = Vector3.zero;  // Đặt về đúng vị trí gốc của parent
        eff.transform.localRotation = Quaternion.identity; // Giữ nguyên góc xoay
        eff.transform.localScale = Vector3.one;
        eff.GetComponent<ParticleSystem>().Play();
    }

}







