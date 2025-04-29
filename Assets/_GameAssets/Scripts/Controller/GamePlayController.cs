using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using static LevelData;

public class GamePlayController : Singleton<GamePlayController>
{
    public List<CharacterController> CurrentListEnemy;
    public int hitCount = 0;
    private bool isWaitingForSecondHit = false;
    public EmojiType EmojiTypeTarget;
    public EmojiType? firstHitEmoji = null;
    public CharacterController firstHitEnemy = null;
    public CharacterController secondHitEnemy = null;
    private float hitResetTime = 5f;
    public Coroutine resetHitCoroutine;
    public Coroutine WaitForSecondHit;
    public CharacterTarget[] _characterTarget;
    public int currentTargetIndex = 0;
    public GameObject tickPreview1;
    public GameObject tickPreview2;
    public GameObject tickTextPistolLevel;
    private float timeToTarget = 10f;
    [SerializeField] UIMoveNew groupMissionShow;
    [SerializeField] DotGroupController groupDOT;
    [SerializeField] GameObject _Effects;
    public bool isTransitioningMission = false;


    private IWeaponGameWinHandler weaponGameWinHandler;
    private PistolInitAnimHandler pistolInitHandler;

    [SerializeField] private List<GameObject> _ghostAnims = new List<GameObject>();
    private List<GameObject> _currentGhostAnims = new List<GameObject>();
    public void InitWeaponLogic(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Bow:
                weaponGameWinHandler = new BowGameWinHandler(this);

                foreach (var enemy in CurrentListEnemy)
                {
                    enemy.characterMove.InitBowLevel();
                }
                break;
            case WeaponType.Pistol:
                weaponGameWinHandler = gameObject.AddComponent<SpecialPistolLevelManager>();
                ((SpecialPistolLevelManager)weaponGameWinHandler).Init(this);

                pistolInitHandler = gameObject.AddComponent<PistolInitAnimHandler>();
                pistolInitHandler.InitCharacterHandler();
                break;
        }
    }

    public void SetTickPreviewByEnemy(EmojiType emoji)
    {
        weaponGameWinHandler?.SetTickPreviewByEnemy(emoji);
    }
    public void OnEnemyTargetHit(CharacterController enemy)
    {
        weaponGameWinHandler?.OnEnemyTargetHit(enemy);
    }
    private void OnGameWin()
    {
        UIManager.I.Show<PanelGameWin>();
    }







    public void WaitGameWin()
    {
        StartCoroutine(IEWaitGameWin());
    }

    public IEnumerator IEWaitGameWin()
    {
        isTransitioningMission = true;
        currentTargetIndex++;
        if (currentTargetIndex >= _characterTarget.Length)
        {
            _Effects.SetActive(true);
        }
        else
        {
            firstHitEmoji = null;
            secondHitEnemy = null;
        }
        if (WaitForSecondHit != null) StopCoroutine(WaitForSecondHit);
        yield return new WaitForSeconds(1.5f);
        if (currentTargetIndex >= _characterTarget.Length)
        {
            CountdownTimer.InvokeStop();
            GameManager.Instance.clickArrow = false;
            UIManager.I.Get<PanelGamePlay>().gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        hitCount = 0;
        if (currentTargetIndex >= _characterTarget.Length)
        {
            OnGameWin();
        }
        else
        {
            tickPreview1.SetActive(false);
            tickPreview2.SetActive(false);
            groupMissionShow.Hide();
            yield return new WaitForSeconds(0.2f);
            LevelManager.I.SetUpLeveLGamePlay(LevelManager.I.currentLevelData);
            GameManager.Instance.clickArrow = true;
            if (WaitForSecondHit != null)
                StopCoroutine(WaitForSecondHit);
        }
        groupDOT.SetCurrentTargetIndex(currentTargetIndex);
        isTransitioningMission = false;
    }

    public IEnumerator IEWaitForSecondHit(int enemyIndex)
    {
        GameObject go = null;
        if (enemyIndex == 0)
        {
            tickPreview1.SetActive(true);
            go = tickPreview1;
        }
        else if (enemyIndex == 1)
        {
            tickPreview2.SetActive(true);
            go = tickPreview2;
        }
        isWaitingForSecondHit = true;
        yield return new WaitForSeconds(timeToTarget);
        tickPreview1.SetActive(false);
        tickPreview2.SetActive(false);

        isWaitingForSecondHit = false;
        if (hitCount < 2)
        {
            hitCount = 0;
        }
    }

    //
    public void SpawnGhostAnim(Vector3 midPoint)
    {
        _currentGhostAnims.Clear();

        float radius = 1.0f; // bán kính rải ghost quanh midPoint
        int ghostCount = _ghostAnims.Count;

        for (int i = 0; i < ghostCount; i++)
        {
            if (_ghostAnims[i] != null)
            {
                // Tính vị trí spawn theo hình tròn xung quanh midPoint
                float angle = (360f / ghostCount) * i * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                Vector3 spawnPosition = new Vector3(midPoint.x, midPoint.y - 3f, midPoint.z) + offset;

                GameObject ghost = LeanPool.Spawn(_ghostAnims[i], spawnPosition, Quaternion.identity, null);
                ghost.GetComponent<GhostFollower>().Init(midPoint, secondHitEnemy.transform, i);


                _currentGhostAnims.Add(ghost);
            }
        }
    }




    public void HideGhostAnim()
    {
        foreach (var ghost in _currentGhostAnims)
        {
            if (ghost != null)
            {
                ghost.GetComponent<GhostFollower>().StopFollowAndDespawn();
            }
        }

        _currentGhostAnims.Clear();
    }



}


