using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeSpinsCounter : MonoBehaviour
{
    [SerializeField] private CanvasGroup freeSpinsCounter;
    [SerializeField] private Text freeSpinsLeftCounterText;

    public void ShowCounter()
    {
        freeSpinsCounter.alpha = 1;
        freeSpinsCounter.transform.localScale = Vector3.one;
    }

    public void HideCounter()
    {
        freeSpinsCounter.alpha = 0;
        freeSpinsCounter.transform.localScale = Vector3.zero;
    }

    public void SetFreeSpinsValue(int freeSpinsLeft)
    {
        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();
    }
}
