using UnityEngine;
using DG.Tweening;
using System;

public class ReelsScroll : MonoBehaviour
{
    public event Action AllReelsStarted;
    public event Action AllReelsStopped;

    [SerializeField] private ReelsSymbolManager reelsSymbolManager;

    [SerializeField] private RectTransform[] reels;
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] [Range(0, 10000)] private float spinSpeed;
    [SerializeField] private float boostDistance, spinDistance;
    [SerializeField] private float boostDuration, slowdownDuration;
    [SerializeField] private Ease boostEase, slowdownEase;
    [SerializeField] private float delayStep;
    [SerializeField] private int symbolHeigth;
    [SerializeField] private RectTransform thirdReelParent;
    [SerializeField] private int visibleSymbolsOnReelCount;

    [SerializeField] private RectTransform[] fakeReels;
    private float startFakeReelPositionY;
    private readonly float middlePosition = 0;

    private float correctedSlowDownDistance;
    private float startReelPositionY;
    private float traveledDistance;
    private float cellYCorrection;

    private bool isFirstSpin;

    void Start()
    {
        isFirstSpin = true;
        GameController.Instance.SpinStarted += OnSpinStarted;
        GameController.Instance.SpinInterrupted += OnSlowdownSpin;

        startReelPositionY = reels[0].localPosition.y;
        startFakeReelPositionY = fakeReels[0].localPosition.y;
    }

    private void OnSpinStarted()
    {
        StartSpinning();
        isFirstSpin = false;
    }

    private void StartSpinning()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            var delay = i * delayStep;
            var reel = reels[i];
            if (isFirstSpin == false) 
            {
                symbolsManager.SetSymbolsOnReelAlpha(reel, false);
                MoveFakeReelOut(reel, delay);
            }
            reel.DOAnchorPosY(boostDistance, boostDuration).SetDelay(delay)
                .SetEase(boostEase).OnComplete(() => LinearSpin(reel));
        }
    }

    private void LinearSpin(RectTransform reel)
    {
        if (reel.GetComponent<ReelInfo>().ReelID == reels.Length) AllReelsStarted?.Invoke();

        reel.DOAnchorPosY(spinDistance, -spinDistance / spinSpeed).SetEase(Ease.Linear)
            .OnComplete(delegate
            {
                SlowdownReelSpin(reel);
                MoveFakeReelIn(reel);
                //CorrectReelPos(reel);
                StopReel(reel, true);
            });
    }  

    private void StopReel(RectTransform reel, bool isStopping)
    {
        reel.GetComponent<ReelInfo>().IsStopping = isStopping;
    }

    public void SlowdownReelSpin(RectTransform reel)
    {
        var currReelPos = reel.localPosition.y;
        symbolsManager.SetSymbolsOnReelAlpha(reel, true);
        symbolsManager.MakeAllSymbolsMutable(true);
        DOTween.Kill(reel);
        correctedSlowDownDistance = CalculateSlowDownDistance(currReelPos);
        reel.DOAnchorPosY(correctedSlowDownDistance, slowdownDuration).SetEase(slowdownEase)
            .OnComplete(delegate
            {
                ResetReelPos(reel);
                if (reel.GetComponent<ReelInfo>().ReelID == reels.Length) AllReelsStopped?.Invoke();
            });
    }

    public void OnSlowdownSpin()
    {
        DOTween.KillAll();
        foreach (RectTransform reel in reels)
        {
            SlowdownReelSpin(reel);
            MoveFakeReelIn(reel);
            //CorrectReelPos(reel);
            StopReel(reel, true);
        }
    }

    private void ResetReelPos(RectTransform reel)
    {
        var reelCurrPos = reel.localPosition;
        if (correctedSlowDownDistance != reelCurrPos.y) cellYCorrection = correctedSlowDownDistance - reelCurrPos.y;
        else cellYCorrection = 0;
        reel.localPosition = new Vector3(reelCurrPos.x, startReelPositionY, reelCurrPos.z);
        StopReel(reel, false);
        symbolsManager.ResetSymbolsPosition(correctedSlowDownDistance, cellYCorrection, startReelPositionY, reel);
    }

    private float CalculateSlowDownDistance(float currReelPos)
    {
        traveledDistance = startReelPositionY - currReelPos;
        var symbolsChanged = traveledDistance / symbolHeigth;
        var integerPart = Mathf.Floor(symbolsChanged);
        var fractionalPart = symbolsChanged - integerPart;
        var extraDistance = symbolHeigth * visibleSymbolsOnReelCount + (1 - fractionalPart) * symbolHeigth;
        var slowDownDistance = currReelPos - extraDistance;
        return slowDownDistance;
    }

    private void MoveFakeReelIn(RectTransform reel)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var fakeReel = fakeReels[reelID - 1];
        fakeReel.DOAnchorPosY(middlePosition, slowdownDuration).SetEase(slowdownEase);
    }

    private void MoveFakeReelOut(RectTransform reel, float delay)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var fakeReel = fakeReels[reelID - 1];
        fakeReel.DOAnchorPosY(boostDistance, boostDuration).SetEase(boostEase).SetDelay(delay).OnComplete(() => ResetFakeReelPos(fakeReel, reelID));
    }

    private void ResetFakeReelPos(RectTransform reel, int reelID)
    {
        var reelCurrPos = reel.localPosition;
        reel.localPosition = new Vector3(reelCurrPos.x, startFakeReelPositionY, reelCurrPos.z);
        if (reelID == 3)
            reelsSymbolManager.FillReels();
    }
}