using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Config", menuName = "Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private FinalScreenSO[] finalScreens;

    [SerializeField] private SymbolData[] symbolsData;

    [SerializeField] private WinLine[] winLines;
    [Space]
    [SerializeField] private int visibleSymbolsOnReel;

    private Dictionary<int, int> scattersToFreeSpinsMap = new Dictionary<int, int>()
    {
        {3, 10},
        {4, 10},
        {5, 15},
        {6, 15},
        {7, 15},
        {8, 20},
        {9, 20},
    };

    public FinalScreenSO[] FinalScreens  => finalScreens;

    public SymbolData[] SymbolsData => symbolsData;

    public int VisibleSymbolsOnReel => visibleSymbolsOnReel;

    public WinLine[] WinLines => winLines;

    public Dictionary<int, int> ScattersToFreeSpinsMap => scattersToFreeSpinsMap;
}
