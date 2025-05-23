using TMPro; // Para usar o TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CrosswordCell : MonoBehaviour
{
    public char letter;
    public char expectedLetter;
    public string clue;


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
        {
            backgroundImage = GetComponent<Image>();
        }

        backgroundImage.enabled = letter != default;

        // Inicializa o InputField
        if (letterInputField == null)
        {
            letterInputField = GetComponentInChildren<TMP_InputField>(); // Aqui pega o InputField
        }

        // Desativa a possibilidade de digitar se a célula estiver bloqueada
        letterInputField.interactable = !isLocked;

        crosswordManager = FindFirstObjectByType<CrosswordManager>();

        // Seta como inativo as células que não tem letra 
        if (letterInputField != null && letterInputField.enabled && expectedLetter == default)
        {
            letterInputField.gameObject.SetActive(false);
        }

        // Configura o InputField para que ele mostre o valor da letra, mas não seja editável caso esteja bloqueado
        if (letterInputField != null)
        {
            letterInputField.text = ""; // deixa vazio inicialmente 
            letterInputField.onValueChanged.AddListener(OnInputValueChanged); // Adiciona ouvinte para alterações
            letterInputField.onSelect.AddListener(OnInputSelected);
        }
    }

    // Esse método é chamado toda vez que o valor do InputField é alterado
    private void OnInputValueChanged(string newText)
    {
        // Verifica se a nova letra é válida
        if (newText.Length > 0)
        {
            letter = char.ToUpper(newText[0]); // pega a letra digitada pelo usuário, em maiúscula
            letterInputField.text = letter.ToString(); // garante que só tenha 1 letra visível

            // Opcional: pode validar automaticamente aqui
            if (letter == expectedLetter)
            {
                MarkCorrect();
            }
            else
            {
                MarkIncorrect();
            }
        }
        else
        {
            letter = default;
            backgroundImage.color = normalColor;
        }
    }

    public void OnInputSelected(string text)
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
        expectedLetter = char.ToUpper(value); // define a letra correta esperada

        letter = default;
        if (letterInputField != null)
        {
            letterInputField.text = ""; // limpa o input para o usuário digitar
            letterInputField.interactable = true; // garante que o usuário possa digitar
        }

        isLocked = false;

    }

    // setar o clue
      public void SetClue(string value)
    {
        clue = value; // define a letra correta esperada

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
        
        isLocked = false;
        isSelected = false;
        letter = default;

        if (letterInputField != null)
        {
            letterInputField.text = ""; // Limpa o campo de texto
            letterInputField.interactable = true;
            letterInputField.gameObject.SetActive(true); // Reativa o campo de input quando a célula é resetada

        }
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
            backgroundImage.enabled = expectedLetter != default;

        }
    }
}
