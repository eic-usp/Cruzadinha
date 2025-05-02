using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CrosswordCell : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI letterText; // aqui ele define, certo? nop, elle só inicializa aqui, mas sem nenhum valor ainda
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
        
        if (letterText == null)
            letterText = GetComponentInChildren<TextMeshProUGUI>(); //aq ele pega o texto

        Debug.Log("Letra pegada", letterText);

        crosswordManager = FindFirstObjectByType<CrosswordManager>();
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

    public void SetLetter(char letter)
    {
        if (letterText != null)
        {
            letterText.text = letter.ToString();
            isLocked = true;
        }
    }

    public void MarkCorrect()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = correctColor;
            isLocked = true;
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
        if (letterText != null)
        {
            letterText.text = "";
        }
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
} 