using UnityEngine;

public class SubReel : MonoBehaviour
{
    [SerializeField] private int reelId;
    [SerializeField] private Symbol[] visibleReelSymbols;
    [SerializeField] private Symbol invisibleSymbol;
    private ReelState reelState = ReelState.Stop;

    [SerializeField] private GameConfig gameConfig;
    private int currentSet;

    public int ReelID { get => reelId; set => reelId = value; }
    public ReelState ReelState { get => reelState; set => reelState = value; }
    public Symbol[] VisibleReelSymbols => visibleReelSymbols;


    void Start()
    {
        currentSet = 0;
        FillReel();
    }

    public void FillReel()
    {
        for (int i = 0; i < visibleReelSymbols.Length; i++)
        {
            var finalScreenSymbolIndex = i + (reelId - 1) * visibleReelSymbols.Length;
            var symbol = visibleReelSymbols[i];
            symbol.SymbolSO = gameConfig.FinalScreens[currentSet].FinalScreenSymbols[finalScreenSymbolIndex] == null ?
                    GetRandomSymbol() : gameConfig.FinalScreens[currentSet].FinalScreenSymbols[finalScreenSymbolIndex];
            symbol.Icon.sprite = symbol.SymbolSO.SymbolImage;
        }
        invisibleSymbol.Icon.sprite = GetRandomSymbol().SymbolImage;
        NextSet();
    }

    public void NextSet()
    {
        _ = currentSet >= gameConfig.FinalScreens.Length - 1 ? currentSet = 0 : currentSet += 1;
    }

    private SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, gameConfig.SymbolsData.Length - 1);
        return gameConfig.SymbolsData[random];
    }
}