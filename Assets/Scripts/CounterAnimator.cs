using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;

public class CounterAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float countDuration;
    [SerializeField] private CanvasGroup counterDisabler;
    [SerializeField] private CanvasGroup currencyDisabler;
    [SerializeField] private PrizeCalculator calculator;

    //public event Action StartChangingPrize;
    //public event Action FinishChangingPrize;

    private float value;
    private bool isInterrupted;
    private TweenerCore<float, float, FloatOptions> tween;

    private void Start()
    {
        GameController.Instance.ReelsStarted += ResetCounter;
        calculator.PrizeCalculated += OnShowCounterAnimation;
    }
    private void OnShowCounterAnimation(float prizeAmount)
    {
        UpdateValue(0, prizeAmount);
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
            //StartChangingPrize?.Invoke();
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

                //FinishChangingPrize?.Invoke();
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
