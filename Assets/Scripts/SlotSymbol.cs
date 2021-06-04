using UnityEngine;

public class SlotSymbol : MonoBehaviour
{
    [SerializeField] private SymbolData symbolSO;
    private RectTransform defaultParentReel;

    public SymbolData SymbolSO { get => symbolSO; set => symbolSO = value; }
    public RectTransform DefaultParentReel { get => defaultParentReel; set => defaultParentReel = value; }
}
