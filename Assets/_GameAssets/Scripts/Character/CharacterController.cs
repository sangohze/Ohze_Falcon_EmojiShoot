using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;


public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public CharacterMove characterMove;
    public bool isEnemyTarget;
    private Quaternion characterRotationDefault;
    private Dictionary<EmojiType, ParticleSystem> emojiToEffectMap = new Dictionary<EmojiType, ParticleSystem>();
    private Coroutine resetMovementCoroutine;
    public bool isMan;
    private Transform headPosition;
    private Transform eyesPosition;
    private Transform mouthPosition;
    [SerializeField] private Transform parentPos;
    public int characterID;
    public Sprite Avatar;
    private AudioCueKey _currentSFXKey;
    private float timeEndAnim = 10f;
    private Dictionary<EmojiType, TypeEffect > emojiEffectMap;
    private TypeEffect? _currentEffect;
    private Vector3 effectPositions = new Vector3(0, 2.4f, 0);
    private void InitEffectMap()
    {
        emojiEffectMap = new Dictionary<EmojiType, TypeEffect>
    {
        { EmojiType.Love,TypeEffect.Eff_LoveSingle},
        { EmojiType.Sad,TypeEffect.Eff_SadSingle},
        { EmojiType.Angry,TypeEffect.Eff_AngrySingle},
        { EmojiType.Pray,  TypeEffect.Eff_PrayerSingle},
        { EmojiType.Devil,TypeEffect.Eff_DevilSingle},
        { EmojiType.Dance, TypeEffect.Eff_DanceSingle},
        { EmojiType.Vomit, TypeEffect.Eff_VomitSingle}
    };
    }

    void Start()
    {
        InitEffectMap();
        SetUpPostionEffect();
        SetUpRotationLookAtCamera();
    }

    private void SetUpRotationLookAtCamera()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        directionToCamera.y = 0; // Giữ nhân vật nhìn theo mặt phẳng ngang

        if (directionToCamera != Vector3.zero)
        {
            characterRotationDefault = Quaternion.LookRotation(directionToCamera);
        }
        else
        {
            characterRotationDefault = Quaternion.identity;
        }

    }

    void SetUpPostionEffect()
    {
        headPosition = new GameObject("Head").GetComponent<Transform>();
        eyesPosition = new GameObject("Eyes").GetComponent<Transform>();
        mouthPosition = new GameObject("Mouth").GetComponent<Transform>();

        headPosition.transform.SetParent(parentPos, worldPositionStays: false);
        headPosition.transform.localPosition = new Vector3(0.00045f, -0.0002f, 0.0059f);
        headPosition.transform.localRotation = Quaternion.Euler(0, 0, 0);
        headPosition.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        eyesPosition.transform.SetParent(parentPos, worldPositionStays: false);
        eyesPosition.transform.localPosition = new Vector3(-0.00115f, -0.00064f, 0.0024f);
        eyesPosition.transform.localRotation = Quaternion.Euler(0, -0, -90);
        eyesPosition.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        mouthPosition.transform.SetParent(parentPos, worldPositionStays: false);
        mouthPosition.transform.localPosition = new Vector3(-0.00284f, -0.00043f, -0.00564f);
        mouthPosition.transform.localRotation = Quaternion.Euler(0, -90, -90);
        mouthPosition.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "EmojiProjectile")
        {
            HandleCollision();
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
            HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            GamePlayController.I.OnEnemyTargetHit(this);
        }
        GamePlayController.I.ResetHitState();
    }

    private void HandleFirstHit(string animState, EmojiType currentEmoji)
    {

        transform.DORotateQuaternion(characterRotationDefault, 0.5f);
        animator.CrossFade(animState, 0, 0);
        PlayEffectCombo(this, currentEmoji);
        HideEffOne();
        SpawnEmojiEffect(currentEmoji);
        PlaySoundFXSingle(currentEmoji, this);
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
                EffectManager.I.HideEffectAll();
                GamePlayController.I.secondHitEnemy.characterMove.MoveTowardsEnemy(characterMove, currentEmoji, () =>
                {
                    animator.CrossFade(animState, 0, 0);
                    PlaySoundFXCombo(currentEmoji, this);
                    SpawnEmojiEffect(currentEmoji);
                    GamePlayController.I.secondHitEnemy.animator.CrossFade(animState, 0, 0);
                    GamePlayController.I.secondHitEnemy.SpawnEmojiEffect(currentEmoji);          
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
            HideEffOne();
            SpawnEmojiEffect(currentEmoji);
            PlaySoundFXSingle(currentEmoji, this);
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
        yield return new WaitForSeconds(timeEndAnim);
        EffectManager.I.HideEffectAll();
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

    private void SpawnEmojiEffect(EmojiType emoji)
    {
        if (emojiEffectMap.TryGetValue(emoji, out var eff))
        {
            GameObject effect = EffectManager.I.PlayEffect(eff, Vector3.zero);
            effect.transform.SetParent(this.transform, worldPositionStays: false);
            effect.transform.localPosition = effectPositions;
            _currentEffect = eff;
            Debug.Log("Sangdev_currentEffect1 " + this.name+ _currentEffect);
        }
    }

    private void HideEffOne()
    {
        if (_currentEffect != null)
        {
            EffectManager.I.HideEffectOne(_currentEffect.Value);
            _currentEffect = null;
            Debug.Log("Sangdev_currentEffect0 " + this.name + _currentEffect.ToString());
        }
    }    


    private IEnumerator ResetCharacterState()
    {
        yield return new WaitForSeconds(timeEndAnim);
        EffectManager.I.HideEffectAll();
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
                var emojiMap = new Dictionary<string, EmojiType>
                {
                    { "Praying", EmojiType.Pray },
                    { "running scare", EmojiType.Sad },
                    { "Vomit", EmojiType.Vomit },
                    { "Cherring", EmojiType.Dance }
                };

                if (emojiMap.TryGetValue(animationKey, out var emojiTypeRemaining))
                {
                    enemy.SpawnEmojiEffect(emojiTypeRemaining);
                }
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

    private void PlayEffectCombo(CharacterController enemy, EmojiType emojiType) //postiton special
    {
        GameObject eff;
        Transform parentTransform = null;
        switch (emojiType)
        {
            case EmojiType.Angry:
                parentTransform = enemy.headPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_FireAngry, transform.position);
                eff.transform.SetParent(parentTransform, worldPositionStays: false);
                eff.transform.localPosition = Vector3.zero;  
                eff.transform.localRotation = Quaternion.identity; 
                eff.transform.localScale = Vector3.one;
                break;
            case EmojiType.Vomit:
                parentTransform = enemy.mouthPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Vomit, transform.position);
                eff.transform.SetParent(parentTransform, worldPositionStays: false);
                eff.transform.localPosition = Vector3.zero;
                eff.transform.localRotation = Quaternion.identity;
                eff.transform.localScale = Vector3.one;
                break;
            case EmojiType.Sad:
                parentTransform = enemy.eyesPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_SadClould, transform.position);
                break;
            default:
                return;
        }
       
    }

    private void PlaySoundFXSingle(EmojiType emojiType, CharacterController enemy)
    {
        SoundManager.I.StopAllSFX();
        var soundMap = new Dictionary<EmojiType, TypeSound>
    {
        { EmojiType.Love, enemy.isMan ? TypeSound.SFX_Love_Man : TypeSound.SFX_Love_Girl },
        { EmojiType.Sad, enemy.isMan ? TypeSound.SFX_Sad_Man : TypeSound.SFX_Sad_Girl },
        { EmojiType.Angry, enemy.isMan ? TypeSound.SFX_Angry_Man : TypeSound.SFX_Angry_Girl},
        { EmojiType.Pray, TypeSound.SFX_Pray },
        { EmojiType.Devil, TypeSound.SFX_Devil },
        { EmojiType.Dance,  TypeSound.SFX_Dance},
        { EmojiType.Vomit, TypeSound.SFX_Vomit }
    };

        if (soundMap.TryGetValue(emojiType, out TypeSound sound))
        {
            _currentSFXKey = SoundManager.I.PlaySFX(sound);
        }
    }

    private void PlaySoundFXCombo(EmojiType emojiType, CharacterController enemy)
    {
        SoundManager.I.StopAllSFX();

        var soundMap = new Dictionary<EmojiType, TypeSound>
    {
        { EmojiType.Love, TypeSound.SFX_Lovers },
        { EmojiType.Sad, enemy.isMan ? TypeSound.SFX_Sad_Man : TypeSound.SFX_Sad_Girl },
        { EmojiType.Angry, TypeSound.SFX_Fight},
        { EmojiType.Pray, TypeSound.SFX_God },
        { EmojiType.Devil, TypeSound.SFX_Summon },
        { EmojiType.Dance,  TypeSound.SFX_Dance},
        { EmojiType.Vomit, TypeSound.SFX_Stinky }
    };

        if (soundMap.TryGetValue(emojiType, out TypeSound sound))
        {
            _currentSFXKey = SoundManager.I.PlaySFX(sound);
        }
    }

}







