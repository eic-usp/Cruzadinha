using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.SceneManagement;  


public class CrosswordManager : MonoBehaviour
{
    public CrosswordCell cellPrefab;
    public Transform gridParent;
    public float cellSize = 1f;
    public float spacing = 0.1f;
    public Text hintText;
    public Text diseaseTitleText;
    public Button backButton;

    public CrosswordCell[,] grid;
    private CrosswordData crosswordData = new CrosswordData();
    private List<CrosswordData.CrosswordWord> placedWords = new();

    public GameTimer GameTimer;
    private int selectedTheme;
    private int selectedLevel;
    public MensagemController mensagemController;
    public GameObject gameOverPanel;
    public Button PularmodalButton;  // O botão de finalizar no modal
    //private bool gameOver = false;  // Controla se o jogo acabou
    public bool isCompleted = false;

    private int totalWords = 0;
    private int correctWordsCount = 0;




    void Start()
    {
        //level 
        selectedTheme = PlayerPrefs.GetInt("SelectedTheme", 0);  // ou "SelectedCrosswordIndex" conforme usa no menu
        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        Debug.Log($"Tema selecionado: {selectedTheme}, Nível selecionado: {selectedLevel}");
        //inicia o jogo 
        if (selectedLevel == 2)
        {
            GameTimer.StartTimer(120f);
        }
        else
        {
            // Nível 1: modo normal, sem tempo
            GameTimer.StopTimer();
        }

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

        //contador de palavras para a vitoria 
        totalWords = crosswordData.words.Count;
        Debug.Log($"Total de palavras no jogo: {totalWords}");
        correctWordsCount = 0; // Reset the counter to ensure it starts at 0

        if (PularmodalButton != null)
        {
            PularmodalButton.onClick.AddListener(OnFinishButtonClicked);
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
  

    // Método chamado quando o botão de finalizar é clicado
    public void OnFinishButtonClicked()
    {
        // Fechar o modal (gameOverPanel)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Desativa o painel do modal
        }

        // Carregar a cena de seleção
        SceneManager.LoadScene("CrosswordSelection");  
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
                    cell.x = x;
                    cell.y = y;
                    cell.SetLetter(word.word[i]);
                    cell.SetClue(word.clue);
                    cell.palavraAssociada = word;
                    cell.crosswordManager = this;

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
    public CrosswordCell GetNextCell(int x, int y)
    {
        // Verifica se a célula à direita ou abaixo está dentro dos limites
        if (x < crosswordData.gridWidth - 1)
        {
            return grid[x + 1, y]; // Próxima célula na horizontal
        }
        else if (y < crosswordData.gridHeight - 1)
        {
            return grid[x, y + 1]; // Próxima célula na vertical
        }

        return null; // Se não houver mais células
    }
    



    public void CheckWord(CrosswordData.CrosswordWord word)
    {
        Debug.Log($"checando palavra {word.word}");
        if (word.isCompleted)
        {
            Debug.Log($"Palavra '{word.word}' já está completa, ignorando.");
            return;
        }
        
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
                    Debug.Log($"ERRADA! letra: {cell.letter}, esperada: {cell.expectedLetter}");
                    isCorrect = false;
                    break;
                }
            }
        }

        if (isCorrect)
        {
            word.isCompleted = true;
            correctWordsCount++;
            Debug.Log($"Palavra '{word.word}' correta! Progresso: {correctWordsCount}/{totalWords}");
            
            if (mensagemController != null)
            {
                mensagemController.MostrarMensagemTemporaria($"Palavra '{word.word}' correta!");
            }
            
            // Verifica se todas as palavras foram acertadas
            if (correctWordsCount >= totalWords)
            {
                Debug.Log($"Vitória alcançada! Total de palavras: {totalWords}, Palavras corretas: {correctWordsCount}");
                isCompleted = true;
                gameOverPanel.SetActive(true);  // Mostra o painel de vitória
            }
        }
        else
        {
            Debug.Log($"Palavra '{word.word}' ainda não está correta.");
        }
    }
    

}

