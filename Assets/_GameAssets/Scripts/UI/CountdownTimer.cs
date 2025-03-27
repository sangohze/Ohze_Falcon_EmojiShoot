using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime = 30f; // Tổng thời gian đếm ngược
    private float currentTime;
    public TextMeshProUGUI timerText; // UI hiển thị thời gian

    private bool isGameOver = false;

    void Start()
    {
        currentTime = countdownTime;
        DisplayTime();
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
}
