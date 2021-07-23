using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TotalFSPrizePopup : MonoBehaviour, IPopup
{
    public event Action<bool> ActiveStatusChanged;
    private bool isActive;

    [SerializeField] private Image shadow;
    [SerializeField] private Button clickableShadow;

    [SerializeField] private CanvasGroup totalPrizePopUp;
    [SerializeField] private RectTransform totalPrizePopUpRT;
    [SerializeField] private TextMeshProUGUI totalPrizeText;

    private void Start()
    {
        clickableShadow.onClick.AddListener(HidePopup);
    }
    public void HidePopup()
    {
        clickableShadow.interactable = false;
        shadow.raycastTarget = false;
        shadow.DOFade(0f, 0.1f);
        totalPrizePopUpRT.DOScale(Vector3.zero, 0.7f).OnComplete(() => totalPrizePopUp.alpha = 0);

        isActive = false;
        ActiveStatusChanged?.Invoke(isActive);
    }

    public void ShowPopup(int? ignore, float? prize)
    {
        isActive = true;
        ActiveStatusChanged?.Invoke(isActive);
        StartCoroutine(CoShowTotalPrizePopUp((float)prize));
    }
    private IEnumerator CoShowTotalPrizePopUp(float prize)
    {
        shadow.raycastTarget = true;
        yield return new WaitForSecondsRealtime(0.7f);
        totalPrizeText.text = prize.ToString();
        totalPrizePopUp.alpha = 1;
        shadow.DOFade(0.65f, 0.2f);
        totalPrizePopUpRT.DOScale(Vector3.one, 0.7f).OnComplete(() =>
        {
            clickableShadow.interactable = true;
        });
    }
}
