using UnityEngine;

public class SubReel : MonoBehaviour
{
    [SerializeField] private int reelId;
    [SerializeField] private Symbol[] visibleReelSymbols;
    [SerializeField] private Symbol invisibleSymbol;
    private ReelState reelState = ReelState.Stop;

    //[SerializeField] private FreeSpinsController bonusGameController;
    [SerializeField] private GameController gameController;

    [SerializeField] private GameConfig bonusGameConfig;
    [SerializeField] private GameConfig ordinaryGameConfig;
    private GameConfig gameConfig;

    private int currentSet;
    private int lastSet;

    public int ReelID { get => reelId; set => reelId = value; }
    public ReelState ReelState { get => reelState; set => reelState = value; }
    public Symbol[] VisibleReelSymbols => visibleReelSymbols;


    void Start()
    {
        gameConfig = ordinaryGameConfig;
        gameController.FreeSpinsStarted += SetBonusConfig;
        gameController.FreeSpinsFinished += SetOrdinaryConfig;
        currentSet = 0;
        FillReel();
    }

    private void SetOrdinaryConfig()
    {
        currentSet = lastSet;
        lastSet = 0;
        gameConfig = ordinaryGameConfig;
    }

    private void SetBonusConfig(int ignoreValue)
    {
        lastSet = currentSet;
        currentSet = 0;
        gameConfig = bonusGameConfig;
    }

    public void FillReel()
    {
        for (int i = 0; i < visibleReelSymbols.Length; i++)
        {
            var finalScreenSymbolIndex = i + (reelId - 1) * visibleReelSymbols.Length;
            var symbol = visibleReelSymbols[i];
            var newSymbol = gameConfig.FinalScreens[currentSet].FinalScreenSymbols[finalScreenSymbolIndex];
            if (newSymbol != null) 
                symbol.SymbolSO = newSymbol;
            else 
                symbol.SymbolSO = GetRandomSymbol();
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