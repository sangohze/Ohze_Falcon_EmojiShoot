using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DotGroupController : MonoBehaviour
{
    [Header("Dot Setup")]
    [SerializeField] private List<GameObject> dotObjects; // Kéo các dot sẵn từ Editor
    [SerializeField] private Sprite dotOnSprite;
    [SerializeField] private Sprite dotOffSprite;

    

    private List<Image> dotImages = new List<Image>();

    private void Awake()
    {
        // Cache Image component từ mỗi dot
        foreach (var dot in dotObjects)
        {
            Image img = dot.GetComponent<Image>();
            if (img != null) dotImages.Add(img);
        }
    }

    private void Start()
    {
        UpdateDots();
    }

    public void SetCurrentTargetIndex(int index)
    {
        GamePlayController.I.currentTargetIndex = Mathf.Clamp(index, 0, GamePlayController.I._characterTarget.Length - 1);
        UpdateDots();
    }

    private void UpdateDots()
    {
        int targetCount = GamePlayController.I._characterTarget.Length;

        for (int i = 0; i < dotObjects.Count; i++)
        {
            bool shouldShow = targetCount > 1 && i < targetCount;

            dotObjects[i].SetActive(shouldShow);

            if (shouldShow)
            {
                bool isOn = i <= GamePlayController.I.currentTargetIndex;
                dotImages[i].sprite = isOn ? dotOnSprite : dotOffSprite;
            }
        }
    }
}
