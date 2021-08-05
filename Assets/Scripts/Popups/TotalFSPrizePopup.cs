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
    private Action callBack;
    private bool isActive;

    [SerializeField] private Image shadow;
    [SerializeField] private Button clickableShadow;

    [SerializeField] private CanvasGroup totalPrizePopUp;
    [SerializeField] private RectTransform totalPrizePopUpRT;
    [SerializeField] private TextMeshProUGUI totalPrizeText;

    [SerializeField] private AudioController audioController;

    private void Start()
    {
        clickableShadow.onClick.AddListener(HidePopup);
    }
    public void HidePopup()
    {
        audioController.PlayHidePopupSound();
        clickableShadow.interactable = false;
        shadow.raycastTarget = false;
        shadow.DOFade(0f, 0.1f);
        totalPrizePopUpRT.DOScale(Vector3.zero, 0.7f).OnComplete(() => totalPrizePopUp.alpha = 0);

        isActive = false;
        callBack?.Invoke();
        ActiveStatusChanged?.Invoke(isActive);
    }

    public void ShowPopup(int? ignore, float? prize, Action callBack)
    {
        
        this.callBack = callBack;
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
        audioController.PlayTotalFSPrizePopupSound();
        totalPrizePopUpRT.DOScale(Vector3.one, 0.7f).OnComplete(() =>
        {
            clickableShadow.interactable = true;
        });
    }
}
