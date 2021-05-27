using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WinLine", menuName = "WinLine")]
public class WinLine : ScriptableObject
{
    [SerializeField] private int[] winSymbols;

    public int[] WinSymbols { get => winSymbols; }
}
