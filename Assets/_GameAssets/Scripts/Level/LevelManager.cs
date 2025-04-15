using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static RootMotion.FinalIK.InteractionTrigger;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData[] levels;
    private int currentLevelIndex;
    public int currentTargetIndex;

    private GameObject currentMap;
    private GameObject currentWeapon;
    private List<CharacterController> currentEnemyTargets = new List<CharacterController>();
    public EmojiType currentEmojiTypeTarget;
    public List<CharacterController> CurrentListEnemy;
  
    public bool _isTest;
    [SerializeField] private LevelTest _LevelTest;
    [SerializeField] private CharacterTarget[] _characterTarget;
    private int currentlevelIndexText;
    public List<EmojiType> selectedEmojiTypesPerCharacter = new List<EmojiType>();


    void OnEnable()
    {
        GameManager.Instance.clickArrow = true;
        if (_isTest)
        {
            LoadLevelTest();
            GamePlayController.I._characterTarget = _LevelTest._characterTarget;
        }
        else
        {
            currentLevelIndex = ES3.Load<int>("currentLevelIndex", 0);
            LoadLevel(currentLevelIndex);
            GamePlayController.I._characterTarget = _characterTarget;
        }
        SetUpLeveLGamePlay();
    }

    private void Start()
    {
        currentTargetIndex = 0;   
    }
    public void SetUpLeveLGamePlay()
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
            GamePlayController.I.currentLevelIndexText = currentlevelIndexText;
            GamePlayController.I.CurrentListEnemy = CurrentListEnemy;
            SetUpEnemyTarget();
            GamePlayController.I.EmojiTypeTarget = currentEmojiTypeTarget;
            foreach (var enemy in currentEnemyTargets)
            {
                enemy.SetAsEnemyTarget();
            }
        }
        SetUpUI();
    }
    private void SetUpUI()
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

            UIManager.I.Get<PanelGamePlay>().PreviewAvatar.sprite = levels[currentLevelIndex]._characterTarget[currentTargetIndex].PreviewCharaterTarget;
            UIManager.I.Get<PanelGamePlay>().PreviewEmoji.sprite = levels[currentLevelIndex]._characterTarget[currentTargetIndex].PreviewEmojiTarget;
            UIManager.I.Get<PanelGamePlay>().emojiShowRandom = levels[currentLevelIndex].selectedEmojiTypesPerCharacter;
            if (levels[currentLevelIndex]._characterTarget[currentTargetIndex].PreviewCharaterTarget2 != null)
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(true);
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.sprite = levels[currentLevelIndex]._characterTarget[currentTargetIndex].PreviewCharaterTarget2;
            }
            else
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(false);
            }
        }
    }

    public void LoadLevel(int index)
    {
        Debug.LogError("SangLevel" + (index+1) );
        if (index < 0 || index >= levels.Length) return;
        currentEnemyTargets.Clear();
        CurrentListEnemy.Clear();

        LevelData level = levels[index];

        currentlevelIndexText = levels[index].level;
        currentMap = Instantiate(level.map, level.map.transform.position, Quaternion.identity);

        _characterTarget = level._characterTarget;

        currentEmojiTypeTarget = _characterTarget[currentTargetIndex].EmojiTypeTarget;

        selectedEmojiTypesPerCharacter = level.selectedEmojiTypesPerCharacter;
        // Đặt vũ khí vào camera
        // Set vị trí camera
        Camera.main.transform.position = level.cameraPosition;
        Camera.main.transform.rotation = level.cameraRotation;
        Vector3 spawnPosition = new Vector3(0, 1, 0); // Chỉnh vị trí spawn phù hợp
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 2f, NavMesh.AllAreas))
        {
            foreach (CharacterController charactersPrefab in level.characters)
            {
                Quaternion spawnRot = Quaternion.Euler(0, 90, 0);
                CharacterController enemy = Instantiate(charactersPrefab, GetRandomSpawnPosition(level), spawnRot);
                CurrentListEnemy.Add(enemy);
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy vị trí hợp lệ trên NavMesh!");
        }
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
        if (currentLevelIndex < levels.Length)
        {

        }
        else
        {
            currentLevelIndex = 0;
            ES3.Save("currentLevelIndex", currentLevelIndex);
        }
    }

    private Vector3 GetRandomSpawnPosition(LevelData lv)
    {
        Quaternion rotation = lv.cameraRotation;
        float randomDistance = UnityEngine.Random.Range(7.5f, 10f);
        Vector3 spawnPosition = lv.cameraPosition + (rotation * Vector3.forward * randomDistance);
        spawnPosition.y = lv.cameraPosition.y +10f;
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





    private void LoadLevelTest()
    {
        _LevelTest.LoadLevelTest();
    }
}
