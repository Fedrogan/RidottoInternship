using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    public Action<List<Symbol[]>, float, int> LinesChecked;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private PrizeCalculator prizeCalculator;
    [SerializeField] private ReelsScroll reelsScroll;
    private List<Symbol[]> winLines;
    private int freeSpinsDetected;
    private float currentPrize;

    private void Start()
    {
        reelsScroll.AllReelsStopped += CheckLines;
    }

    private void CheckLines()
    {
        freeSpinsDetected = winLinesChecker.CheckFSGame();
        winLines = winLinesChecker.GetWinLines();
        currentPrize = prizeCalculator.CalculateWin(winLines);

        LinesChecked?.Invoke(winLines, currentPrize, freeSpinsDetected);
    }
}
