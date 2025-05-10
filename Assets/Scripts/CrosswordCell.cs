using TMPro; // Para usar o TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CrosswordCell : MonoBehaviour, IPointerClickHandler
{
    public char letter;
    public TMP_InputField letterInputField; // Substituído para usar InputField
    public Image backgroundImage;
    public bool isSelected = false;
    public int x, y;
    public bool isLocked = false;

    private Color normalColor = Color.white;
    private Color selectedColor = new Color(0.8f, 0.8f, 1f);
    private Color correctColor = new Color(0.6f, 1f, 0.6f);
    private Color incorrectColor = new Color(1f, 0.6f, 0.6f);

    private CrosswordManager crosswordManager;

    void Start()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        backgroundImage.enabled = letter != default;

        // Inicializa o InputField
        if (letterInputField == null)
            letterInputField = GetComponentInChildren<TMP_InputField>(); // Aqui pega o InputField
            Debug.Log("TMP_InputField atribuído: " + (letterInputField != null));  // Verifique se o InputField foi atribuído


        // Desativa a possibilidade de digitar se a célula estiver bloqueada
        letterInputField.interactable = !isLocked;

        crosswordManager = FindFirstObjectByType<CrosswordManager>();
        
        // Configura o InputField para que ele mostre o valor da letra, mas não seja editável caso esteja bloqueado
        if (letterInputField != null)
        {
            letterInputField.text = letter.ToString();
            letterInputField.onValueChanged.AddListener(OnInputValueChanged); // Adiciona ouvinte para alterações
        }
    }

    // Esse método é chamado toda vez que o valor do InputField é alterado
    private void OnInputValueChanged(string newText)
    {
        // Verifica se a nova letra é válida
        if (newText.Length > 0)
        {
            letter = newText[0]; // Pega a primeira letra inserida
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked) return;

        isSelected = !isSelected;
        UpdateVisuals();

        if (isSelected && crosswordManager != null)
        {
            crosswordManager.ShowHint(this);
        }
    }

    public void UpdateVisuals()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }
    }

    public void SetLetter(char value)
    {
        backgroundImage.enabled = true;
        letter = value;
        letterInputField.text = value.ToString(); // Coloca a letra na célula
        isLocked = true;
        letterInputField.interactable = false; // Desativa a interação com o InputField
    }

    public void MarkCorrect()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = correctColor;
            isLocked = true;
            letterInputField.interactable = false; // Desativa a interação quando correto
        }
    }

    public void MarkIncorrect()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = incorrectColor;
        }
    }

    public void ResetCell()
    {
        if (isLocked) return;

        isSelected = false;
        if (letterInputField != null)
        {
            letterInputField.text = ""; // Limpa o campo de texto
        }
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
}
