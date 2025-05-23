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
    public GameTimer gameTimer;
    public MensagemController mensagemController;


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
    void OnTimeExpired()
    {
        Debug.Log("Tempo acabou! Faça o que quiser aqui: finalizar, mostrar painel, etc.");
        // Por exemplo: mostrar tela de Game Over, desabilitar inputs, etc.
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
        // Obtem o GridLayoutGroup
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridWidth;

        // Obtem o RectTransform do gridParent
        RectTransform parentRect = gridParent.GetComponent<RectTransform>();

        // Obtem padding, cellSize e spacing do GridLayoutGroup
        RectOffset padding = gridLayout.padding;

        float cellWidth = gridLayout.cellSize.x;
        float cellHeight = gridLayout.cellSize.y;
        float spacingX = gridLayout.spacing.x;
        float spacingY = gridLayout.spacing.y;

        // Calcula o tamanho total necessário
        float totalWidth = padding.left + padding.right + (cellWidth * gridWidth) + (spacingX * (gridWidth - 1));
        float totalHeight = padding.top + padding.bottom + (cellHeight * gridHeight) + (spacingY * (gridHeight - 1));

        parentRect.sizeDelta = new Vector2(totalWidth, totalHeight);



        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 position = new Vector3(x * (cellSize + spacing), -y * (cellSize + spacing), 0);

                CrosswordCell cell = Instantiate(cellPrefab, gridParent);
                grid[x, y] = cell;

                //RectTransform rect = cell.GetComponent<RectTransform>();
                //if (rect != null)
                //{
                //   rect.anchoredPosition = new Vector2(x * (cellSize + spacing), -y * (cellSize + spacing));
                //}

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

                // Se a célula não faz parte de nenhuma palavra, altere a cor do fundo
                if (!isPartOfWord)
                {
                    var image = cell.GetComponentInChildren<Image>(); // Para a imagem (se houver)
                    var text = cell.GetComponentInChildren<Text>(); // Para o texto (se houver)
                    var letterInputField = cell.GetComponentInChildren<InputField>(); // Para o campo de input (se houver)

                    if (image != null)
                    {
                        // Alterar a cor da imagem dentro do InputField (definir cor de fundo como #12806A)
                        image.color = new Color(18f / 255f, 128f / 255f, 106f / 255f); // Hex: #12806A
                    }

                    // Tornar o texto invisível
                    if (text != null)
                    {
                        text.enabled = false; // Torna o texto invisível
                    }

                    // Se o campo de input for necessário, ainda pode ser usado mas invisível
                    if (letterInputField != null)
                    {
                        // Acessar a Image dentro do letterInputField e alterar
                        var inputImage = letterInputField.GetComponent<Image>();
                        Debug.Log("image componente" + inputImage);
                        if (inputImage != null)
                        {
                            // Alterar a cor do fundo do InputField, se desejado
                            inputImage.color = new Color(18f / 255f, 128f / 255f, 106f / 255f); // Hex: #12806A
                        }

                        letterInputField.enabled = false; // Torna o campo de input invisível, mas sem desativá-lo completamente
                    }
                }
                else
                {
                    // Se a célula faz parte de uma palavra, você pode reverter as mudanças de visibilidade
                    var image = cell.GetComponentInChildren<Image>();
                    if (image != null)
                    {
                        image.color = Color.white; // Reseta a cor da célula para a cor original (ou qualquer outra cor desejada)
                    }

                    var text = cell.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.enabled = true; // Torna o texto visível novamente
                    }

                    var letterInputField = cell.GetComponentInChildren<InputField>();
                    if (letterInputField != null)
                    {
                        letterInputField.enabled = true; // Torna o campo de input visível e funcional novamente
                    }
                }
            }
        }
    }



    void PlaceWords()
    {
        foreach (CrosswordData.CrosswordWord word in crosswordData.words)
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
                    cell.SetClue(word.clue);
                   // Debug.Log($"colocando: '{x}, {y}', palavra '{word.word[i]} de {word.word}'");
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
        hintText.text = cell.clue;

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
                if (cell == null || cell.letter != cell.expectedLetter)
                {
                    isCorrect = false;
                    break;
                }
            }
        }

        if (isCorrect)
        {
            Debug.Log($"Word '{word.word}' is correct!");
            Debug.Log("Teste");
            
            // Marcar células como corretas
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.col + (word.isHorizontal ? i : 0);
                int y = word.row + (word.isHorizontal ? 0 : i);
                grid[x, y].MarkCorrect();
            }
            Debug.Log("Chamando MostrarMensagemTemporaria para a palavra: " + word.word);

            // Mostrar a mensagem por 5 segundos
            if (mensagemController == null)
            {
                Debug.LogWarning("mensagemController está null no CheckWord!");
            }
            else
            {
                mensagemController.MostrarMensagemTemporaria($"Palavra '{word.word}' correta!");
            }

        }
    }
}

