using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol Data", menuName = "Symbol Data")]
public class SymbolData : ScriptableObject
{
    [SerializeField] private Sprite symbolImage;
    [SerializeField] private SymbolType symbolType;
    [SerializeField] private float symbolCost;

    public Sprite SymbolImage => symbolImage;
    public SymbolType SymbolType => symbolType;
    public float SymbolCost => symbolCost;
}
