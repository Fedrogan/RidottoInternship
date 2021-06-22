using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpsContainer : MonoBehaviour
{
    public event Action<int> BonusGameStartPopUpShown;
    public event Action TotalPrizePopUpShown;

    [SerializeField] private GameConfig gameConfig;

    [SerializeField] private CanvasGroup bonusGameStartPopUp;
    [SerializeField] private RectTransform bonusGameStartPopUpRT;
    [SerializeField] private TextMeshProUGUI bonusGameText;
    [SerializeField] private Button startFSButton;
    [SerializeField] private RectTransform startFSButtonRT;

    [SerializeField] private CanvasGroup totalPrizePopUp;
    [SerializeField] private RectTransform totalPrizePopUpRT;
    [SerializeField] private TextMeshProUGUI totalPrizeText;

    private int _numberOfFreeSpins = 0;

    [SerializeField] private Image shadow;
    [SerializeField] private Button clickableShadow;

    private Vector3 visibleScale = new Vector3(1, 1);
    private Vector3 invisibleScale = new Vector3(0, 0);

    public CanvasGroup BonusGameStartPopUp => bonusGameStartPopUp;

    private void Start()
    {
        startFSButton.onClick.AddListener(OnStartButtonClicked);
        clickableShadow.onClick.AddListener(OnShadowClicked);
    }

    private void OnStartButtonClicked()
    {
        startFSButton.interactable = true;
        startFSButtonRT.DOScale(invisibleScale, 0.1f).OnComplete(() =>
        {
            bonusGameStartPopUpRT.DOScale(invisibleScale, 1);
            shadow.DOFade(0f, 0.1f);
            shadow.raycastTarget = false;
            bonusGameStartPopUp.alpha = 0;
        });
        BonusGameStartPopUpShown?.Invoke(_numberOfFreeSpins);
    }

    public void ShowBonusGameStartPopUp(int scattersDetected)
    {
        _numberOfFreeSpins = gameConfig.ScattersToFreeSpinsMap[scattersDetected];
        StartCoroutine(CoShowBonusGameStartPopUp());
    }

    private IEnumerator CoShowBonusGameStartPopUp()
    {
        yield return new WaitForSecondsRealtime(0.7f);
        bonusGameText.text = _numberOfFreeSpins.ToString();
        bonusGameStartPopUp.alpha = 1;
        shadow.DOFade(0.65f, 0.2f);
        shadow.raycastTarget = true;
        bonusGameStartPopUpRT.DOScale(visibleScale, 1).OnComplete(() =>
        {
            startFSButtonRT.DOScale(visibleScale, 0.2f);
            startFSButton.interactable = true;
        });
    }

    public void ShowTotalPrizePopUp(float totalPrize)
    {
        StartCoroutine(CoShowTotalPrizePopUp(totalPrize));
    }

    private IEnumerator CoShowTotalPrizePopUp(float totalPrize)
    {
        yield return new WaitForSecondsRealtime(0.7f);
        totalPrizeText.text = totalPrize.ToString();
        totalPrizePopUp.alpha = 1;
        shadow.DOFade(0.65f, 0.2f);
        shadow.raycastTarget = true;
        totalPrizePopUpRT.DOScale(visibleScale, 0.7f).OnComplete(() =>
        {
            clickableShadow.interactable = true;
        });        
    }

    private void OnShadowClicked()
    {
        clickableShadow.interactable = false;
        shadow.raycastTarget = false;
        shadow.DOFade(0f, 0.1f);
        totalPrizePopUpRT.DOScale(invisibleScale, 0.7f).OnComplete(() => totalPrizePopUp.alpha = 0);

        TotalPrizePopUpShown?.Invoke();
    }
}
