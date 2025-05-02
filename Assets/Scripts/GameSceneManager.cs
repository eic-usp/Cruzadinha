using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [field: SerializeField] public Crossword[] Crosswords { get; private set; }
    
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

    public void LoadCrosswordGame(int crosswordIndex)
    {
        PlayerPrefs.SetInt("SelectedDisease", crosswordIndex);
        SceneManager.LoadScene("CrosswordGame");
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }
} 