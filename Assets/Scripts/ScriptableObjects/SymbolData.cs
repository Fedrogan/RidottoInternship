﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol Data", menuName = "Symbol Data")]
public class SymbolData : ScriptableObject
{
    [SerializeField] private string symbolName;
    [SerializeField] private Sprite symbolImage;
    //private int symbolID;

    public string SymbolName { get => symbolName; }
    public Sprite SymbolImage { get => symbolImage; set => symbolImage = value; }
    //public int SymbolID { get => symbolID; set => symbolID = value; }
}