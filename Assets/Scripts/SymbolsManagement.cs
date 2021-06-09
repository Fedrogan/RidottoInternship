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

    private int firstReelSymbol;
    private int secondReelSymbol;
    private int thirdReelSymbol;
    private int currentSet;    

    private SymbolData symbolData;
    private RectTransform symbol;
    private Sprite symbolSprite;
    private float symbolHeigth;
    public bool isMutable;
    private float mainCanvasScale;

    public bool IsRandomGame { get => isRandomGame; }

    void Start()
    {
        GameController.Instance.SpinStarted += ResetSymbols;
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
        if (!IsRandomGame) NextSet();
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
        if (isMutable && symbol.GetComponent<SlotSymbol>() != null) ChangeSpriteAndSetSymbolData(symbol);
    }

    private void ChangeSpriteAndSetSymbolData(RectTransform symbol)
    {
        var newSymbol = symbol.GetComponentInParent<ReelInfo>().IsStopping && !isRandomGame ?
            GetFinalScreenSymbol(symbol) : GetRandomSymbol();
        symbolData = newSymbol;
        symbol.GetComponent<SlotSymbol>().SymbolSO = symbolData;       
        symbolSprite = symbolData.SymbolImage;
        symbol.GetComponent<Image>().sprite = symbolSprite;
    }

    public void ResetSymbolsPosition(float correctedSlowDownDistance, float cellYCorrection, float startReelPositionY, RectTransform reel)
    {
        var symbolsCount = reel.childCount;
        print("Symbols On Reel Count = " + symbolsCount);
        for (int i = 0; i < symbolsCount; i++)
        {
            var symbol = reel.GetChild(i);
            print(symbol);
            print("Symbol pos before = " + symbol.localPosition.y);
            var symbolPos = symbol.localPosition;
            print(symbolPos);
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
        index = index > finalScreens.Count - 1 ? 0 : index; 
        var newSymbol = finalScreens[currentSet].FinalScreenSymbols[index];
        
        return newSymbol != null ? newSymbol : GetRandomSymbol();
    }
    private SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, symbolsData.Count - 1);
        return symbolsData[random];
    }

    public SlotSymbol GetSymbolOnReelById(RectTransform reel, int id)
    {
        var childCount = reel.childCount;
        var symbolsOnReel = new List<SlotSymbol>();

        for (int i = 0; i < childCount; i++)
        {
            var symbol = reel.GetChild(i).GetComponent<SlotSymbol>();
            if (symbol != null && symbol.GetComponent<RectTransform>().localPosition.y != 0)
                symbolsOnReel.Add(symbol);
        }
        var sortedSymbols = symbolsOnReel.OrderBy(x => x.GetComponent<RectTransform>().localPosition.y);
        var sortedSymbolsArray = sortedSymbols.ToArray();
        return sortedSymbolsArray[id];
    }

    public SymbolData[] GetSymbolsData(SlotSymbol[] symbolsInLine)
    {
        var symbolsData = new SymbolData[symbolsInLine.Length];
        //var i = 0;
        //foreach (var symbol in symbolsInLine)
        //{
        //    symbolsData[i] = symbol.SymbolSO;
        //    i++;
        //}
        for (int i = 0; i < symbolsInLine.Length - 1; i++)
        {
            symbolsData[i] = symbolsInLine[i].SymbolSO;
        }
        return symbolsData;
    }

    public SlotSymbol[] GetAllSymbols()
    {
        var allSymbols = new SlotSymbol[12];
        int i = 0;
        for (int j = 0; j < reelAnchors.Count; j++)
        {
            var childCount = reelAnchors[j].childCount;
            for (int k = 0; k < childCount; k++)
            {
                var symbol = reelAnchors[j].GetChild(k).GetComponent<SlotSymbol>();
                print(symbol);
                if (symbol != null)
                {
                    allSymbols[i] = symbol;
                    i++;
                }                
            }
        }
        return allSymbols;
    }
}