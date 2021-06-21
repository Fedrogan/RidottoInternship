using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrizeCalculator : MonoBehaviour
{
    public event Action<float> PrizeCalculated;
    public float CalculateWin(List<Symbol[]> winningSymbols)
    {
        float prize = 0;

        foreach (var line in winningSymbols)
        {
            for (int i = 0; i < line.Length; i += 3)
            {
                if (line[i].SymbolSO != null) prize += line[i].SymbolSO.SymbolCost;
            }            
        }
        return prize;
    }    
}