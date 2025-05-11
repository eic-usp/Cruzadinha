using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CrosswordData
{
    public int gridWidth;
    public int gridHeight;
    public List<CrosswordWord> words = new();

    public class CrosswordWord
    {
        public int row;
        public int col;
        public string word;
        public string clue;
        public bool isHorizontal;
    }

    public static CrosswordData LoadFromXML(TextAsset xmlFile)
    {
        try
        {
            // Criar a instância de CrosswordData para armazenar os dados
            CrosswordData data = new CrosswordData();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);

            Debug.Log("XML document loaded successfully");

            // Get grid dimensions
            XmlNode widthNode = xmlDoc.SelectSingleNode("//integer[@name='grid_width']");
            XmlNode heightNode = xmlDoc.SelectSingleNode("//integer[@name='grid_height']");

            if (widthNode == null || heightNode == null)
            {
                Debug.LogError("Could not find grid dimensions in XML");
                return null;
            }

            data.gridWidth = int.Parse(widthNode.InnerText);
            data.gridHeight = int.Parse(heightNode.InnerText);
            Debug.Log($"Grid dimensions: {data.gridWidth}x{data.gridHeight}");

            // Processar todas as palavras do arquivo XML
            XmlNodeList wordArrays = xmlDoc.SelectNodes("//item");
            Debug.Log($"Found {wordArrays.Count} word arrays");

            // Carregar as palavras horizontais e verticais
            foreach (XmlNode wordNode in wordArrays)
            {
                int row = int.Parse(wordNode.Attributes["row"].Value);
                int col = int.Parse(wordNode.Attributes["col"].Value);
                string word = wordNode["word"].InnerText;
                string clue = wordNode["clue"].InnerText;
                string dir = wordNode.Attributes["dir"].Value.ToLower();  // Acessar o atributo "dir"
                
                CrosswordWord crosswordWord = new CrosswordWord
                {
                    row = row,
                    col = col,
                    word = word,
                    clue = clue,
                    isHorizontal = dir == "h",
                };

                Debug.Log("Adicionado a palavra: " + crosswordWord.word + " que é " + crosswordWord.isHorizontal);

                // Adicionar a palavra à lista
                data.words.Add(crosswordWord);
            }

            Debug.Log($"Total words loaded: {data.words.Count}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading XML: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
}
