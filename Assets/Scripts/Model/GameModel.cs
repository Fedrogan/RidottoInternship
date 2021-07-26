using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    public Action<int> FreeSpinsStarted;
    public Action FreeSpinsFinished;
    [SerializeField] private GameType gameType;

    [SerializeField] private float totalWinning;
    [SerializeField] private float lastWinning;
    [SerializeField] private float freeSpinsWinning;
    [SerializeField] private float prevWinning;

    [SerializeField] private GameConfig gameConfig;
    private int freeSpinsLeft;
    private bool isFirstFreeSpin = true;

    public GameType GameType => gameType;

    public float TotalWinning => totalWinning;

    public float LastWinning => lastWinning;

    public int FreeSpinsLeft => freeSpinsLeft;

    public float FreeSpinsWinning => freeSpinsWinning;

    public float PrevWinning => prevWinning;

    public bool IsFirstFreeSpin => isFirstFreeSpin;

    void Start()
    {
        gameType = GameType.Ordinary;
        totalWinning = 0;
        lastWinning = 0;
    }
    
    public void UpdateGame(float newWinning, int scattersDetected, Action<int> startFSCallBack, Action finishFSCallBack)
    {
        UpdateWinnings(newWinning);
        if (scattersDetected > 0)
        {
            if (gameType == GameType.FreeSpins)
            {
                freeSpinsLeft += gameConfig.ScattersToFreeSpinsMap[scattersDetected];
            }
            if (gameType == GameType.Ordinary)
            {
                freeSpinsWinning = 0;
                gameType = GameType.FreeSpins;
                freeSpinsLeft = gameConfig.ScattersToFreeSpinsMap[scattersDetected];
                startFSCallBack?.Invoke(freeSpinsLeft);
            }
        }        

        if (freeSpinsLeft > 0)
        {
            freeSpinsLeft--;
            isFirstFreeSpin = false;
            return;
        }

        if (freeSpinsLeft == 0 && gameType == GameType.FreeSpins)
        {
            gameType = GameType.Ordinary;            
            finishFSCallBack?.Invoke();            
        }
    }

    private void UpdateWinnings(float newWinning)
    {
        lastWinning = newWinning;
        totalWinning += lastWinning;

        if (gameType == GameType.FreeSpins)
        {
            prevWinning = freeSpinsWinning;
            freeSpinsWinning += lastWinning;
        }        
    }
}
