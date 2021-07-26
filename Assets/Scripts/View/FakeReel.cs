using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FakeReel : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private RectTransform mainCanvasRT;
    [SerializeField] private Symbol[] fakeReelSymbols;

    [SerializeField] private int reelID;
    private ReelState reelState = ReelState.Stop;

    private readonly int reelSymbolsCount = 4;
    private readonly int defaultExitYPos = 240;

    private Symbol symbol;
    private float symbolHeigth;
    private float mainCanvasScale;

    public int ReelID => reelID;

    public ReelState ReelState { get => reelState; set => reelState = value; }

    void Start()
    {
        mainCanvasScale = mainCanvasRT.lossyScale.y;        
    }

    void Update()
    {
        for (int i = 0; i < fakeReelSymbols.Length; i++)
        {
            symbol = fakeReelSymbols[i];            
            CheckSymbolPosition(symbol);
        }        
    }        

    private void CheckSymbolPosition(Symbol symbol)
    {
        if (symbol.SymbolRT.position.y <= defaultExitYPos * mainCanvasScale)
        {
            ChangeSymbolPosition(symbol);
        }
    }

    private void ChangeSymbolPosition(Symbol symbol)
    {
        symbolHeigth = symbol.SymbolRT.rect.height;
        var offset = symbol.SymbolRT.position.y + symbolHeigth * mainCanvasScale * reelSymbolsCount;
        var newPos = new Vector3(symbol.SymbolRT.position.x, offset, symbol.SymbolRT.position.z);
        symbol.SymbolRT.position = newPos;
            ChangeSprite(symbol);
    }

    private void ChangeSprite(Symbol symbol)
    {
        if (symbol.IsHidden == true) 
            symbol.SetSymbolTransparency(true);
        else 
            symbol.SetSymbolTransparency(false);

        var newSymbol = GetRandomSymbol().SymbolImage;        
        symbol.Icon.sprite = newSymbol;
    }

    public void ResetSymbolsPosition(float cellYCorrection, float startReelPositionY)
    {
        for (int i = 0; i < fakeReelSymbols.Length; i++)
        {
            var symbol = fakeReelSymbols[i];
            var symbolPos = symbol.SymbolRT.localPosition;
            var newYPos = Mathf.Round(symbolPos.y - cellYCorrection - startReelPositionY);
            symbol.SymbolRT.localPosition = new Vector3(symbolPos.x, newYPos, symbolPos.z);
        }
    }

    public void MakeAllSymbolsTransparent()
    {
        foreach (var symbol in fakeReelSymbols)
        {
            symbol.SetSymbolTransparency(true);
        }
    }

    public void HideSymbolsOnFakeReel(bool isChangeable)
    {
        for (int i = 0; i < fakeReelSymbols.Length; i++)
        {
            var symbol = fakeReelSymbols[i];
            symbol.IsHidden = isChangeable;
        }
    }    
        
    private SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, gameConfig.SymbolsData.Length);
        return gameConfig.SymbolsData[random];
    }     
}