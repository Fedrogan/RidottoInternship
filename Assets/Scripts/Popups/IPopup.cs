using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPopup
{
    event Action<bool> ActiveStatusChanged;
    void ShowPopup(int? freeSpins = null, float? prize = null, Action callBack = null);
    void HidePopup();
}