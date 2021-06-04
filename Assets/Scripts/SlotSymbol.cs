using UnityEngine;
using UnityEngine.UI;

public class SlotSymbol : MonoBehaviour
{
    [SerializeField] private SymbolData symbolSO;
    [SerializeField] private GameObject particleFrame;
    [SerializeField] private new ParticleSystem particleSystem;
    private RectTransform defaultParentReel;

    public SymbolData SymbolSO { get => symbolSO; set => symbolSO = value; }
    public RectTransform DefaultParentReel { get => defaultParentReel; set => defaultParentReel = value; }
    public GameObject ParticleFrame { get => particleFrame; set => particleFrame = value; }
    public ParticleSystem ParticleSystem { get => particleSystem; set => particleSystem = value; }
}
