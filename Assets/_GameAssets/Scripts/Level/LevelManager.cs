using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static LevelData;
using static RootMotion.FinalIK.InteractionTrigger;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData[] levels;
    public int currentLevelIndex;
    public int currentTargetIndex;

    private GameObject currentMap;
    private GameObject currentWeapon;
    private List<CharacterController> currentEnemyTargets = new List<CharacterController>();
    public EmojiType currentEmojiTypeTarget;
    public List<CharacterController> CurrentListEnemy;

    public bool _isTest;
    [SerializeField] private LevelTest _LevelTest;
    [SerializeField] private CharacterTarget[] _characterTarget;
    public List<EmojiType> selectedEmojiTypesPerCharacter = new List<EmojiType>();

    public LevelDataBatch _LevelDataBatch;
    private Dictionary<WeaponType, Func<LevelData, Vector3>> spawnPositionFuncs;
    public LevelData currentLevelData { get; private set; }
    void OnEnable()
    {
        spawnPositionFuncs = new Dictionary<WeaponType, Func<LevelData, Vector3>>
    {
        { WeaponType.Bow, GetRandomSpawnPositionLevelBow },
        { WeaponType.Pistol, GetRandomSpawnPositionLevelPistol },
        // Thêm weapon khác nếu cần
    };
        GameManager.Instance.clickArrow = true;
        if (_isTest)
        {
            LoadLevelTest();
            GamePlayController.I._characterTarget = _LevelTest._characterTarget;
        }
        else
        {
            currentLevelIndex = ES3.Load<int>("currentLevelIndex", 0);
            if (currentLevelIndex < levels.Length)
            {
                LoadLevel(levels[currentLevelIndex]);             
                SetUpLeveLGamePlay(levels[currentLevelIndex]);
            }
            else
            {
                // Load từ danh sách generatedLevels
                int autoIndex = currentLevelIndex - levels.Length;

                if (_LevelDataBatch != null && _LevelDataBatch.generatedLevels != null && _LevelDataBatch.generatedLevels.Count > 0)
                {
                    // Lặp lại generatedLevels nếu autoIndex vượt quá
                    autoIndex = autoIndex % _LevelDataBatch.generatedLevels.Count;
                    LoadLevel(_LevelDataBatch.generatedLevels[autoIndex]);
                    SetUpLeveLGamePlay(_LevelDataBatch.generatedLevels[autoIndex]);
                }
                else
                {
                    Debug.LogWarning("⚠️ LevelDataBatch trống hoặc null. Không thể load level.");
                }
            }
            GamePlayController.I._characterTarget = _characterTarget;

        }
    }

    private void Start()
    {
        currentTargetIndex = 0;

    }
    public void SetUpLeveLGamePlay(LevelData level)
    {
        if (_isTest)
        {
            _LevelTest.currentTargetIndex = GamePlayController.I.currentTargetIndex;
            GamePlayController.I.CurrentListEnemy = _LevelTest.CurrentListEnemy;
            _LevelTest.SetUpEnemyTarget();
            GamePlayController.I.EmojiTypeTarget = _LevelTest.currentEmojiTypeTarget;
            foreach (var enemy in _LevelTest.currentEnemyTargets)
            {
                enemy.SetAsEnemyTarget();

            }
        }
        else
        {
            currentTargetIndex = GamePlayController.I.currentTargetIndex;
            GamePlayController.I.CurrentListEnemy = CurrentListEnemy;
            SetUpEnemyTarget();


            GamePlayController.I.EmojiTypeTarget = currentEmojiTypeTarget;
            EmojiController.I.selectedEmojiTypesPerCharacter = selectedEmojiTypesPerCharacter;
            foreach (var enemy in currentEnemyTargets)
            {
                enemy.SetAsEnemyTarget();
            }
        }

        SetUpUI(level);

        if (EmojiController.I != null)
        {
            GamePlayController.I.SetTickPreviewByEnemy(EmojiController.I.currentEmoji);
        }
    }
    private void SetUpUI(LevelData level)
    {

        if (_isTest)
        {
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar.sprite = _LevelTest._characterTarget[_LevelTest.currentTargetIndex].PreviewCharaterTarget;
            UIManager.I.Get<PanelGamePlay>().PreviewEmoji.sprite = _LevelTest._characterTarget[_LevelTest.currentTargetIndex].PreviewEmojiTarget;
            UIManager.I.Get<PanelGamePlay>().emojiShowRandom = _LevelTest.selectedEmojiTypesPerCharacter;
            if (_LevelTest._characterTarget[_LevelTest.currentTargetIndex].PreviewCharaterTarget2 != null)
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(true);
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.sprite = _LevelTest._characterTarget[_LevelTest.currentTargetIndex].PreviewCharaterTarget2;
            }
            else
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(false);
            }
        }
        else
        {

            SetUpMissiopnBoard(level);
            UIManager.I.Get<PanelGamePlay>().emojiShowRandom = level.selectedEmojiTypesPerCharacter;
        }
    }

    private void SetUpMissiopnBoard(LevelData level)
    {
        if (level.playerWeapon == WeaponType.Pistol)
        {
            UIManager.I.Get<PanelGamePlay>()._textPistolLevel.gameObject.SetActive(true);
            UIManager.I.Get<PanelGamePlay>()._textPistolLevel.text = level._characterTarget[currentTargetIndex].PistolLevelTextMission;
            UIManager.I.Get<PanelGamePlay>().PreviewEmoji.gameObject.SetActive(false);
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar.gameObject.SetActive(false);
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(false);
        }
        else
        {
            UIManager.I.Get<PanelGamePlay>()._textPistolLevel.gameObject.SetActive(false);
            UIManager.I.Get<PanelGamePlay>().PreviewEmoji.gameObject.SetActive(true);
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar.gameObject.SetActive(true);
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(true);
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar.sprite = level._characterTarget[currentTargetIndex].PreviewCharaterTarget;
            UIManager.I.Get<PanelGamePlay>().PreviewEmoji.sprite = level._characterTarget[currentTargetIndex].PreviewEmojiTarget;
            if (level._characterTarget[currentTargetIndex].PreviewCharaterTarget2 != null)
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(true);
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.sprite = level._characterTarget[currentTargetIndex].PreviewCharaterTarget2;
            }
            else
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(false);
            }
        }
    }

    public void LoadLevel(LevelData level)
    {
        if (level == null)
        {
            Debug.LogError("❌ LevelData null, không thể load level.");
            return;
        }

        Debug.LogError("🔁 Sang Level: " + currentLevelIndex);

        currentLevelData = level;

        currentMap = Instantiate(level.map, level.map.transform.position, Quaternion.identity);

        _characterTarget = level._characterTarget;

        currentEmojiTypeTarget = _characterTarget[currentTargetIndex].EmojiTypeTarget;

        selectedEmojiTypesPerCharacter = level.selectedEmojiTypesPerCharacter;
        PlayerController.I._playerWeapon = level.playerWeapon;
        PlayerController.I.CheckWeaponInLevel();

        Camera.main.transform.position = level.cameraPosition;
        Camera.main.transform.rotation = level.cameraRotation;
        Vector3 spawnPosition = new Vector3(0, 1, 0); // Chỉnh vị trí spawn phù hợp
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 2f, NavMesh.AllAreas))
        {
            foreach (CharacterController charactersPrefab in level.characters)
            {
                Quaternion spawnRot = Quaternion.Euler(0, 90, 0);
                Vector3 spawnPos = spawnPositionFuncs[level.playerWeapon].Invoke(level);
                CharacterController enemy = Instantiate(charactersPrefab, spawnPos, spawnRot);

                CurrentListEnemy.Add(enemy);
            }
            //Invoke(nameof(InitWeaponDelayed), 0f);         
        }
        else
        {
            Debug.LogError("Không tìm thấy vị trí hợp lệ trên NavMesh!");
        }
        Invoke(nameof(InitWeaponDelayed), 0f);
    }
    void InitWeaponDelayed()
    {
        GamePlayController.I.InitWeaponLogic(PlayerController.I._playerWeapon);
    }

    public void SetUpEnemyTarget()
    {
        currentEmojiTypeTarget = _characterTarget[currentTargetIndex].EmojiTypeTarget;
        currentEnemyTargets.Clear();

        if (currentTargetIndex < 0 || currentTargetIndex >= _characterTarget.Length)
        {
            Debug.LogWarning("currentTargetIndex out of range!");
            return;
        }
        foreach (var enemy in CurrentListEnemy)
        {
            enemy.isEnemyTarget = false;
        }
        List<CharacterController> enemyTargetList = _characterTarget[currentTargetIndex].EnemyTarget;

        foreach (CharacterController enemy in CurrentListEnemy)
        {
            // So sánh dựa trên prefab gốc
            if (enemyTargetList.Any(prefab => prefab.name == enemy.name.Replace("(Clone)", "").Trim()))
            {
                currentEnemyTargets.Add(enemy);
            }
        }
    }


    public void NextLevel()
    {
        currentLevelIndex++;
        ES3.Save("currentLevelIndex", currentLevelIndex);
        ES3.Save("currentLevelIndex", currentLevelIndex);
    }

    private Vector3 GetRandomSpawnPositionLevelBow(LevelData lv)
    {
        Quaternion rotation = lv.cameraRotation;
        float randomDistance = UnityEngine.Random.Range(8.2f, 10f);
        Vector3 spawnPosition = lv.cameraPosition + (rotation * Vector3.forward * randomDistance);
        spawnPosition.y = lv.cameraPosition.y + 10f;
        spawnPosition += rotation * Vector3.right * UnityEngine.Random.Range(-5f, 5f);
        Vector3[] offsets = {
        Vector3.zero, Vector3.right * 2.5f, Vector3.left * 2.5f,
        Vector3.forward * 2.5f, Vector3.back * 2.5f
    };

        RaycastHit[] hits = new RaycastHit[1];
        foreach (var offset in offsets)
        {
            Vector3 adjustedSpawn = spawnPosition + offset;
            Vector3 topPosition = adjustedSpawn + Vector3.up * 10f;


            if (Physics.RaycastNonAlloc(topPosition, Vector3.down, hits, 25f) > 0)
            {
                if (NavMesh.SamplePosition(hits[0].point, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas))
                {
                    return navMeshHit.position;
                }
            }
        }

        Debug.LogWarning("Không tìm thấy vị trí hợp lệ trên bất kỳ NavMesh nào!");
        return spawnPosition;
    }


    private Vector3 GetRandomSpawnPositionLevelPistol(LevelData lv)
    {
        Quaternion rotation = lv.cameraRotation;
        float forwardDistance = UnityEngine.Random.Range(8f, 9f);
        //float forwardDistance = 8f;
        float sideOffset = UnityEngine.Random.Range(-3f, 3f);
        float heightOffset = 10f;

        // Vị trí cơ bản phía trước camera
        Vector3 spawnBase = lv.cameraPosition + (rotation * Vector3.forward * forwardDistance);
        spawnBase += rotation * Vector3.right * sideOffset;
        spawnBase.y += heightOffset;

        // Các offset phụ để tìm vị trí gần nhất có thể đứng được
        Vector3[] offsets = {
        Vector3.zero, Vector3.right * 2f, Vector3.left * 2f,
        Vector3.forward * 2f, Vector3.back * 2f
    };

        RaycastHit[] hits = new RaycastHit[1];
        foreach (var offset in offsets)
        {
            Vector3 adjusted = spawnBase + offset;
            Vector3 topPosition = adjusted + Vector3.up * 10f;

            if (Physics.RaycastNonAlloc(topPosition, Vector3.down, hits, 25f) > 0)
            {
                if (NavMesh.SamplePosition(hits[0].point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }
        }

        Debug.LogWarning("Không tìm thấy vị trí hợp lệ cho Pistol! Trả về vị trí cơ bản.");
        return spawnBase;
    }


    private void LoadLevelTest()
    {
        _LevelTest.LoadLevelTest();
    }
}
