using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSymbol : MonoBehaviour
{
    [SerializeField] private SymbolData symbolSO;

    public SymbolData SymbolSO { get => symbolSO; set => symbolSO = value; }
}
