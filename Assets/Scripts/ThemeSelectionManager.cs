using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeSelectionManager : MonoBehaviour
{
    public GameObject levelSelectionModal;  // referencie o painel modal no inspector
    private int selectedThemeIndex;

    public void OnThemeSelected(int themeIndex)
    {
        selectedThemeIndex = themeIndex;

        // Mostra o modal para escolher o nível
        levelSelectionModal.SetActive(true);
    }

    public void OnLevelSelected(int level)
    {
        // Salva a escolha no PlayerPrefs para usar na próxima cena
        PlayerPrefs.SetInt("SelectedTheme", selectedThemeIndex);
        PlayerPrefs.SetInt("SelectedLevel", level);

        // Carrega a cena do jogo da cruzadinha
        SceneManager.LoadScene("CrosswordGameScene");
    }

    public void OnCancelLevelSelection()
    {
        levelSelectionModal.SetActive(false);
    }
}
