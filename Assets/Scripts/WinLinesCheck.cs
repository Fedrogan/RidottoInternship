using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WinLinesCheck : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;

    [SerializeField] private SubReel[] subReels;

    private Symbol[] winLineSymbols;

    private void Start()
    {
        winLineSymbols = new Symbol[subReels.Length];
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

    //public int 
}
