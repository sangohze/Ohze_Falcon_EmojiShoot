using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingFill : MonoBehaviour
{
    public Image LoadingBarFill;
    public static bool IsOn = false;

    [SerializeField] private GameObject canvasloading;

    // Thêm biến static để kiểm tra đã load chưa
    private static bool HasLoadedBefore = false;

    private void Awake()
    {
        LoadingBarFill.fillAmount = 0f;
        IsOn = false;

        if (HasLoadedBefore)
        {
            // Nếu đã load rồi -> bỏ qua loading
            canvasloading.SetActive(false);
            IsOn = true;
            GameManager.Instance.clickArrow = true;
        }
        else
        {
            // Chưa load lần nào -> chạy loading
            HasLoadedBefore = true;
            Load();
        }
    }

    public void Load()
    {
        float timeload = 3f;

        LoadingBarFill.fillAmount = 0f;
        LoadingBarFill.DOFillAmount(1, timeload).SetEase(Ease.Linear).OnComplete(() =>
        {
            IsOn = true;
            canvasloading.SetActive(false);
            GameManager.Instance.clickArrow = true;
        });
    }
}
