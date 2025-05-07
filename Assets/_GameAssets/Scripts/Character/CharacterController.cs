using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;



public class CharacterController : MonoBehaviour
{
    public Animator animator;
    public CharacterMove characterMove;
    public bool isEnemyTarget;
    public Quaternion characterRotationDefault;
    private Dictionary<EmojiType, ParticleSystem> emojiToEffectMap = new Dictionary<EmojiType, ParticleSystem>();
    private Coroutine resetMovementCoroutine;
    public Coroutine ResetAfterDelayPistol;
    public bool isMan;
    private Transform headPosition;
    private Transform eyesPosition;
    private Transform mouthPosition;
    private Transform handPosition;
    [SerializeField] private Transform parentPos;
    [SerializeField] private Transform handParentPos;
    public int characterID;
    public Sprite Avatar;
    private AudioCueKey _currentSFXKey;
    public float timeEndAnim = 10f;
    public Dictionary<EmojiType, TypeEffect> emojiEffectMap;
    private TypeEffect? _currentEffectSingle;
    private List<(TypeEffect type, GameObject obj)> _currentEffectComboObj = new();
    private GameObject _currentEffectSingleObj;
    private TypeEffect? _currentEffectComboMidpoint;
    private GameObject? _currentEffectComboObjMidpoint;
    private Vector3 effectPositions = new Vector3(0, 2.4f, 0);
    private BulletCollisionHandler bulletHandler;
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
        { EmojiType.Vomit, TypeEffect.Eff_VomitSingle},
        { EmojiType.Talkative, TypeEffect.Eff_TalkativeSingle},
        { EmojiType.Scared, TypeEffect.Eff_ScraredSingle},
        { EmojiType.Shit, TypeEffect.Eff_ShitSingle},
    };
    }
    private Dictionary<EmojiType, List<EffectData>> _effectConfigs = new()
{
    {
        EmojiType.Angry, new List<EffectData>
        {
            new EffectData(TypeEffect.Eff_FireAngry, e => e.headPosition.transform, Vector3.zero, Vector3.one),
        }
    },
    {
        EmojiType.Vomit, new List<EffectData>
        {
            new EffectData(TypeEffect.Eff_Vomit, e => e.mouthPosition.transform, Vector3.zero, Vector3.one),
        }
    },
    {
        EmojiType.Shit, new List<EffectData>
        {
            new EffectData(TypeEffect.Eff_ShitSmoke, e => e.mouthPosition.transform, Vector3.zero, new Vector3(3, 3, 3)),
            //new EffectData(TypeEffect.Eff_shitHand, e => e.handPosition.transform, Vector3.zero, Vector3.one),
        }
    },
    {
        EmojiType.Sad, new List<EffectData>
        {
            new EffectData(TypeEffect.Eff_SadClould, e => e.eyesPosition.transform, Vector3.zero, Vector3.one),
        }
    },
    {
        EmojiType.Talkative, new List<EffectData>
        {
            new EffectData(TypeEffect.Eff_FloatText, e => e.mouthPosition.transform, Vector3.zero, Vector3.one),
        }
    },
};

    private readonly Dictionary<EmojiType, TypeEffect> _midpointEffectMap = new()
{
    { EmojiType.Devil, TypeEffect.Eff_Devil },
    { EmojiType.Angry, TypeEffect.Eff_Smoke },
    { EmojiType.Dance, TypeEffect.Eff_Dance },
    { EmojiType.Pray, TypeEffect.Eff_Pray },
    { EmojiType.Sad, TypeEffect.Eff_Sad },
    { EmojiType.Shit, TypeEffect.Eff_ShitCombo },
    { EmojiType.Scared, TypeEffect.Eff_Devil }, // Note: sẽ xử lý riêng bên dưới
};

    void Start()
    {
        InitEffectMap();
        SetUpPostionEffect();
        SetUpRotationLookAtCamera();
        bulletHandler = new BulletCollisionHandler(this);
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
        handPosition = new GameObject("Hand").GetComponent<Transform>();

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

        handPosition.transform.SetParent(handParentPos, worldPositionStays: false);
        handPosition.transform.localPosition = new Vector3(0, -0.00041f, 0.00073f);
        handPosition.transform.localRotation = Quaternion.Euler(-90, -0.007f, -0.028f);
        handPosition.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

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

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("EmojiProjectileBullet"))
        {
            Vector3 pos = other.ClosestPoint(transform.position);
            EffectManager.I.PlayEffect(TypeEffect.Eff_Hit, pos);
            bulletHandler.HandleCollision();
        }
    }

    private void HandleCollision()
    {
        if (EmojiController.I == null) return;
        EmojiType currentEmoji = EmojiController.I.currentEmoji;
        string animStateSingle = currentEmoji.ToString();
        string animStateDouble = $"{currentEmoji}2";
        if (GamePlayController.I.firstHitEnemy == null || GamePlayController.I.firstHitEnemy == this || GamePlayController.I.firstHitEmoji != currentEmoji || GamePlayController.I.firstHitEmoji == null)
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

    public void HandleFirstHit(string animState, EmojiType currentEmoji)
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

    public void HandleSecondHit(string animState, EmojiType currentEmoji)
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
                GamePlayController.I.HideGhostAnim();
                GamePlayController.I.secondHitEnemy.HideEffCombo();
                GamePlayController.I.secondHitEnemy.characterMove.StopMoving();
                GamePlayController.I.secondHitEnemy.characterMove.MoveTowardsEnemy(characterMove, currentEmoji, (midpoint) =>
                {

                    HideEffOne();
                    GamePlayController.I.secondHitEnemy.HideEffOne();
                    this.animator.CrossFade(animState, 0, 0);

                    if (currentEmoji == EmojiType.Shit)
                    {
                        delayedEmoji = currentEmoji;
                        delayedMidpoint = midpoint;// lưu lại để dùng trong delay
                        PlayShitHandEffect(this);
                        PlayShitHandEffect(GamePlayController.I.secondHitEnemy);
                        Invoke(nameof(DelayedComboEffects), 1.5f);
                    }
                    else
                    {
                        PlayEffectComboMidPoint(midpoint, currentEmoji);
                        PlaySoundFXCombo(currentEmoji, this);
                    }
                    SpawnEmojiEffectSingle(currentEmoji);
                    PlayEffectCombo(this, currentEmoji);

                    GamePlayController.I.secondHitEnemy.animator.CrossFade(animState, 0, 0);
                    GamePlayController.I.secondHitEnemy.SpawnEmojiEffectSingle(currentEmoji);
                    GamePlayController.I.secondHitEnemy.PlayEffectCombo(GamePlayController.I.secondHitEnemy, currentEmoji);
                    StopAllCharaterMoving();
                    PlayAnimationForRemainingEnemies(currentEmoji, () =>
                        {
                        });
                    if (currentEmoji == EmojiType.Scared)
                    {
                        this.characterMove.RestartMovement(Characteranimationkey.DevilRemaining);
                        GamePlayController.I.secondHitEnemy.characterMove.RestartMovement(Characteranimationkey.DevilRemaining);
                        Invoke(nameof(HideEffMidpoint), 1f);
                    }

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

    private EmojiType delayedEmoji;
    private Vector3 delayedMidpoint;

    void DelayedComboEffects()
    {
        delayedMidpoint.y += 1f;
        PlayEffectComboMidPoint(delayedMidpoint, delayedEmoji);
        PlaySoundFXCombo(delayedEmoji, this);
    }

    public void StopAllCharaterMoving()
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

    public void SpawnEmojiEffectSingle(EmojiType emoji)
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
        if (_currentEffectSingle != null)
        {
            EffectManager.I.HideEffectOne(_currentEffectSingle.Value, _currentEffectSingleObj);
            _currentEffectSingle = null;
        }
        HideEffCombo();
        HideEffMidpoint();
    }


    public void ResetAllCharacters()
    {

        foreach (var enemy in GamePlayController.I.CurrentListEnemy)
        {

            enemy.resetMovementCoroutine = enemy.StartCoroutine(ResetCharacterStateAll(enemy));
        }
    }

    private IEnumerator ResetCharacterStateAll(CharacterController character)
    {
        yield return new WaitForSeconds(timeEndAnim);
        if (GamePlayController.I.firstHitEnemy == character)
        {
            GamePlayController.I.firstHitEmoji = null;
        }
        character.HideEffOne();
        character.characterMove.RestartMovement(Characteranimationkey.Walking);
        GamePlayController.I.HideGhostAnim();
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

            }
            else
            {
                string animationKey = emojitype == EmojiType.Pray ? Characteranimationkey.PrayRemaining :
                                      emojitype == EmojiType.Devil ? Characteranimationkey.DevilRemaining :
                                      emojitype == EmojiType.Talkative ? Characteranimationkey.Cherring :
                                       emojitype == EmojiType.Scared ? Characteranimationkey.DevilRemaining :
                                       emojitype == EmojiType.Shit ? Characteranimationkey.Vomit :
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
                    enemy.PlayEffectCombo(enemy, emojiTypeRemaining);
                    
                }
                enemy.characterMove.StopMoving();
                enemy.animator.CrossFade(animationKey, 0, 0);
                if (emojitype == EmojiType.Devil || emojitype == EmojiType.Scared)
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

    public void PlayEffectCombo(CharacterController enemy, EmojiType emojiType)
    {
        _currentEffectComboObj = new List<(TypeEffect, GameObject)>();

        if (emojiType == EmojiType.Scared)
        {
            if (GamePlayController.I.secondHitEnemy != null && GamePlayController.I.firstHitEmoji == emojiType)
                return;

            var ghostEffects = SpawnMultipleEffectsAround(
                enemy.transform,
                TypeEffect.Eff_GhotSingle,
                3,
                1f,
                Vector3.one,
                1f
            );

            foreach (var eff in ghostEffects)
            {
                _currentEffectComboObj.Add((TypeEffect.Eff_GhotSingle, eff));
            }

            return;
        }

        if (!_effectConfigs.TryGetValue(emojiType, out var effectList))
            return;

        foreach (var effData in effectList)
        {
            var parent = effData.parentGetter(enemy);
            GameObject effObj = EffectManager.I.PlayEffect(effData.type, transform.position);
            effObj.transform.SetParent(parent, worldPositionStays: false);
            effObj.transform.localPosition = effData.localPosition;
            effObj.transform.localRotation = Quaternion.identity;
            effObj.transform.localScale = effData.localScale;

            _currentEffectComboObj.Add((effData.type, effObj));

            // Optional: Talkative special logic
            if (emojiType == EmojiType.Talkative)
            {
                FloatingText ft = effObj.GetComponent<FloatingText>();
                if (ft != null)
                {
                    ft.spawnRoot = enemy.mouthPosition.transform;
                    ft.floatingTextPrefab = effObj;
                }
            }
        }
    }



    public void PlayEffectComboMidPoint(Vector3 pos, EmojiType emojiType)
    {
        if (!_midpointEffectMap.TryGetValue(emojiType, out var effectType))
            return;

        if (emojiType == EmojiType.Scared)
        {
            GamePlayController.I.HideGhostAnim();
            GamePlayController.I.SpawnGhostAnim(pos);
        }

        GameObject eff = EffectManager.I.PlayEffect(effectType, pos);
        _currentEffectComboMidpoint = effectType;
        _currentEffectComboObjMidpoint = eff;
    }

    public void HideEffMidpoint()
    {
        if (_currentEffectComboMidpoint != null && _currentEffectComboObjMidpoint != null)
        {
            EffectManager.I.HideEffectOne(_currentEffectComboMidpoint.Value, _currentEffectComboObjMidpoint);
            _currentEffectComboObjMidpoint = null;
            _currentEffectComboMidpoint = null;
        }
    }


    public void HideEffCombo()
    {
        if (_currentEffectComboObj != null && _currentEffectComboObj.Count > 0)
        {
            foreach (var (type, obj) in _currentEffectComboObj)
            {
                if (obj != null)
                {
                    EffectManager.I.HideEffectOne(type, obj);
                }
            }

            _currentEffectComboObj.Clear();
        }
    }

    public void PlaySoundFXSingle(EmojiType emojiType, CharacterController enemy)
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
        { EmojiType.Vomit, TypeSound.SFX_Vomit },
        { EmojiType.Talkative, enemy.isMan ? TypeSound.SFX_Talkative_male : TypeSound.SFX_Talkative_female},
        { EmojiType.Scared, TypeSound.SFX_Scared },
        { EmojiType.Shit, TypeSound.SFX_Emoji_Shit },
    };

        if (soundMap.TryGetValue(emojiType, out TypeSound sound))
        {
            _currentSFXKey = SoundManager.I.PlaySFX(sound);
        }
    }

    public void PlaySoundFXCombo(EmojiType emojiType, CharacterController enemy)
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
        { EmojiType.Vomit, TypeSound.SFX_Stinky },
             { EmojiType.Talkative, enemy.isMan ? TypeSound.SFX_Talkative_male : TypeSound.SFX_Talkative_female},
        { EmojiType.Scared, TypeSound.SFX_Scared },
        { EmojiType.Shit, TypeSound.SFX_Combo_Emoji_Shit },
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
            resetMovementCoroutine = null;
        }

    }
    public void StopCoroutineResetAfterDelayPistol()
    {
        if (ResetAfterDelayPistol != null)
        {
            StopCoroutine(ResetAfterDelayPistol);
            ResetAfterDelayPistol = null;
        }
    }

    private List<GameObject> SpawnMultipleEffectsAround(Transform target, TypeEffect effectType, int count, float distance, Vector3 scale, float yOffset)
    {
        List<GameObject> spawnedEffects = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            // Calculate the angle for each effect based on the number of effects
            float angle = (i / (float)count) * 360f; // Evenly spread the effects in a circle
            float angleRad = Mathf.Deg2Rad * angle; // Convert angle to radians

            // Calculate the position based on the angle
            Vector3 spawnPos = new Vector3(
                target.position.x + Mathf.Cos(angleRad) * distance,
                target.position.y + yOffset,
                target.position.z + Mathf.Sin(angleRad) * distance
            );

            // Play effect
            GameObject eff = EffectManager.I.PlayEffect(effectType, spawnPos);

            // Set the parent of the effect
            eff.transform.SetParent(target, worldPositionStays: true);

            // Make the effect face the target
            Vector3 lookAtPos = new Vector3(target.position.x, eff.transform.position.y, target.position.z);
            eff.transform.LookAt(lookAtPos);

            // Fix rotation on the Z axis
            Vector3 euler = eff.transform.eulerAngles;
            euler.z = 0f;
            eff.transform.eulerAngles = euler;

            // Apply scale to the effect
            eff.transform.localScale = scale;

            // Add to the list of spawned effects
            spawnedEffects.Add(eff);
        }

        return spawnedEffects;
    }

    private void PlayShitHandEffect(CharacterController enemy)
    {
        if (enemy == null) return;

        Transform parent = enemy.handPosition.transform;
        GameObject handEff = EffectManager.I.PlayEffect(TypeEffect.Eff_shitHand, parent.position);
        handEff.transform.SetParent(parent, worldPositionStays: false);
        handEff.transform.localPosition = Vector3.zero;
        handEff.transform.localRotation = Quaternion.identity;
        handEff.transform.localScale = Vector3.one;

        _currentEffectComboObj.Add((TypeEffect.Eff_shitHand, handEff));
    }


}
public struct EffectData
{
    public TypeEffect type;
    public Func<CharacterController, Transform> parentGetter;
    public Vector3 localPosition;
    public Vector3 localScale;

    public EffectData(TypeEffect type, Func<CharacterController, Transform> parentGetter, Vector3 localPos, Vector3 localScale)
    {
        this.type = type;
        this.parentGetter = parentGetter;
        this.localPosition = localPos;
        this.localScale = localScale;
    }
}







