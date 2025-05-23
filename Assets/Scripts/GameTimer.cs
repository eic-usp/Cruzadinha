using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 60f;      // Tempo em segundos
    private float currentTime;
    private bool timerRunning = false;

    public Text timerText;             // Texto da UI para mostrar o tempo

    public delegate void TimeExpiredHandler();
    public event TimeExpiredHandler OnTimeExpired;

    void Update()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;
            OnTimeExpired?.Invoke();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        currentTime = timeLimit;
        timerRunning = true;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        timerRunning = false;
        timerText.text = "";
    }
}
