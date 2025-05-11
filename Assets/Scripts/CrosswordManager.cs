using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Linq;

public class CrosswordManager : MonoBehaviour
{
    public CrosswordCell cellPrefab;
    public Transform gridParent;
    public float cellSize = 1f;
    public float spacing = 0.1f;
    public Text hintText;
    public Text diseaseTitleText;
    public Button backButton;

    private CrosswordCell[,] grid;
    private CrosswordData crosswordData = new CrosswordData();
    private List<CrosswordData.CrosswordWord> placedWords = new();


    void Start()
    {
        // Get selected disease from PlayerPrefs
        int selectedDisease = PlayerPrefs.GetInt("SelectedDisease", 0);

        Debug.Log($"Selected disease: {selectedDisease}");
        Crossword disease = GameSceneManager.Instance.Crosswords[selectedDisease];
        TextAsset xmlFile = disease.TextAsset;

        if (xmlFile == null)
        {
            Debug.LogError($"Could not open xml file {xmlFile} for {selectedDisease}");
            return;
        }

        // Carregar dados do crossword do XML
        crosswordData = CrosswordData.LoadFromXML(xmlFile);
        if (crosswordData == null)
        {
            Debug.LogError("Failed to load crossword data from XML");
            return;
        }

        // Set disease title
        if (diseaseTitleText != null)
        {
            diseaseTitleText.text = disease.CrosswordName;
        }

        try
        {
            CreateGrid();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erro ao criar grid: " + ex.Message + "\n" + ex.StackTrace);
        }

        PlaceWords();

        // Setup back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    void OnBackClicked()
    {
        GameSceneManager.Instance.LoadCrosswordSelection();
    }


    void CreateGrid()
    {
        int gridWidth = crosswordData.gridWidth;
        int gridHeight = crosswordData.gridHeight;

        grid = new CrosswordCell[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 position = new Vector3(x * (cellSize + spacing), -y * (cellSize + spacing), 0);

                CrosswordCell cell = Instantiate(cellPrefab, gridParent);
                grid[x, y] = cell;

                RectTransform rect = cell.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(x * (cellSize + spacing), -y * (cellSize + spacing));
                }

                // Verifica se a célula faz parte de alguma palavra
                bool isPartOfWord = false;
                foreach (var word in crosswordData.words)
                {
                    // Se a célula faz parte da palavra
                    if (word.isHorizontal)
                    {
                        if (y == word.row && x >= word.col && x < word.col + word.word.Length)
                        {
                            isPartOfWord = true;
                            break;
                        }
                    }
                    else
                    {
                        if (x == word.col && y >= word.row && y < word.row + word.word.Length)
                        {
                            isPartOfWord = true;
                            break;
                        }
                    }
                }

                // Se a célula não faz parte de nenhuma palavra, torná-la invisível
                if (!isPartOfWord)
                {
                    Debug.Log("não faz parte");
                    var image = cell.GetComponentInChildren<Image>(); // Para a imagem (se houver)
                    var text = cell.GetComponentInChildren<Text>(); // Para o texto (se houver)
                    var inputField = cell.GetComponentInChildren<InputField>(); // Para o campo de input (se houver)

                    if (image != null)
                    {
                        Debug.Log("image pega");
                        image.color = new Color(1, 1, 1, 0); // Torna a imagem invisível
                    }

                    // Tornar o texto invisível
                    if (text != null)
                    {
                        text.enabled = false; // Torna o texto invisível
                    }

                    // Caso queira esconder o input, mas deixar ele funcional se necessário:
                    if (inputField != null)
                    {
                        inputField.enabled = false; // Opcional: esconder o campo de input
                    }
                }
            }
        }
    }

    void PlaceWords()
    {
        foreach (CrosswordData.CrosswordWord word in  crosswordData.words)
        {
            bool canPlace = true;

            // Verificar se a palavra pode ser colocada
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.col + (word.isHorizontal ? i : 0);
                int y = word.row + (word.isHorizontal ? 0 : i);

                if (x >= grid.GetLength(0) || y >= grid.GetLength(1))
                {
                    canPlace = false;
                    break;
                }

                // Se a célula já tiver uma letra que não corresponde, não pode colocar a palavra
                if (grid[x, y] != null && grid[x, y].letter != default && grid[x, y].letter != word.word[i])
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                placedWords.Add(word);
                // Coloca a palavra no grid
                for (int i = 0; i < word.word.Length; i++)
                {
                    int x = word.col + (word.isHorizontal ? i : 0);
                    int y = word.row + (word.isHorizontal ? 0 : i);

                    CrosswordCell cell = grid[x, y];
                    cell.SetLetter(word.word[i]);
                }
            }
            else
            {
                Debug.LogWarning($"Cannot place word '{word.word}' at ({word.row},{word.col})");
            }
        }
    }

    public void ShowHint(CrosswordCell cell)
    {
        foreach (CrosswordData.CrosswordWord word in placedWords) 
        {
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.col + (word.isHorizontal ? i : 0);
                int y = word.row + (word.isHorizontal ? 0 : i);

                if (x == cell.x && y == cell.y)
                {
                    if (hintText != null)
                    {
                        hintText.text = word.clue; 
                    }
                    return;
                }
            }
        }
    }

    public void CheckWord(CrosswordData.CrosswordWord word)
    {
        bool isCorrect = true;
        for (int i = 0; i < word.word.Length; i++)
        {
            int x = word.col + (word.isHorizontal ? i : 0);
            int y = word.row + (word.isHorizontal ? 0 : i);

            if (x < crosswordData.gridWidth && y < crosswordData.gridHeight)
            {
                CrosswordCell cell = grid[x, y];
                if (cell == null || cell.letterInputField.text != word.word[i].ToString())
                {
                    isCorrect = false;
                    break;
                }
            }
        }

        if (isCorrect)
        {
            Debug.Log($"Word '{word.word}' is correct!");
            // Marcar células como corretas
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.col + (word.isHorizontal ? i : 0);
                int y = word.row + (word.isHorizontal ? 0 : i);
                grid[x, y].MarkCorrect();
            }
        }
    }
}

