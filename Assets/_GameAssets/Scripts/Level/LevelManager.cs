using System.Collections.Generic;
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
  
    [SerializeField] private bool _isTest;
    [SerializeField] private LevelTest _LevelTest;
    [SerializeField] private CharacterTarget[] _characterTarget;


    void OnEnable()
    {
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
    }

    private void Start()
    {
        currentTargetIndex = 0;
        SetUpLeveLGamePlay();
    }
    public void SetUpLeveLGamePlay()
    {
        if (_isTest)
        {
            //GamePlayController.I.enemyTargets = _LevelTest.currentEnemyTargets;
            GamePlayController.I.EmojiTypeTarget = _LevelTest.currentEmojiTypeTarget;
            GamePlayController.I.CurrentListEnemy = _LevelTest.CurrentListEnemy;
            foreach (var enemy in GamePlayController.I._characterTarget[currentTargetIndex].EnemyTarget)
            {
                enemy.SetAsEnemyTarget();
            }
        }
        else
        {
            //GamePlayController.I.enemyTargets = currentEnemyTargets;
            GamePlayController.I.EmojiTypeTarget = currentEmojiTypeTarget;
            GamePlayController.I.CurrentListEnemy = CurrentListEnemy;
            foreach (var enemy in GamePlayController.I._characterTarget[currentTargetIndex].EnemyTarget)
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
            UIManager.I.Get<PanelGamePlay>().PreviewAvatar.sprite = _LevelTest._characterTarget[currentTargetIndex].PreviewCharaterTarget;
            UIManager.I.Get<PanelGamePlay>().PreviewEmoji.sprite = _LevelTest._characterTarget[currentTargetIndex].PreviewEmojiTarget;
            if (_LevelTest._characterTarget[currentTargetIndex].PreviewCharaterTarget2 != null)
            {
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.gameObject.SetActive(true);
                UIManager.I.Get<PanelGamePlay>().PreviewAvatar2.sprite = _LevelTest._characterTarget[currentTargetIndex].PreviewCharaterTarget2;
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
        if (index < 0 || index >= levels.Length) return;
        currentEnemyTargets.Clear();
        CurrentListEnemy.Clear();

        LevelData level = levels[index];

        
        currentMap = Instantiate(level.map, level.map.transform.position, Quaternion.identity);

        _characterTarget = level._characterTarget;

        currentEmojiTypeTarget = _characterTarget[0].EmojiTypeTarget;

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
                if (level._characterTarget[0].EnemyTarget.Contains(charactersPrefab))
                {
                    currentEnemyTargets.Add(enemy);
                }
            }

        }
        else
        {
            Debug.LogError("Không tìm thấy vị trí hợp lệ trên NavMesh!");
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
        float randomDistance = Random.Range(6f, 9f);
        Vector3 spawnPosition = lv.cameraPosition + (rotation * Vector3.forward * randomDistance);
        spawnPosition.y = lv.cameraPosition.y;
        spawnPosition += rotation * Vector3.right * Random.Range(-5f, 5f);
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
