using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [SerializeField] private SymbolData symbolSO;
    [SerializeField] private GameObject particleFrame;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform symbolRT;
    [SerializeField] private UIShiny symbolShiny;
    private bool isHidden = false;

    private readonly Color transparent = new Color(1, 1, 1, 0);
    private readonly Color opaque = new Color(1, 1, 1, 1);
    

    public GameObject ParticleFrame => particleFrame;
    public ParticleSystem ParticleSystem => particleSystem;
    public RectTransform SymbolRT => symbolRT;
    public SymbolData SymbolSO { get => symbolSO; set => symbolSO = value; }        
    public bool IsHidden { get => isHidden; set => isHidden = value; }
    public Image Icon { get => icon; set => icon = value; }
    public UIShiny SymbolShiny => symbolShiny;

    public void SetSymbolTransparency(bool isTransparent)
    {
        if (isTransparent == true) icon.color = transparent;
        else icon.color = opaque;
    }
}
