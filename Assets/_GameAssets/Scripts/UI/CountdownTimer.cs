using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime = 30f; // Tổng thời gian đếm ngược
    private float currentTime;
    public TextMeshProUGUI timerText; // UI hiển thị thời gian
    private static event Action OnRevive;
    private static event Action OnStop;
    private bool isGameOver = false;
    [SerializeField] BlinkEffect blinkEffect;
    private bool blinkAt30Triggered = false;
    private bool blinkAt10Triggered = false;
    void OnEnable()
    {
        RandomCountdownTime();
        DisplayTime();
        OnRevive += HandleRevive;
        OnStop += HandleStopTime;
        blinkEffect.enabled = false;
    }


    private void RandomCountdownTime()
    {
        countdownTime = UnityEngine.Random.Range(10, 15) * 5f;
        currentTime = countdownTime;
    }
       
    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        DisplayTime();

        if (!blinkAt30Triggered && currentTime <= 31f)
        {
            blinkAt30Triggered = true;
            StartCoroutine(BlinkForOneSecond());
        }

        if (!blinkAt10Triggered && currentTime <= 16f)
        {
            blinkAt10Triggered = true;
            StartCoroutine(BlinkForOneSecond());
            SoundManager.I.PlaySFX(TypeSound.SFX_ClockWarning);
        }
        

        if (currentTime <= 0)
        {
            OnTimeUp();
        }
    }
    private IEnumerator BlinkForOneSecond()
    {
        blinkEffect.enabled = true;
        yield return new WaitForSeconds(1f); 
        blinkEffect.enabled = false;
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
        UIManager.I.Get<PanelGamePlay>().gameObject.SetActive(false);
        UIManager.I.Show<PanelGameLose>();
    }
    private void HandleRevive()
    {
        Debug.Log("Player Revived! Reset timer." + countdownTime);
        isGameOver = false;
        currentTime = countdownTime; // Reset thời gian
    }

    private void HandleStopTime()
    {
        isGameOver = true;      
    }
    public static void InvokeRevive()
    {
        OnRevive?.Invoke();
    }
    public static void InvokeStop()
    {
        OnStop?.Invoke();
    }

    void OnDestroy()
    {
        // Hủy đăng ký khi object bị hủy để tránh lỗi
        OnRevive -= HandleRevive;
        OnStop -= HandleStopTime;
    }
}
