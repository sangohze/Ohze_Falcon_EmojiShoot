using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime = 30f; // Tổng thời gian đếm ngược
    private float currentTime;
    public TextMeshProUGUI timerText; // UI hiển thị thời gian
    private static event Action OnRevive;
    private bool isGameOver = false;

    void Start()
    {
        currentTime = countdownTime;
        DisplayTime();
        OnRevive += HandleRevive;
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        DisplayTime();

        if (currentTime <= 0)
        {
            OnTimeUp();
        }
    }

    void DisplayTime()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.Max(0, Mathf.FloorToInt(currentTime % 60));
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void OnTimeUp()
    {
        isGameOver = true;
        Debug.Log("Game Over! Time's up.");
        UIManager.I.Hide<PanelGamePlay>();
        UIManager.I.Show<PanelGameLose>();
    }
    private void HandleRevive()
    {
        Debug.Log("Player Revived! Reset timer.");
        isGameOver = false;
        currentTime = countdownTime; // Reset thời gian
    }
    public static void InvokeRevive()
    {
        OnRevive?.Invoke();
    }
    void OnDestroy()
    {
        // Hủy đăng ký khi object bị hủy để tránh lỗi
        OnRevive -= HandleRevive;
    }
}
