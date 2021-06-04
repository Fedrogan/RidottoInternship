using UnityEngine;


public class WinLinesCheck : MonoBehaviour
{
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLine[] winLines;
    [SerializeField] private RectTransform[] reels;

    private SlotSymbol[] winLineSymbols;
    private SymbolData[] symbolsInLineData;

    private void Start()
    {
        GameController.Instance.SpinFinished += CheckWin;
        winLineSymbols = new SlotSymbol[reels.Length];
    }

    public void CheckWin()
    {
        foreach (var winLine in winLines)
        {
            for (int i = 0; i < winLine.WinSymbols.Length; i++)
            {
                winLineSymbols[i] = symbolsManager.GetSymbolOnReelById(reels[i], winLine.WinSymbols[i]);
            }
            symbolsInLineData = symbolsManager.GetSymbolsData(winLineSymbols);
            
            if (symbolsInLineData[1] == symbolsInLineData[0] && symbolsInLineData[2] == symbolsInLineData[1])
            {
                animationsManager.AddWinLineToShowList(winLineSymbols);
            }
        }
        animationsManager.StartAnimations();
    }    
}
