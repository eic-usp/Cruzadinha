using UnityEngine;

[CreateAssetMenu(menuName = "Crossword", fileName = "New crossword")]
public class Crossword : ScriptableObject
{
    [field: SerializeField] public string CrosswordName { get; private set; }
    [field: SerializeField] public TextAsset TextAsset { get; private set; }
}
