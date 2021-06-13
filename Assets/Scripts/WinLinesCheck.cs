using UnityEngine;


public class WinLinesCheck : MonoBehaviour
{
    
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLine[] winLines;

    [SerializeField] private SubReel[] subReels;

    private Symbol[] winLineSymbols;

    private void Start()
    {
        GameController.Instance.SpinFinished += CheckWin;
        winLineSymbols = new Symbol[subReels.Length];
    }

    public void CheckWin()
    {
        foreach (var winLine in winLines)
        {
            for (int i = 0; i < winLine.WinSymbols.Length; i++)
            {
                winLineSymbols[i] = subReels[i].VisibleReelSymbols[winLine.WinSymbols[i]];                    
            }

            if (winLineSymbols[0].SymbolSO == winLineSymbols[1].SymbolSO && winLineSymbols[1].SymbolSO == winLineSymbols[2].SymbolSO)           
            {
                animationsManager.AddWinLineToShowList(winLineSymbols);
            }
        }
        animationsManager.StartAnimations();
    }
}
