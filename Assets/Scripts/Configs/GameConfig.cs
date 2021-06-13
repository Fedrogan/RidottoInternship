using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Config", menuName = "Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private FinalScreenSO[] finalScreens;

    [SerializeField] private SymbolData[] symbolsData;
    [Space]
    [SerializeField] private int visibleSymbolsOnReel;

    public FinalScreenSO[] FinalScreens  => finalScreens;

    public SymbolData[] SymbolsData => symbolsData;

    public int VisibleSymbolsOnReel => visibleSymbolsOnReel;
}
