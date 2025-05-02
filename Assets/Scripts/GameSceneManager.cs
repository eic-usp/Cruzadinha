using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCrosswordSelection()
    {
        SceneManager.LoadScene("CrosswordSelection");
    }

    public void LoadCrosswordGame(string diseaseName)
    {
        PlayerPrefs.SetString("SelectedDisease", diseaseName);
        SceneManager.LoadScene("CrosswordGame");
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }
} 