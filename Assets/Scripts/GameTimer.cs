using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;  


public class GameTimer : MonoBehaviour
{
    public Text timerText; // referencie um texto UI para mostrar o tempo
    private float timeRemaining;
    private bool timerIsRunning = false;
    public GameObject gameOverPanel;


    public void StartTimer(float seconds)
    {
        timeRemaining = seconds;
        timerIsRunning = true;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        timerIsRunning = false;
        timerText.text = "";
    }

    void Update()
    {
        if (!timerIsRunning)
            return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            timerIsRunning = false;
            TimerEnded();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
        public void OnFinishButtonClicked()
    {
        // Fechar o modal (gameOverPanel)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Desativa o painel do modal
        }

        // Carregar a cena de seleção
        SceneManager.LoadScene("CrosswordSelection");  
    }


    void TimerEnded()
    {
        Debug.Log("Tempo acabou!");
        // Aqui pode chamar método para terminar o jogo, mostrar mensagem, etc.
        gameOverPanel.SetActive(true);  // Mostra o painel de vitória
    }
}
