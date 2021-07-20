using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WinLinesCheck : MonoBehaviour
{
    public event Action<int> FreeSpinsDetected;
    public event Action<List<Symbol[]>> WinLinesChecked;

    [SerializeField] private FreeSpinsController bonusGameController;
    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private PrizeCalculator prizeCalculator;

    [SerializeField] private GameConfig bonusGameConfig;
    [SerializeField] private GameConfig ordinaryGameConfig;
    private GameConfig gameConfig;

    [SerializeField] private SubReel[] subReels;

    private Symbol[] winLineSymbols;

    private void Start()
    {
        gameConfig = ordinaryGameConfig;
        reelsScroller.AllReelsStopped += GetWinLines;
        prizeCalculator.PrizeCalculated += CheckFSGame;
        bonusGameController.FreeSpinsStarted += SetBonusConfig;
        bonusGameController.FreeSpinsFinished += SetOrdinaryConfig;
        winLineSymbols = new Symbol[subReels.Length];
    }
    private void SetOrdinaryConfig(float ignoreValue)
    {
        gameConfig = ordinaryGameConfig;
    }

    private void SetBonusConfig()
    {
        gameConfig = bonusGameConfig;
    }

    public void GetWinLines()
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
        WinLinesChecked?.Invoke(winningLines);
    }

    public void CheckFSGame(float ignore)
    {
        var scattersInReel = 0;
        var reelsWithScatters = 0;
        var scattersDetected = 0;
        foreach (var subReel in subReels)
        {
            foreach (var symbol in subReel.VisibleReelSymbols)
            {
                if (symbol.SymbolSO.SymbolType == SymbolType.Scatter)
                {
                    scattersInReel++;
                    scattersDetected++;
                }
            }
            if (scattersInReel > 0)
            {
                scattersInReel = 0;
                reelsWithScatters++;
            }
            else break;
        }
        if (reelsWithScatters == subReels.Length)
        {
            FreeSpinsDetected?.Invoke(scattersDetected);
        }
    }
}
