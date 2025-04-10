using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    
     private int currentLevel ;
     private int totalLevels ;

    private void Start()
    {
        totalLevels = LevelManager.I.levels.Length;
        currentLevel = GamePlayController.I.currentLevelIndexText;
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

    private float CalculateFillAmount(int level, int maxLevel)
    {
        if (level <= 1) return 0f;
        if (level == 2) return 0.25f;
        if (level >= 3 && level < maxLevel -1) return 0.5f;
        if (level >= maxLevel -1 && level < maxLevel) return 0.75f;
        return 1f;
    }

    private void UpdateLevelImages()
    {
        int totalSlots = levelImages.Length;

        for (int i = 0; i < totalSlots; i++)
        {
            // Gán mặc định là chưa chơi
            levelImages[i].sprite = notYetSprite;
        }

        // Gán sprite theo currentLevel
        if (currentLevel <= 1)
        {
            levelImages[0].sprite = inProgressSprite;
        }
        else if (currentLevel == 2)
        {
            levelImages[0].sprite = doneSprite;
            levelImages[1].sprite = inProgressSprite;
        }
        else if (currentLevel >= 3 && currentLevel < totalLevels - 1)
        {
            levelImages[0].sprite = doneSprite;
            levelImages[1].sprite = doneSprite;
            levelImages[2].sprite = inProgressSprite;
        }
        else if (currentLevel == totalLevels - 1)
        {
            levelImages[0].sprite = doneSprite;
            levelImages[1].sprite = doneSprite;
            levelImages[2].sprite = doneSprite;
            levelImages[3].sprite = inProgressSprite;
        }
        else if (currentLevel >= totalLevels)
        {
            for (int i = 0; i < totalSlots - 1; i++)
            {
                levelImages[i].sprite = doneSprite;
            }
            levelImages[totalSlots - 1].sprite = inProgressSprite;
        }

        // Optional: animation
        foreach (var img in levelImages)
        {
            img.transform.DOScale(Vector3.one * 1.05f, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }
    }


}
