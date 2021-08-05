using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartFreeSpinsPopup : MonoBehaviour, IPopup
{
    public event Action<bool> ActiveStatusChanged;
    private Action callBack;
    private bool isActive;

    [SerializeField] private CanvasGroup freeSpinsStartPopUp;
    [SerializeField] private RectTransform freeSpinsStartPopUpRT;
    [SerializeField] private TextMeshProUGUI freeSpinsText;
    [SerializeField] private Button startFSButton;
    [SerializeField] private RectTransform startFSButtonRT;
    [SerializeField] private Image shadow;

    [SerializeField] private AudioController audioController;


    private void Start()
    {
        startFSButton.onClick.AddListener(HidePopup);
    }

    public void ShowPopup(int? freeSpins, float? ignore, Action callBack)
    {
        this.callBack = callBack;
        isActive = true;
        ActiveStatusChanged?.Invoke(isActive);
        
        StartCoroutine(CoShowFreeSpinsStartPopUp((int)freeSpins));
    }
    public void HidePopup()
    {
        audioController.PlayHidePopupSound();
        startFSButton.interactable = true;
        startFSButtonRT.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
        {
            freeSpinsStartPopUpRT.DOScale(Vector3.zero, 1);
            shadow.DOFade(0f, 0.1f);
            shadow.raycastTarget = false;
            freeSpinsStartPopUp.alpha = 0;
        });

        isActive = false;
        callBack?.Invoke();
        ActiveStatusChanged?.Invoke(isActive);
    } 

    private IEnumerator CoShowFreeSpinsStartPopUp(int freeSpins)
    {
        shadow.raycastTarget = true;
        yield return new WaitForSecondsRealtime(0.7f);
        freeSpinsText.text = freeSpins.ToString();
        freeSpinsStartPopUp.alpha = 1;
        shadow.DOFade(0.65f, 0.2f);
        audioController.PlayStartFSPopupSound();
        freeSpinsStartPopUpRT.DOScale(Vector3.one, 1).OnComplete(() =>
        {
            startFSButtonRT.DOScale(Vector3.one, 0.2f);
            startFSButton.interactable = true;
        });
    } 
}
