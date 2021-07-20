using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpsContainer : MonoBehaviour
{
    public event Action/*<int>*/ FreeSpinsStartPopUpShown;
    public event Action TotalPrizePopUpShown;

    [SerializeField] private GameConfig gameConfig;

    [SerializeField] private CanvasGroup freeSpinsStartPopUp;
    [SerializeField] private RectTransform freeSpinsStartPopUpRT;
    [SerializeField] private TextMeshProUGUI freeSpinsText;
    [SerializeField] private Button startFSButton;
    [SerializeField] private RectTransform startFSButtonRT;

    [SerializeField] private CanvasGroup totalPrizePopUp;
    [SerializeField] private RectTransform totalPrizePopUpRT;
    [SerializeField] private TextMeshProUGUI totalPrizeText;

    private int numberOfFreeSpins = 0;

    [SerializeField] private Image shadow;
    [SerializeField] private Button clickableShadow;

    public CanvasGroup BonusGameStartPopUp => freeSpinsStartPopUp;

    private void Start()
    {
        startFSButton.onClick.AddListener(OnStartButtonClicked);
        clickableShadow.onClick.AddListener(OnShadowClicked);
    }

    private void OnStartButtonClicked()
    {
        startFSButton.interactable = true;
        startFSButtonRT.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
        {
            freeSpinsStartPopUpRT.DOScale(Vector3.zero, 1);
            shadow.DOFade(0f, 0.1f);
            shadow.raycastTarget = false;
            freeSpinsStartPopUp.alpha = 0;
        });
        FreeSpinsStartPopUpShown?.Invoke();
    }

    public void ShowFreeSpinsStartPopUp(int scattersDetected)
    {
        numberOfFreeSpins = scattersDetected;
        StartCoroutine(CoShowBonusGameStartPopUp());
    }

    private IEnumerator CoShowBonusGameStartPopUp()
    {
        shadow.raycastTarget = true;
        yield return new WaitForSecondsRealtime(0.7f);
        freeSpinsText.text = numberOfFreeSpins.ToString();
        freeSpinsStartPopUp.alpha = 1;
        shadow.DOFade(0.65f, 0.2f);        
        freeSpinsStartPopUpRT.DOScale(Vector3.one, 1).OnComplete(() =>
        {
            startFSButtonRT.DOScale(Vector3.one, 0.2f);
            startFSButton.interactable = true;
        });
    }

    public void ShowTotalPrizePopUp(float totalPrize)
    {
        StartCoroutine(CoShowTotalPrizePopUp(totalPrize));
    }

    private IEnumerator CoShowTotalPrizePopUp(float totalPrize)
    {
        shadow.raycastTarget = true;
        yield return new WaitForSecondsRealtime(0.7f);
        totalPrizeText.text = totalPrize.ToString();
        totalPrizePopUp.alpha = 1;
        shadow.DOFade(0.65f, 0.2f);        
        totalPrizePopUpRT.DOScale(Vector3.one, 0.7f).OnComplete(() =>
        {
            clickableShadow.interactable = true;
        });        
    }

    private void OnShadowClicked()
    {
        clickableShadow.interactable = false;
        shadow.raycastTarget = false;
        shadow.DOFade(0f, 0.1f);
        totalPrizePopUpRT.DOScale(Vector3.zero, 0.7f).OnComplete(() => totalPrizePopUp.alpha = 0);

        TotalPrizePopUpShown?.Invoke();
    }
}
