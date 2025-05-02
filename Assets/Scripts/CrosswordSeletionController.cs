using UnityEngine;
using UnityEngine.UI;

public class CrosswordSelectionController : MonoBehaviour
{
    public GameObject crosswordButtonPrefab;
    public Transform gridLayout;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        CreateCrosswordButtons();
    }

    void CreateCrosswordButtons()
    {
        for (var i = 0; i < GameSceneManager.Instance.Crosswords.Length; i++)
        {
            Crossword crossword = GameSceneManager.Instance.Crosswords[i];
            GameObject buttonObj = Instantiate(crosswordButtonPrefab, gridLayout);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            
            if (buttonText != null)
            {
                buttonText.text = crossword.CrosswordName;
            }
            
            int selectedIndex = i;
            button.onClick.AddListener(() => OnCrosswordSelected(selectedIndex));
        }
    }

    void OnCrosswordSelected(int crosswordIndex)
    {
        GameSceneManager.Instance.LoadCrosswordGame(crosswordIndex);
    }

    void OnBackClicked()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }
} 