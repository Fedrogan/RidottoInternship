using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    [SerializeField] private GameType gameType;
    [SerializeField] private float totalWinning;
    [SerializeField] private float lastWinning;

    public GameType GameType => gameType;
    public float TotalWinning => totalWinning;

    public float LastWinning => lastWinning;

    void Start()
    {
        gameType = GameType.Ordinary;
        totalWinning = 0;
        lastWinning = 0;
    }
    
    public void UpdateGame(float newWinning, int freeSpinsLeft)
    {
        SetGameType(freeSpinsLeft);
        UpdateWinnings(newWinning);
    }

    private void SetGameType(int freeSpinsLeft)
    {
        if (freeSpinsLeft > 0) gameType = GameType.FreeSpins;
        else gameType = GameType.Ordinary;        
    }

    private void UpdateWinnings(float newWinning)
    {
        if (gameType == GameType.FreeSpins)
        {
            lastWinning += newWinning;            
        }
        else
        {
            lastWinning = newWinning;            
        }
        totalWinning += newWinning;
    }
}
