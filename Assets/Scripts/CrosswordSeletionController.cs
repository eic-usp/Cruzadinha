using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CrosswordSelectionController : MonoBehaviour
{
    public GameObject crosswordButtonPrefab;
    public Transform gridLayout;
    public Button backButton;

    private List<string> diseases = new List<string>
    {
        "Chagas",
        "Leishmaniose",
        "Esquistossomose",
        "Leptospirose",
        "Malaria"
    };

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        CreateCrosswordButtons();
    }

    void CreateCrosswordButtons()
    {
        foreach (string disease in diseases)
        {
            GameObject buttonObj = Instantiate(crosswordButtonPrefab, gridLayout);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            
            if (buttonText != null)
            {
                buttonText.text = disease;
            }

            // Store the disease name in the button's onClick event
            string diseaseName = disease; // Create a local copy for the closure
            button.onClick.AddListener(() => OnCrosswordSelected(diseaseName));
        }
    }

    void OnCrosswordSelected(string diseaseName)
    {
        GameSceneManager.Instance.LoadCrosswordGame(diseaseName);
    }

    void OnBackClicked()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }
} 