using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WinLinesCheck : MonoBehaviour
{
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private List<WinLine> winLines;
    [SerializeField] private List<RectTransform> reels;
    [SerializeField] private Image shadow;
    



    private List<SlotSymbol> winningSymbols = new List<SlotSymbol>();
    private List<SlotSymbol> symbolsInLine = new List<SlotSymbol>();
    private List<SlotSymbol> allSymbols = new List<SlotSymbol>();

    public void CheckWin()
    {       
        foreach (var winLine in winLines)
        {
            for (int i = 0; i < reels.Count; i++)
            {
                var symbol = symbolsManager.GetSymbolOnReelById(reels[i], winLine.WinSymbols[i]);
                symbolsInLine.Add(symbol);                
            }
            var symbolsInLineData = GetSymbolsData(symbolsInLine);
            if (symbolsInLineData.Distinct().Count() == 1)
            {
                winningSymbols.AddRange(symbolsInLine);
            }
            symbolsInLine.Clear();
        }
        SetWinningSymbolsMaskable(false);
        if (winningSymbols.Count > 0)
        {
            shadow.color = new Color(0f, 0f, 0f, 0.5f);
            animationsManager.ShowWinAnimation(winningSymbols);
        }
        
    }

    private List<SymbolData> GetSymbolsData(List<SlotSymbol> symbolsInLine)
    {
        var symbolsData = new List<SymbolData>();
        foreach (var symbol in symbolsInLine)
        {
            symbolsData.Add(symbol.SymbolSO);
        }
        return symbolsData;
    }    

    public void ResetWinCheck()
    {
        DOTween.KillAll();        
        foreach (var winningSymbol in winningSymbols)
        {
            SetWinningSymbolMaskable(winningSymbol, true);
            var symbolRT = winningSymbol.GetComponent<RectTransform>();
            symbolRT.localScale = new Vector3(1, 1, 1);
            print(winningSymbol.DefaultParentReel);
            winningSymbol.transform.SetParent(winningSymbol.DefaultParentReel);
        }
        winningSymbols.Clear();
        symbolsInLine.Clear();
        allSymbols.Clear();
        shadow.color = new Color(0f, 0f, 0f, 0f);
    }

    private void SetWinningSymbolsMaskable(bool maskable)
    {
        foreach (var winningSymbol in winningSymbols)
        {
            winningSymbol.GetComponent<Image>().maskable = maskable;
        }
    }
    private void SetWinningSymbolMaskable(SlotSymbol symbol, bool maskable)
    {
        symbol.GetComponent<Image>().maskable = maskable;
    }
}
