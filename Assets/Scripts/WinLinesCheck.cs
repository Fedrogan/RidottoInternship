using UnityEngine;


public class WinLinesCheck : MonoBehaviour
{
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLine[] winLines;
    //[SerializeField] private RectTransform[] reels;

    [SerializeField] private RectTransform[] fakeReels;

    [SerializeField] private ReelsSymbolManager reelsSymbolManager;

    private SlotSymbol[] winLineSymbols;
    private SymbolData[] symbolsInLineData;

    private void Start()
    {
        GameController.Instance.SpinFinished += CheckWin;
        winLineSymbols = new SlotSymbol[fakeReels.Length];
    }

    public void CheckWin()
    {
        foreach (var winLine in winLines)
        {
            print(winLine.WinSymbols.Length);
            for (int i = 0; i < winLine.WinSymbols.Length; i++)
            {
                winLineSymbols[i] = reelsSymbolManager.GetSymbolOnReelById(fakeReels[i], winLine.WinSymbols[i]);
            }
            symbolsInLineData = reelsSymbolManager.GetSymbolsData(winLineSymbols);
            //print("First symbol " + symbolsInLineData[0]);
            //print("Second symbol " + symbolsInLineData[1]);
            //print("Third symbol " + symbolsInLineData[2]);

            if (symbolsInLineData[1] == symbolsInLineData[0] && symbolsInLineData[2] == symbolsInLineData[1])
            {
                animationsManager.AddWinLineToShowList(winLineSymbols);
            }
        }
        animationsManager.StartAnimations();
    }
}
