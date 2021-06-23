using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WinLinesCheck : MonoBehaviour
{
    public event Action<int> FreeSpinsDetected;

    [SerializeField] private BonusGameController bonusGameController;

    [SerializeField] private GameConfig bonusGameConfig;
    [SerializeField] private GameConfig ordinaryGameConfig;
    private GameConfig gameConfig;

    [SerializeField] private SubReel[] subReels;

    private Symbol[] winLineSymbols;

    private void Start()
    {
        gameConfig = ordinaryGameConfig;
        bonusGameController.BonusGameStarted += SetBonusConfig;
        bonusGameController.BonusGameFinished += SetOrdinaryConfig;
        winLineSymbols = new Symbol[subReels.Length];
    }
    private void SetOrdinaryConfig()
    {
        gameConfig = ordinaryGameConfig;
    }

    private void SetBonusConfig()
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

    public void CheckFSGame()
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

    public bool CheckAnticipation(SubReel firstSubReel, SubReel secondSubReel)
    {
        var scattersOnFirst = CheckScattersOnReel(firstSubReel);
        var scattersOnSecond = CheckScattersOnReel(secondSubReel);
        return scattersOnFirst > 0 && scattersOnSecond > 0 ? true : false;
    }
        
    private int CheckScattersOnReel(SubReel subReel)
    {
        var scattersInReel = 0;
        foreach (var symbol in subReel.VisibleReelSymbols)
        {
            if (symbol.SymbolSO.SymbolType == SymbolType.Scatter)
            {
                scattersInReel++;
            }
        }
        return scattersInReel;
    }
}
