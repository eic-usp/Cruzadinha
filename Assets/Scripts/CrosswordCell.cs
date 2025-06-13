using TMPro; // Para usar o TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CrosswordCell : MonoBehaviour
{
    public char letter;
    public char expectedLetter;
    public string clue;

    public TMP_InputField letterInputField; // Usando InputField do TextMeshPro
    public Image backgroundImage;
    public bool isSelected = false;
    public bool isHorizontal = false;
    public int x, y;
    public bool isLocked = false;

    private Color normalColor = Color.white;
    private Color selectedColor = new Color(0.8f, 0.8f, 1f);
    private Color correctColor = new Color(0f, 1f, 0.1f);
    private Color incorrectColor = new Color(1f, 0.6f, 0.6f);

    public CrosswordManager crosswordManager;
    public List<CrosswordData.CrosswordWord> palavrasAssociadas = new List<CrosswordData.CrosswordWord>(); // Lista de palavras associadas

    void Start()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }

        backgroundImage.enabled = letter != default;

        // Inicializa o InputField
        if (letterInputField != null)
        {
            letterInputField.onValueChanged.AddListener(OnInputValueChanged);
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

    // Função que retorna a próxima célula com base nas coordenadas fornecidas (x, y)
    public CrosswordCell GetNextCell(int x, int y)
    {
        // Verifique se a coordenada (x, y) está dentro dos limites da grade
        if (x >= 0 && x < crosswordManager.grid.GetLength(0) && y >= 0 && y < crosswordManager.grid.GetLength(1))
        {
            return crosswordManager.grid[x, y];  // Retorna a célula na posição (x, y)
        }
        return null;  // Se a coordenada não for válida, retorna null
    }

    // Esse método é chamado toda vez que o valor do InputField é alterado
    private void OnInputValueChanged(string newText)
    {
        if (string.IsNullOrEmpty(newText))
        {
            letter = default;
            backgroundImage.color = normalColor;
            return;
        }

        letter = char.ToUpper(newText[0]);
        letterInputField.text = letter.ToString();
        Debug.Log($"Input alterado na célula ({x}, {y}): Letra definida como '{letter}', Letra esperada é '{expectedLetter}'");

        if (letter == expectedLetter)
        {
            MarkCorrect();
            MoveToNextCell();
        }
        else
        {
            MarkIncorrect();
        }

        // Volta a verificar as palavras associadas
        if (crosswordManager != null && palavrasAssociadas.Count > 0)
        {
            foreach (var palavra in palavrasAssociadas)
            {
                crosswordManager.CheckWord(palavra);  // Verifica todas as palavras associadas à célula
            }
        }
    }

    private void MoveToNextCell()
    {
        if (palavrasAssociadas.Count == 0) return;

        // A primeira palavra associada (pode ser melhorada para trabalhar com múltiplas)
        CrosswordData.CrosswordWord palavraAssociada = palavrasAssociadas[0];

        int nextX = x;
        int nextY = y;

        // Determina a próxima posição baseada na direção da palavra
        if (palavraAssociada.isHorizontal)
        {
            nextX = x + 1;
            nextY = y;
        }
        else
        {
            nextX = x;
            nextY = y + 1;
        }

        if (palavraAssociada.isHorizontal && nextX >= palavraAssociada.col + palavraAssociada.word.Length)
        {
            return;
        }
        else if (!palavraAssociada.isHorizontal && nextY >= palavraAssociada.row + palavraAssociada.word.Length)
        {
            return;
        }

        // Obtém a próxima célula
        CrosswordCell nextCell = GetNextCell(nextX, nextY);

        // Verifica se a próxima célula é válida e pode receber input
        while (nextCell != null && (nextCell.letterInputField == null || nextCell.isLocked || !nextCell.letterInputField.gameObject.activeSelf))
        {
            // Move para a próxima célula se a atual não for válida
            if (palavraAssociada.isHorizontal)
            {
                nextX++;
            }
            else
            {
                nextY++;
            }

            nextCell = GetNextCell(nextX, nextY);
        }

        if (nextCell != null)
        {
            // Desativa o input field atual
            letterInputField.DeactivateInputField();

            // Atualiza o foco e seleciona o próximo InputField
            nextCell.letterInputField.ActivateInputField();
            EventSystem.current.SetSelectedGameObject(nextCell.letterInputField.gameObject);

            // Garantir que o cursor esteja na posição inicial
            nextCell.letterInputField.caretPosition = 0;
            nextCell.letterInputField.selectionAnchorPosition = 0;
            nextCell.letterInputField.selectionFocusPosition = 0;
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
        expectedLetter = char.ToUpper(value); // Define a letra correta esperada
        Debug.Log($"Definindo letra esperada '{expectedLetter}' para célula na posição ({x}, {y})");

        letter = default;
        if (letterInputField != null)
        {
            letterInputField.text = ""; // Limpa o input para o usuário digitar
            letterInputField.interactable = true;
            letterInputField.textComponent.alignment = TextAlignmentOptions.Center;
        }

        isLocked = false;
    }

    // Seta o clue
    public void SetClue(string value)
    {
        clue = value; // Define a dica
    }

    public void MarkCorrect()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = correctColor;
            isLocked = true;
            letterInputField.interactable = false; // Desativa a interação quando a letra estiver correta
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
