using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    #region FIELDS  
    
    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private GameObject playButton, stopButton;    
    #endregion

    void Start()
    {
        SetStopButtonActive(false);        
        symbolsManager.ResetSymbols();        
    }
    public void StartSpin()
    {
        SetPlayButtonActive(false);
        reelsScroller.StartSpinning();
        symbolsManager.ResetSymbols();
        symbolsManager.NextSet();
    }

    public void StopSpin()
    {
        stopButton.SetActive(false);
        reelsScroller.SlowdownSpin();
    }
    public void SetPlayButtonActive(bool active)
    {
        playButton.SetActive(active);
    }
    public void SetStopButtonActive(bool active)
    {
        stopButton.SetActive(active);
    }

    public void CheckWin()
    {
        winLinesChecker.CheckWin();
    }
} 
