using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Text gameTitle;

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    public void OnStartClicked()
    {
        GameSceneManager.Instance.LoadCrosswordSelection();
        Debug.Log("Iniciando o jogo...");
    }

    public void OnSettingsClicked()
    {
        GameSceneManager.Instance.LoadSettings();
        Debug.Log("Socorro deus");
    }
} 