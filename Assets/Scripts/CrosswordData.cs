using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CrosswordData
{
    public int gridWidth;
    public int gridHeight;
    public List<CrosswordWord> words = new List<CrosswordWord>();

    public class CrosswordWord
    {
        public string word;
        public string hint;
        public bool isHorizontal;
        public int startX;
        public int startY;
    }

    public static CrosswordData LoadFromXML(TextAsset xmlFile)
    {
        try
        {
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

            // Process all word arrays
            XmlNodeList wordArrays = xmlDoc.SelectNodes("//string-array");
            Debug.Log($"Found {wordArrays.Count} word arrays");

            foreach (XmlNode array in wordArrays)
            {
                string arrayName = array.Attributes["name"].Value;
                bool isHorizontal = arrayName.Contains("horizontal");
                Debug.Log($"Processing array: {arrayName} (horizontal: {isHorizontal})");
                
                foreach (XmlNode item in array.ChildNodes)
                {
                    if (string.IsNullOrEmpty(item.InnerText)) continue;

                    string[] parts = item.InnerText.Split(';');
                    if (parts.Length >= 3)
                    {
                        CrosswordWord word = new CrosswordWord
                        {
                            startX = int.Parse(parts[0]),
                            word = parts[1],
                            hint = parts[2],
                            isHorizontal = isHorizontal
                        };

                        word.startY = 0;
                        data.words.Add(word);
                        Debug.Log($"Added word: {word.word} at position ({word.startX}, {word.startY})");
                    }
                }
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