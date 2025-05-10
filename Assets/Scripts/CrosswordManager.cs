using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    private CrosswordData crosswordData;
    private List<CrosswordData.CrosswordWord> placedWords = new();
    
    void Start()
    {
        // Get selected disease from PlayerPrefs
        int selectedDisease = PlayerPrefs.GetInt("SelectedDisease", 0);

        // Load the XML file from Resources
        // TextAsset xmlFile = Resources.Load<TextAsset>($"CrosswordData/{selectedDisease.ToLower()}_data");

        Debug.Log($"Selected disease: {selectedDisease}");
        Crossword disease = GameSceneManager.Instance.Crosswords[selectedDisease];
        TextAsset xmlFile = disease.TextAsset;
        
        if (xmlFile == null)
        {
            Debug.LogError($"Could not open xml file {xmlFile} for {selectedDisease}");
            return;
        }


        // Set disease title
        if (diseaseTitleText != null)
        {
            diseaseTitleText.text = disease.CrosswordName;
        }

        // Load and setup crossword
        crosswordData = CrosswordData.LoadFromXML(xmlFile);
        if (crosswordData == null)
        {
            Debug.LogError("Failed to load crossword data from XML");
            return;
        }
        
        try
        {
            CreateGrid();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erro ao criar grid: " + ex.Message + "\n" + ex.StackTrace);
        }

        //CreateGrid();
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
        grid = new CrosswordCell[crosswordData.gridWidth, crosswordData.gridHeight];
        for (int y = 0; y < crosswordData.gridHeight; y++)
        {
            for (int x = 0; x < crosswordData.gridWidth; x++)
            {
                Vector3 position = new Vector3(
                    x * (cellSize + spacing),
                    y * (cellSize + spacing),
                    0
                );

                CrosswordCell cell = Instantiate(cellPrefab, gridParent);
                grid[x, y] = cell;
                
                RectTransform rect = cell.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(
                        x * (cellSize + spacing),
                        -y * (cellSize + spacing) // y negativo para alinhar de cima pra baixo
                    );
                }
            }
        }
    }


    void PlaceWords()
    {
        var words = new List<CrosswordData.CrosswordWord>();

        var hCount = 5;
        var hRangeStart = Random.Range(0, crosswordData.horizontalWords.Count - hCount);
        var hRandomRange = crosswordData.horizontalWords.GetRange(hRangeStart, hCount);
        
        var vCount = 5;
        var vRangeStart = Random.Range(0, crosswordData.verticalWords.Count - vCount);
        var vRandomRange = crosswordData.verticalWords.GetRange(vRangeStart, vCount);
        
        words.AddRange(hRandomRange);
        words.AddRange(vRandomRange);
        
        words.Shuffle();
        
        foreach (CrosswordData.CrosswordWord word in words)
        {
            Debug.Log($"WORD: {word.word}");
            
            bool canPlace = true;
            
            // Check if word can be placed
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.startX + (word.isHorizontal ? i : 0);
                int y = word.startY + (word.isHorizontal ? 0 : i);

                if (x >= crosswordData.gridWidth || y >= crosswordData.gridHeight)
                {
                    canPlace = false;
                    break;
                }
                
                // Debug.Log($"Is horizontal: {word.isHorizontal}");
                // Debug.Log($"Grid[{x},{y}]: {(grid[x,y] == null ? "null" : grid[x,y].letter)}");
                // Debug.Log($"Word[i] = Word[{i}]: {word.word[i]}");
                // Debug.Log($"X: {x}, Y: {y}. I: {i}");
                
                if (grid[x,y] != null && grid[x,y].letter != default && grid[x,y].letter != word.word[i])
                {
                    Debug.Log($"Grid[{x},{y}] é {grid[x,y].letter}. Word[{i}] é {word.word[i]}");
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                placedWords.Add(word);
                for (int i = 0; i < word.word.Length; i++)
                {
                    int x = word.startX + (word.isHorizontal ? i : 0);
                    int y = word.startY + (word.isHorizontal ? 0 : i);

                    CrosswordCell cell = grid[x, y];
                    
                    Debug.Log($"Placing {word.word[i]} at ({x},{y}) [{word.word}]");
                    
                    if (cell != null)
                    {
                        cell.SetLetter(word.word[i]);
                    }
                }
            }
        }
    }

    public void ShowHint(CrosswordCell cell)
    {
        foreach (CrosswordData.CrosswordWord word in placedWords)
        {
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.startX + (word.isHorizontal ? i : 0);
                int y = word.startY + (word.isHorizontal ? 0 : i);

                if (x == cell.x && y == cell.y)
                {
                    if (hintText != null)
                    {
                        hintText.text = word.hint;
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
            int x = word.startX + (word.isHorizontal ? i : 0);
            int y = word.startY + (word.isHorizontal ? 0 : i);

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
            // Mark cells as correct
            for (int i = 0; i < word.word.Length; i++)
            {
                int x = word.startX + (word.isHorizontal ? i : 0);
                int y = word.startY + (word.isHorizontal ? 0 : i);
                grid[x, y].MarkCorrect();
            }
        }
    }
} 