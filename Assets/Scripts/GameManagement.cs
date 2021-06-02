using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        winLinesChecker.ResetWinCheck();
        SetPlayButtonActive(false);
        reelsScroller.StartSpinning();
        symbolsManager.ResetSymbols();
        if (symbolsManager.IsRandomGame == false) symbolsManager.NextSet();
    }

    public void StopSpin()
    {
        stopButton.SetActive(false);
        reelsScroller.SlowdownSpin();
    }
    public void SetPlayButtonActive(bool active)
    {
        //playButton.SetActive(active);
        playButton.GetComponent<Button>().interactable = active;
    }
    public void SetStopButtonActive(bool active)
    {
        //stopButton.SetActive(active);
        stopButton.GetComponent<Button>().interactable = active;
    }

    public void CheckWin()
    {
        winLinesChecker.CheckWin();
    }
} 
