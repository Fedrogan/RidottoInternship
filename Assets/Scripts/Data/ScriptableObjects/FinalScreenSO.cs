using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Final Screen", menuName = "Final Screen")]
public class FinalScreenSO : ScriptableObject
{
    [SerializeField] private List<SymbolData> finalScreenSymbols;
    public List<SymbolData> FinalScreenSymbols  => finalScreenSymbols;
}