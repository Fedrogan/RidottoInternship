using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReelsSymbolManager : MonoBehaviour
{
    [SerializeField] private RectTransform mainCanvasRT;
    [SerializeField] private SlotSymbol[] firstReelSymbols;
    [SerializeField] private SlotSymbol[] secondReelSymbols;
    [SerializeField] private SlotSymbol[] thirdReelSymbols;


    [SerializeField] private SymbolData[] symbolsData;
    [SerializeField] private FinalScreenSO[] finalScreens;
    [SerializeField] private bool isRandomGame;

    [SerializeField] private RectTransform[] substitutionReels;

    private int currentSet;

    public bool isMutable;

    [SerializeField] public bool reelStopping;

    public bool IsRandomGame { get => isRandomGame; }

    public static ReelsSymbolManager Instance { get; private set; }

    void Start()
    {
        Instance = this;
        currentSet = 0;
        FillReels();        
    }   


    public void FillReels()
    {
        for (int i = 0; i < 4; i++)
        {
            var symbol = firstReelSymbols[i];
            if (i == 3) symbol.SymbolSO = GetRandomSymbol();
            else symbol.SymbolSO = finalScreens[currentSet].FinalScreenSymbols[i] == null ? 
                    GetRandomSymbol() : finalScreens[currentSet].FinalScreenSymbols[i];
            symbol.GetComponent<Image>().sprite = symbol.SymbolSO.SymbolImage; 
        }
        for (int i = 0; i < 4; i++)
        {
            var symbol = secondReelSymbols[i];
            if (i == 3) symbol.SymbolSO = GetRandomSymbol();
            else symbol.SymbolSO = finalScreens[currentSet].FinalScreenSymbols[i + 3] == null ? 
                    GetRandomSymbol() : finalScreens[currentSet].FinalScreenSymbols[i + 3];
            symbol.GetComponent<Image>().sprite = symbol.SymbolSO.SymbolImage;            
        }
        for (int i = 0; i < 4; i++)
        {
            var symbol = thirdReelSymbols[i];
            if (i == 3) symbol.SymbolSO = GetRandomSymbol();
            else symbol.SymbolSO = finalScreens[currentSet].FinalScreenSymbols[i + 6] == null ? 
                    GetRandomSymbol() : finalScreens[currentSet].FinalScreenSymbols[i + 6];
            symbol.GetComponent<Image>().sprite = symbol.SymbolSO.SymbolImage;
        }
        NextSet();
    }


    public void NextSet()
    {
        _ = currentSet >= finalScreens.Length - 1 ? currentSet = 0 : currentSet += 1;
    } 
       
    private SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, symbolsData.Length - 1);
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
        var i = 0;
        foreach (var symbol in symbolsInLine)
        {
            symbolsData[i] = symbol.SymbolSO;
            i++;
        }
        return symbolsData;
    }

    public SlotSymbol[] GetAllVisibleSymbols()
    {
        var allSymbols = new SlotSymbol[9];
        int i = 0;
        for (int j = 0; j < substitutionReels.Length; j++)
        {
            var childCount = substitutionReels[j].childCount;
            for (int k = 0; k < childCount; k++)
            {
                var symbol = substitutionReels[j].GetChild(k).GetComponent<SlotSymbol>();
                if (symbol != null && symbol.GetComponent<RectTransform>().localPosition.y < 300)
                {
                    allSymbols[i] = symbol;
                    i++;                    
                }
            }
        }
        foreach (var symbol in allSymbols) print(symbol);
        return allSymbols;
    }
}
