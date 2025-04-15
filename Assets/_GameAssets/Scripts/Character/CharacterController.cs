using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
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
    private Dictionary<EmojiType, TypeEffect> emojiEffectMap;
    private TypeEffect? _currentEffectSingle;
    private TypeEffect? _currentEffectCombo;
    private GameObject _currentEffectSingleObj;
    private GameObject? _currentEffectComboObj;
    private TypeEffect? _currentEffectComboMidpoint;
    private GameObject? _currentEffectComboObjMidpoint;
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
            Vector3 pos = other.contacts[0].point;
            EffectManager.I.PlayEffect(TypeEffect.Eff_Hit, pos);
            HandleCollision();
        }
    }



    private void HandleCollision()
    {
        if (EmojiController.I == null) return;
        EmojiType currentEmoji = EmojiController.I.currentEmoji;
        string animStateSingle = currentEmoji.ToString();
        string animStateDouble = $"{currentEmoji}2";
        if (GamePlayController.I.firstHitEnemy == null || GamePlayController.I.firstHitEnemy == this || GamePlayController.I.firstHitEmoji != currentEmoji)
        {
            if (currentEmoji == GamePlayController.I.firstHitEmoji)
                return;
            characterMove.StopMoving();
            StopCoroutineResetMovement();
            HandleFirstHit(animStateSingle, currentEmoji);
            if (isEnemyTarget && currentEmoji == GamePlayController.I.EmojiTypeTarget)
            {
                HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                GamePlayController.I.OnEnemyTargetHit(this);
            }
        }
        else if (GamePlayController.I.firstHitEmoji == currentEmoji)
        {
            characterMove.StopMoving();
            StopCoroutineResetMovement();
            HandleSecondHit(animStateDouble, currentEmoji);
        }
        GamePlayController.I.SetTickPreviewByEnemy(currentEmoji);
    }

    private void HandleFirstHit(string animState, EmojiType currentEmoji)
    {
        transform.DORotateQuaternion(characterRotationDefault, 0.5f);
        animator.CrossFade(animState, 0, 0);
        HideEffOne();
        PlayEffectCombo(this, currentEmoji);
        SpawnEmojiEffectSingle(currentEmoji);
        PlaySoundFXSingle(currentEmoji, this);
        GamePlayController.I.firstHitEnemy = this;
        GamePlayController.I.firstHitEmoji = currentEmoji;
        this.resetMovementCoroutine = this.StartCoroutine(ResetCharacterStateAll(this));
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
                GamePlayController.I.secondHitEnemy.characterMove.MoveTowardsEnemy(characterMove, currentEmoji, (midpoint) =>
                {
                    
                    HideEffOne();
                    GamePlayController.I.secondHitEnemy.HideEffOne();
                    PlayEffectComboMidPoint(midpoint, currentEmoji);
                    this.animator.CrossFade(animState, 0, 0);
                    PlaySoundFXCombo(currentEmoji, this);
                    SpawnEmojiEffectSingle(currentEmoji);
                    GamePlayController.I.secondHitEnemy.animator.CrossFade(animState, 0, 0);
                    GamePlayController.I.secondHitEnemy.SpawnEmojiEffectSingle(currentEmoji);
                    StopAllCharaterMoving();
                    PlayAnimationForRemainingEnemies(currentEmoji, () =>
                        {
                        });
                    ResetAllCharacters();
                    if (isEnemyTarget && currentEmoji == GamePlayController.I.EmojiTypeTarget)
                    {
                        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                        GamePlayController.I.OnEnemyTargetHit(this);
                    }
                });
            }
        }
    }

  

    private void StopAllCharaterMoving()
    {
        foreach (var enemy in GamePlayController.I.CurrentListEnemy)
        {
            if (enemy != GamePlayController.I.secondHitEnemy && 
                enemy != GamePlayController.I.firstHitEnemy
                && enemy.resetMovementCoroutine != null)
            {
                enemy.StopCoroutineResetMovement();
            }
        }
    }

    private void SpawnEmojiEffectSingle(EmojiType emoji)
    {
        if (emojiEffectMap.TryGetValue(emoji, out var eff))
        {
            GameObject effect = EffectManager.I.PlayEffect(eff, Vector3.zero);
            effect.transform.SetParent(this.transform, worldPositionStays: false);
            effect.transform.localPosition = effectPositions;
            _currentEffectSingle = eff;
            _currentEffectSingleObj = effect;
        }
    }

    public void HideEffOne()
    {
        Debug.LogError("charactersang" + this.name + _currentEffectSingle);
        if (_currentEffectSingle != null)
        {
            EffectManager.I.HideEffectOne(_currentEffectSingle.Value, _currentEffectSingleObj);
            _currentEffectSingle = null;
        }
        if (_currentEffectCombo != null)
        {
            EffectManager.I.HideEffectOne(_currentEffectCombo.Value, _currentEffectComboObj);
            _currentEffectCombo = null;
        }
        HideEffMidpoint();
    }

    
    private void ResetAllCharacters()
    {
       
        foreach (var enemy in GamePlayController.I.CurrentListEnemy)
        {
            Debug.LogError("sangchade" + enemy.name);
            enemy.resetMovementCoroutine = enemy.StartCoroutine(ResetCharacterStateAll(enemy));
        }
    }

    private IEnumerator ResetCharacterStateAll(CharacterController character)
    {
        yield return new WaitForSeconds(timeEndAnim);
        GamePlayController.I.firstHitEmoji = null;
        character.HideEffOne();
        character.characterMove.RestartMovement(Characteranimationkey.Walking);
    }

  

    public void SetAsEnemyTarget()
    {
        isEnemyTarget = true;
    }

    private void PlayAnimationForRemainingEnemies(EmojiType emojitype, System.Action onComplete)
    {
        int completedAnimations = 0;
        var level = LevelManager.I._isTest ? LevelTest.I.CurrentListEnemy : LevelManager.I.CurrentListEnemy;
        int totalEnemies = level.Count(enemy => enemy != this && enemy != GamePlayController.I.firstHitEnemy);

        foreach (var enemy in level)
        {
            if (enemy == GamePlayController.I.secondHitEnemy || enemy == GamePlayController.I.firstHitEnemy)
            {
            Debug.LogError("sangenemy" + enemy.name);

            }
            else
            {
                Debug.LogError("sangenemyremaing" + enemy.name);
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
                enemy.StopCoroutineResetMovement();
                enemy.HideEffOne();
                if (emojiMap.TryGetValue(animationKey, out var emojiTypeRemaining))
                {
                    enemy.SpawnEmojiEffectSingle(emojiTypeRemaining);
                }
                enemy.characterMove.StopMoving();
                enemy.animator.CrossFade(animationKey, 0, 0);
                if (emojitype == EmojiType.Devil)
                {
                    enemy.characterMove.RestartMovement(animationKey);
                }
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
                _currentEffectCombo = TypeEffect.Eff_FireAngry;
                break;
            case EmojiType.Vomit:
                parentTransform = enemy.mouthPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Vomit, transform.position);
                eff.transform.SetParent(parentTransform, worldPositionStays: false);
                eff.transform.localPosition = Vector3.zero;
                eff.transform.localRotation = Quaternion.identity;
                eff.transform.localScale = Vector3.one;
                _currentEffectCombo = TypeEffect.Eff_Vomit;
                break;
            case EmojiType.Sad:
                parentTransform = enemy.eyesPosition.transform;
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_SadClould, transform.position);
                _currentEffectCombo = TypeEffect.Eff_SadClould;
                break;
            default:
                return;
        }
        _currentEffectComboObj = eff;
    }

    private void PlayEffectComboMidPoint(Vector3 pos, EmojiType emojiType)
    {
        GameObject eff;
        switch (emojiType)
        {
            case EmojiType.Devil:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Devil, pos);
                _currentEffectComboMidpoint = TypeEffect.Eff_Devil;
                break;
            case EmojiType.Angry:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Smoke, pos);
                _currentEffectComboMidpoint = TypeEffect.Eff_Smoke;
                break;
            case EmojiType.Dance:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Dance, pos);
                _currentEffectComboMidpoint = TypeEffect.Eff_Dance;
                break;
            case EmojiType.Pray:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Pray, pos);
                _currentEffectComboMidpoint = TypeEffect.Eff_Pray;
                break;
            case EmojiType.Sad:
                eff = EffectManager.I.PlayEffect(TypeEffect.Eff_Sad, pos);
                _currentEffectComboMidpoint = TypeEffect.Eff_Sad;
                break;
            default:
                return;
        }
        _currentEffectComboObjMidpoint = eff;

    }

    public void HideEffMidpoint()
    {
        if (_currentEffectComboMidpoint != null)
        {
            EffectManager.I.HideEffectOne(_currentEffectComboMidpoint.Value, _currentEffectComboObjMidpoint);
            _currentEffectComboObjMidpoint = null;
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
    public void StopCoroutineResetMovement()
    {
        if (resetMovementCoroutine != null)
        {
            StopCoroutine(resetMovementCoroutine);
            Debug.Log($"Stopped reset coroutine for: {this.name}");
            resetMovementCoroutine = null;
        }
        else
        {
            Debug.LogWarning($"No reset coroutine found for: {this.name}");
        }
    }
}







