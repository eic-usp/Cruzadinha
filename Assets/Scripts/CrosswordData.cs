using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CrosswordData
{
    public int gridWidth;
    public int gridHeight;
    public List<CrosswordWord> horizontalWords = new();
    public List<CrosswordWord> verticalWords = new();

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
                
                for (var i = 0; i < array.ChildNodes.Count; i++)
                {
                    var item = array.ChildNodes[i];
                    
                    if (string.IsNullOrEmpty(item.InnerText)) continue;

                    string[] parts = item.InnerText.Split(';');
                    if (parts.Length >= 3)
                    {
                        var startIndex = int.Parse(parts[0]);

                        // CrosswordWord wH = null, wV = null;
                        //
                        // if (isHorizontal)
                        // {
                        //     wH = new CrosswordWord
                        //     {
                        //         startX = startIndex,
                        //         startY = i,
                        //         word = parts[1],
                        //         hint = parts[2],
                        //         isHorizontal = isHorizontal
                        //     };
                        // }
                        // else
                        // {
                        //     wV = new CrosswordWord
                        //     {
                        //         startX = i,
                        //         startY = startIndex,
                        //         word = parts[1],
                        //         hint = parts[2],
                        //         isHorizontal = isHorizontal
                        //     };
                        // }

                        CrosswordWord word = new CrosswordWord
                        {
                            startX = isHorizontal ? startIndex : i,
                            startY = isHorizontal ? i : startIndex,
                            word = parts[1],
                            hint = parts[2],
                            isHorizontal = isHorizontal
                        };

                        if (isHorizontal)
                        {
                            data.horizontalWords.Add(word);
                        }
                        else
                        {
                            data.verticalWords.Add(word);
                        }
                        
                        Debug.Log($"Added word: {word.word} at position ({word.startX}, {word.startY})");
                    }
                }
            }

            Debug.Log($"Total words loaded: {data.horizontalWords.Count}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading XML: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
}