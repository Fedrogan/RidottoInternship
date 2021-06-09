using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SymbolsManagement : MonoBehaviour
{    
    [SerializeField] private RectTransform mainCanvasRT;
    [SerializeField] private RectTransform[] symbols;
    [SerializeField] private List<SymbolData> symbolsData;
    [SerializeField] private List<FinalScreenSO> finalScreens;
    [SerializeField] private bool isRandomGame;

    [SerializeField] private List<RectTransform> reelAnchors;

    private readonly int reelSymbolsCount = 4;
    private readonly int defaultExitYPos = 240;

    private RectTransform symbol;
    private float symbolHeigth;
    public bool isMutable;
    private float mainCanvasScale;

    //[SerializeField] public bool reelStopping;

    void Start()
    {
        isMutable = true;
        mainCanvasScale = mainCanvasRT.lossyScale.y;
    }

    void Update()
    {
        for (int i = 0; i < symbols.Length; i++)
        {
            symbol = symbols[i];            
            CheckSymbolPosition(symbol);
        }        
    }        

    private void CheckSymbolPosition(RectTransform symbol)
    {
        if (symbol.position.y <= defaultExitYPos * mainCanvasScale)
        {
            ChangeSymbolPosition(symbol);
        }
    }

    private void ChangeSymbolPosition(RectTransform symbol)
    {
        symbolHeigth = symbol.rect.height;
        var offset = symbol.position.y + symbolHeigth * mainCanvasScale * reelSymbolsCount;
        var newPos = new Vector3(symbol.position.x, offset, symbol.position.z);
        symbol.position = newPos;

        if (symbol.GetComponent<SlotSymbol>().IsHidden == true) 
            symbol.GetComponent<Image>().CrossFadeAlpha(0, 0, true);

        else if (symbol.GetComponent<SlotSymbol>().IsHidden == false) 
            symbol.GetComponent<Image>().CrossFadeAlpha(1, 0, true);

        if (isMutable) 
            ChangeSprite(symbol);
    }

    private void ChangeSprite(RectTransform symbol)
    {
        var newSymbol = GetRandomSymbol().SymbolImage;
        symbol.GetComponent<Image>().sprite = newSymbol;
    }

    public void ResetSymbolsPosition(float correctedSlowDownDistance, float cellYCorrection, float startReelPositionY, RectTransform reel)
    {
        var symbolsCount = reel.childCount;
        for (int i = 0; i < symbolsCount; i++)
        {
            var symbol = reel.GetChild(i);
            var symbolPos = symbol.localPosition;
            var newYPos = Mathf.Round(symbolPos.y + correctedSlowDownDistance - cellYCorrection - startReelPositionY);
            symbol.localPosition = new Vector3(symbolPos.x, newYPos, symbolPos.z);
        }
    }

    public void MakeAllSymbolsMutable(bool isMutable)
    {
        this.isMutable = isMutable;
    }    
        
    private SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, symbolsData.Count);
        print(random);
        return symbolsData[random];
    } 
    
    public void SetSymbolsOnReelAlpha (RectTransform reel, bool isHidden)
    {
        int childCount = reel.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (reel.GetChild(i).GetComponent<SlotSymbol>() != null)
            reel.GetChild(i).GetComponent<SlotSymbol>().IsHidden = isHidden;
        }
    }
}