using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolsManagement : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private GameManagement gameManager;
    [SerializeField] private RectTransform mainCanvasRT;
    [SerializeField] private RectTransform[] symbols;
    [SerializeField] private List<SymbolData> symbolsData;
    [SerializeField] List<FinalScreenSO> finalScreens;

    //для проверки на каком риле находится символ
    [SerializeField] private List<RectTransform> reelAnchors;

    //для чтения финальных символов
    private int firstReelSymbol;
    private int secondReelSymbol;
    private int thirdReelSymbol;
    private int currentSet;

    private RectTransform symbol;
    private Sprite symbolSprite;
    private float symbolHeigth;
    public bool isMutable;
    private float mainCanvasScale;
    #endregion

    void Start()
    {
        isMutable = true;
        currentSet = -1;
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

    public void ResetSymbols()
    {
        firstReelSymbol = -3;
        secondReelSymbol = -2;
        thirdReelSymbol = -1;
    }
    public void NextSet()
    {
        _ = currentSet >= finalScreens.Count - 1 ? currentSet = 0 : currentSet += 1;
    }

    private void CheckSymbolPosition(RectTransform symbol)
    {
        if (symbol.position.y <= 240 * mainCanvasScale)
        {
            ChangeSymbolPosition(symbol);
        }
    }
    private void ChangeSymbolPosition(RectTransform symbol)
    {
        symbolHeigth = symbol.rect.height;
        var offset = symbol.position.y + symbolHeigth * mainCanvasScale * 4;
        var newPos = new Vector3(symbol.position.x, offset, symbol.position.z);
        symbol.position = newPos;
        if (isMutable) ChangeSprite(symbol);
    }

    private void ChangeSprite(RectTransform symbol)
    {
        var newSymbol = symbol.GetComponentInParent<ReelInfo>().isStopping ?
            GetFinalScreenSymbol(symbol) : GetRandomSymbol();
        symbolSprite = newSymbol.SymbolImage;
        symbol.GetComponent<Image>().sprite = symbolSprite;
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

    private int GetFinalSymbolIndex(RectTransform symbol)
    {
        switch (CheckSymbolReel(symbol))
        {
            case 1:
                return firstReelSymbol += 3;
            case 2:
                return secondReelSymbol += 3;
            case 3:
                return thirdReelSymbol += 3;
            default:
                return 0;
        }
    }
    private int CheckSymbolReel(RectTransform symbol)
    {
        var currentReel =   symbol.IsChildOf(reelAnchors[0]) ? 1 :
                            symbol.IsChildOf(reelAnchors[1]) ? 2 :
                            symbol.IsChildOf(reelAnchors[2]) ? 3 : 0;
        return currentReel;
    }
    private SymbolData GetFinalScreenSymbol(RectTransform symbol)
    {
        var index = GetFinalSymbolIndex(symbol);
        //if (index > 11)
        //{
        //    index = 0;
        //}
        var newSymbol = finalScreens[currentSet].FinalScreenSymbols[index];
        return newSymbol != null ? newSymbol : GetRandomSymbol();
    }
    private SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, 11);
        return symbolsData[random];
    }
}