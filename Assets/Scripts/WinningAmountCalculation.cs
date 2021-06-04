﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;

public class WinningAmountCalculation : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float countDuration;
    [SerializeField] private CanvasGroup counterDisabler;
    [SerializeField] private CanvasGroup currencyDisabler;

    public event Action StartChangingPrize;
    public event Action FinishChangingPrize;

    private float value;
    private bool isInterrupted;
    private TweenerCore<float, float, FloatOptions> tween;

    public void CalculateWin(List<SlotSymbol[]> winningSymbols)
    {
        print("Calculate");
        print(winningSymbols.Count);
        float winningAmount = 0;

        foreach (var line in winningSymbols)
        {
            print("в цикле");
            for (int i = 0; i < line.Length; i += 3)
            {
                print(line[i].SymbolSO != null);
                if (line[i].SymbolSO != null)
                    print(line[i].SymbolSO.SymbolCost);
                    winningAmount += line[i].SymbolSO.SymbolCost;
            }            
        }
        value = winningAmount;
        print(value);
        UpdateValue(0, winningAmount);
    }

    public void UpdateValue(float prevValue, float newValue, bool immediate = false)
    {
        isInterrupted = false;

        if (immediate || Mathf.Approximately(prevValue, newValue))
        {
            StopAllCoroutines();

            isInterrupted = true;

            tween.Complete();

            SetText(newValue);

            ShowText(newValue);
        }
        else
        {
            ShowText(newValue);
            StartChangingPrize?.Invoke();
            value = prevValue;
            tween = DOTween.To(() => value, (x) => value = x, newValue, countDuration);
            tween.onComplete = () =>
            {
                if (!isInterrupted)
                {
                    SetText(value);
                    StopAllCoroutines();
                    ShowText(newValue);
                }

                FinishChangingPrize?.Invoke();
            };

            StartCoroutine(CoCount());
        }
    }

    private IEnumerator CoCount()
    {
        while (true)
        {
            yield return null;

            if (!isInterrupted)
            {
                SetText(value);
            }
        }
    }

    private void SetText(float value)
    {
        string temp;
        if (value.ToString().Split(',').Length == 2)
        {
            var tempString = value.ToString().Split(',');
            if (tempString[1].Length > 2) temp = tempString[1].Remove(2);
            else temp = null;
            var newString = tempString[0] + "," + temp;
            text.text = newString;
        }
        else
        {
            temp = value.ToString();

            text.text = temp;
        }

    }

    private void ShowText(float value)
    {
        if (counterDisabler != null)
        {
            counterDisabler.alpha = value > 0 ? 1f : 0f;
        }

        if (currencyDisabler != null)
        {
            currencyDisabler.alpha = value > 0 ? 1f : 0f;
        }
    }

    public void ResetCounter()
    {
        isInterrupted = true;
        counterDisabler.alpha = 0f;
        currencyDisabler.alpha = 0f;
    }
}