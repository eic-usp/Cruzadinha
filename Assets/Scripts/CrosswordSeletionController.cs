using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrosswordSelectionController : MonoBehaviour
{
    public GameObject crosswordButtonPrefab;
    public GameObject levelSelectionModal;

    public Transform gridLayout;
    public Button backButton;
    private int selectedCrosswordIndex = -1;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        CreateCrosswordButtons();
        levelSelectionModal.SetActive(false); // modal começa desativado
    }

    void CreateCrosswordButtons()
    {
        for (var i = 0; i < GameSceneManager.Instance.Crosswords.Length; i++)
        {
            Crossword crossword = GameSceneManager.Instance.Crosswords[i];
            GameObject buttonObj = Instantiate(crosswordButtonPrefab, gridLayout);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = crossword.CrosswordName;
            }

            int selectedIndex = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCrosswordSelected(selectedIndex));
        }
    }

    void OnCrosswordSelected(int crosswordIndex)
    {
        selectedCrosswordIndex = crosswordIndex;
        levelSelectionModal.SetActive(true);
        Debug.Log("Crossword selecionado: " + crosswordIndex);
        //GameSceneManager.Instance.LoadCrosswordGame(crosswordIndex);
    }
     public void OnLevelSelected(int level)
    {
        PlayerPrefs.SetInt("SelectedCrosswordIndex", selectedCrosswordIndex);
        PlayerPrefs.SetInt("SelectedLevel", level);

        GameSceneManager.Instance.LoadCrosswordGame(selectedCrosswordIndex);
    }

    public void OnCancelLevelSelection()
    {
        levelSelectionModal.SetActive(false);
    }


    void OnBackClicked()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }
} 