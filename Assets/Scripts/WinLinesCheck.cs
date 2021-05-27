using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WinLinesCheck : MonoBehaviour
{
    [SerializeField] private List<WinLine> winLines;
    [SerializeField] private RectTransform reel1;
    [SerializeField] private RectTransform reel2;
    [SerializeField] private RectTransform reel3;
    [SerializeField] private float scaleDuration;

    private Vector3 scale = new Vector3(1.1f, 1.1f);
    private Vector3 pulseScale = new Vector3(1.2f, 1.2f);
    private Vector3 rotation = new Vector3(0, 0, 720);

    private List<SlotSymbol> winningSymbols = new List<SlotSymbol>();

    
    private SlotSymbol GetSymbolOnReelById(RectTransform reel, int id)
    {
        var childCount = reel.childCount;
        var symbolsOnReel = new List<SlotSymbol>();
        
        for (int i = 0; i < childCount; i++)
        {
            if (reel.GetChild(i).GetComponent<Transform>().localPosition.y != 0)
                symbolsOnReel.Add(reel.GetChild(i).GetComponent<Transform>().GetComponent<SlotSymbol>());
        }
        var sortedSymbols = symbolsOnReel.OrderBy(x => x.GetComponent<Transform>().localPosition.y);
        var sortedSymbolsArray = sortedSymbols.ToArray();
        return sortedSymbolsArray[id];
    }

    public void CheckWin()
    {
        foreach (var winLine in winLines)
        {
            var symbol1 = GetSymbolOnReelById(reel1, winLine.WinSymbols[0]);
            var symbol2 = GetSymbolOnReelById(reel2, winLine.WinSymbols[1]);
            var symbol3 = GetSymbolOnReelById(reel3, winLine.WinSymbols[2]);
            if (symbol1.SymbolSO.SymbolID == symbol2.SymbolSO.SymbolID && symbol1.SymbolSO.SymbolID == symbol3.SymbolSO.SymbolID)
            {
                winningSymbols.Add(symbol1);
                winningSymbols.Add(symbol2);
                winningSymbols.Add(symbol3);
            }
        }
        foreach (var winningSymbol in winningSymbols)
        {
            ShowWinAnimation(winningSymbol);
            
        }
        SetAllSymbolsMaskable();
        winningSymbols.Clear();
        
    }

    private void ShowWinAnimation(SlotSymbol winningSymbol)
    {
        var symbolRT = winningSymbol.GetComponent<RectTransform>();
        var symbolImage = winningSymbol.GetComponent<Image>();
        symbolImage.maskable = false;
        var tweener1 = symbolRT.DOScale(scale, 0.5f).OnComplete(() => 
        {
            var pulseTween = symbolRT.DOScale(pulseScale, 0.5f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
            {
                var backTween = symbolRT.DOScale(1, 1.2f);                                
            });
        });  
    }

    private void SetAllSymbolsMaskable()
    {
        foreach (var winningSymbol in winningSymbols)
        {
            winningSymbol.GetComponent<Image>().maskable = true;
        }
    }
}
