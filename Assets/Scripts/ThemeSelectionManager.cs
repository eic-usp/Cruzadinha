using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThemeSelectionManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public GameObject levelSelectionModal;
    public int numberOfThemes = 3;

    private int selectedThemeIndex;

    void Start()
    {
        for (int i = 0; i < numberOfThemes; i++)
        {
            int themeIndex = i;
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button btn = buttonObj.GetComponent<Button>();
            btn.GetComponentInChildren<Text>().text = "Tema " + (i + 1);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnThemeSelected(themeIndex));
        }

        levelSelectionModal.SetActive(false);
    }

    public void OnThemeSelected(int themeIndex)
    {
        selectedThemeIndex = themeIndex;
        levelSelectionModal.SetActive(true);
    }

    public void OnLevelSelected(int level)
    {
        PlayerPrefs.SetInt("SelectedTheme", selectedThemeIndex);
        PlayerPrefs.SetInt("SelectedLevel", level);
        SceneManager.LoadScene("CrosswordGameScene");
    }

    public void OnCancelLevelSelection()
    {
        levelSelectionModal.SetActive(false);
    }
}
