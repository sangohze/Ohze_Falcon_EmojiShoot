using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static LevelData;

public class LevelProgress : MonoBehaviour
{
    [Header("Progress Fill UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillDuration = 0.5f;

    [Header("Level Image UI")]
    [SerializeField] private Image[] levelImages; // 5 hình ảnh thể hiện tiến trình

    [Header("Level Sprites")]
    [SerializeField] private Sprite doneSprite;
    [SerializeField] private Sprite inProgressSprite;
    [SerializeField] private Sprite notYetSprite;

    [Header("Special Weapon Sprite")]
    [SerializeField] private Sprite specialSprite;
    private int currentLevel ;
     private int totalLevels ;

    private void Start()
    {


        totalLevels = 3;

        currentLevel = Mathf.Clamp(LevelManager.I.currentLevelIndex + 1, 1, totalLevels);
        SetCurrentLevel(currentLevel);
    }

    public void SetCurrentLevel(int newLevel)
    {
        currentLevel = Mathf.Clamp(newLevel, 1, totalLevels);
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateProgressFill();
        UpdateLevelImages();
    }

    private void UpdateProgressFill()
    {
        float fillAmount = CalculateFillAmount(currentLevel, totalLevels);
        fillImage.DOFillAmount(fillAmount, fillDuration).SetEase(Ease.OutQuad);
    }

    private float CalculateFillAmount(int level, int _)
    {
        if (level <= 1) return 0f;
        if (level == 2) return 0.25f;
        return 0.5f; // từ level 3 trở đi là full
    }

    private void UpdateLevelImages()
    {
        int totalSlots = levelImages.Length;

        // Reset tất cả về notYetSprite
        for (int i = 0; i < totalSlots; i++)
        {
            levelImages[i].sprite = notYetSprite;
        }

        var levelData = LevelManager.I.currentLevelData;

        // Cập nhật 3 hình đầu: done / inProgress / special
        UpdateBaseLevelImages(levelData);

        // Cập nhật hình 4 và 5 nếu các level kế tiếp là Pistol
        UpdateUpcomingSpecialSprites();
    }

    private void UpdateBaseLevelImages(LevelData levelData)
    {
        int levelDisplay = Mathf.Clamp(currentLevel, 1, 3);

        for (int i = 0; i < 3; i++)
        {
            if (i < levelDisplay - 1)
            {
                levelImages[i].sprite = doneSprite;
            }
            else if (i == levelDisplay - 1)
            {
                levelImages[i].sprite = IsPistol(levelData) ? specialSprite : inProgressSprite;
                if (levelImages[i].GetComponentInParent<UIScale>() is UIScale scaleInParent)
                {
                    scaleInParent.Show();
                }
                else
                {
                    Debug.Log("Sangnon");
                }    
            }
        }
    }

    private void UpdateUpcomingSpecialSprites()
    {
        for (int i = 0; i < 2; i++)
        {
            int checkIndex = LevelManager.I.currentLevelIndex + 1 + i;

            if (IsPistolLevelAtIndex(checkIndex))
            {
                int imageIndex = i + 3;
                if (imageIndex < levelImages.Length)
                {
                    levelImages[imageIndex].sprite = specialSprite;
                }
            }
        }
    }

    private bool IsPistolLevelAtIndex(int index)
    {
        if (index < LevelManager.I.levels.Length)
        {
            return LevelManager.I.levels[index].playerWeapon == WeaponType.Pistol;
        }

        int autoIndex = index - LevelManager.I.levels.Length;

        if (LevelManager.I._LevelDataBatch?.generatedLevels?.Count > 0)
        {
            autoIndex = autoIndex % LevelManager.I._LevelDataBatch.generatedLevels.Count;
            return LevelManager.I._LevelDataBatch.generatedLevels[autoIndex].playerWeapon == WeaponType.Pistol;
        }

        return false;
    }

    private bool IsPistol(LevelData level)
    {
        return level != null && level.playerWeapon == WeaponType.Pistol;
    }
}
