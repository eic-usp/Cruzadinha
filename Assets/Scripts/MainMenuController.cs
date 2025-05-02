using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Text gameTitle;

    void Start()
    {
        // Set up button listeners
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);

        // You can customize the game title here
        gameTitle.text = "Cruzadinhas";
    }

    void OnStartClicked()
    {
        GameSceneManager.Instance.LoadCrosswordSelection();
    }

    void OnSettingsClicked()
    {
        GameSceneManager.Instance.LoadSettings();
    }
} 