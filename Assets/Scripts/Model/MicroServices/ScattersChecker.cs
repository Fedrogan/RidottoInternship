using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattersChecker : MonoBehaviour
{
    [SerializeField] private SubReel[] subReels;

    public int CheckScattersOnAllReels()
    {
        var scattersInReel = 0;
        var reelsWithScatters = 0;
        var scattersDetected = 0;
        foreach (var subReel in subReels)
        {
            foreach (var symbol in subReel.VisibleReelSymbols)
            {
                if (symbol.SymbolSO.SymbolType == SymbolType.Scatter)
                {
                    scattersInReel++;
                    scattersDetected++;
                }
            }
            if (scattersInReel > 0)
            {
                scattersInReel = 0;
                reelsWithScatters++;
            }
            else break;
        }
        if (reelsWithScatters == subReels.Length)
        {
            return scattersDetected;
        }
        else return 0;
    }

    public int CheckScattersOnReel(SubReel subReel)
    {
        var scattersInReel = 0;
        foreach (var symbol in subReel.VisibleReelSymbols)
        {
            if (symbol.SymbolSO.SymbolType == SymbolType.Scatter)
            {
                scattersInReel++;
            }
        }
        return scattersInReel;
    }
}
