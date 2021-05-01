using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    #region FIELDS  
    [SerializeField] private List<SymbolData> symbols;
    [SerializeField] List<FinalScreenSO> finalScreens;
    [SerializeField] SpinManagement spinManager;
    
    public GameObject playButton, stopButton;

    //для проверки на каком риле находится символ
    [SerializeField] private List<RectTransform> reelAnchors;

    //для чтения финальных символов
    private int firstReelSymbol;
    private int secondReelSymbol;
    private int thirdReelSymbol;
    private int currentSet;

    private bool isSlowingDown;
    #endregion

    void Start()
    {
        stopButton.SetActive(false);
        ResetSymbols();
        currentSet = -1; 
    }
    public void StartSpin()
    {
        playButton.SetActive(false);
        spinManager.StartSpinning();
        ResetSymbols();
        NextSet();
    }

    public void StopSpin()
    {
        stopButton.SetActive(false);
        spinManager.SlowdownSpin();
    }

    private void ResetSymbols()
    {
        firstReelSymbol = -3;
        secondReelSymbol = -2;
        thirdReelSymbol = -1;
    }

    private void NextSet()
    {
        _ = currentSet >= finalScreens.Count - 1 ? currentSet = 0 : currentSet += 1;
    }

    private int GetFinalSymbolIndex(RectTransform symbol)
    {       
        switch(CheckSymbolReel(symbol))
        {
            case 1:
                return firstReelSymbol += 3;
            case 2:
                return secondReelSymbol += 3;
            case 3:
                return thirdReelSymbol += 3;
            default:
                return 0;
        }        
    }

    private int CheckSymbolReel(RectTransform symbol)
    {
        var currentReel = 0;

        if (symbol.IsChildOf(reelAnchors[0])) currentReel = 1;
        if (symbol.IsChildOf(reelAnchors[1])) currentReel = 2;
        if (symbol.IsChildOf(reelAnchors[2])) currentReel = 3;  
        
        return currentReel;
    }
    public SymbolData GetFinalScreenSymbol(RectTransform symbol)
    {
        var index = GetFinalSymbolIndex(symbol);
        var newSymbol = finalScreens[currentSet].FinalScreenSymbols[index];
        return newSymbol != null ? newSymbol : GetRandomSymbol();        
    }
    public SymbolData GetRandomSymbol()
    {
        var random = Random.Range(0, 11);
        return symbols[random];
    }    
} 
