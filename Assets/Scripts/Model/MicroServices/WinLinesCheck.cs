using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WinLinesCheck : MonoBehaviour
{
    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private PrizeCalculator prizeCalculator;
    [SerializeField] private GameController gameController;

    [SerializeField] private GameConfig bonusGameConfig;
    [SerializeField] private GameConfig ordinaryGameConfig;
    private GameConfig gameConfig;

    [SerializeField] private SubReel[] subReels;

    private Symbol[] winLineSymbols;

    private void Start()
    {
        gameConfig = ordinaryGameConfig;
        gameController.FreeSpinsStarted += SetBonusConfig;
        gameController.FreeSpinsFinished += SetOrdinaryConfig;
        winLineSymbols = new Symbol[subReels.Length];
    }
    private void SetOrdinaryConfig()
    {
        gameConfig = ordinaryGameConfig;
    }

    private void SetBonusConfig(int ignoreValue)
    {
        gameConfig = bonusGameConfig;
    }

    public List<Symbol[]> GetWinLines()
    {
        List<Symbol[]> winningLines = new List<Symbol[]>();
        foreach (var winLine in gameConfig.WinLines)
        {
            for (int i = 0; i < winLine.WinSymbols.Length; i++)
            {
                winLineSymbols[i] = subReels[i].VisibleReelSymbols[winLine.WinSymbols[i]];
            }

            if (winLineSymbols[0].SymbolSO == winLineSymbols[1].SymbolSO && winLineSymbols[1].SymbolSO == winLineSymbols[2].SymbolSO)
            {
                var newLine = winLineSymbols.Clone() as Symbol[];
                winningLines.Add(newLine);
            }
        }
        return winningLines;
    } 
}
