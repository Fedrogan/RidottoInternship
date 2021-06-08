using UnityEngine;
using DG.Tweening;
using System;

public class ReelsScroll : MonoBehaviour
{
    public event Action AllReelsStarted;
    public event Action AllReelsStopped;

    [SerializeField] private ReelsSymbolManager reelsSymbolManager;
    [SerializeField] private SymbolsManagement symbolsManager;
    [Space]
    [SerializeField] private RectTransform[] fakeReels;
    [SerializeField] private RectTransform[] subReels;
    [Space]

    [SerializeField] [Range(0, 4000)] private float spinSpeed;
    [SerializeField] private float boostDistance, spinDistance;
    [SerializeField] private float boostDuration, slowdownDuration;
    [SerializeField] private Ease boostEase, slowdownEase;
    [SerializeField] private float delayStep;
    [SerializeField] private int symbolHeigth;
    [SerializeField] private int visibleSymbolsOnReelCount;

    
    private float startSubReelPositionY;
    private readonly float middlePosition = 0;

    private float correctedSlowDownDistance;
    private float startFakeReelPositionY;
    private float traveledDistance;
    private float cellYCorrection;

    private bool isFirstSpin;

    void Start()
    {
        isFirstSpin = true;
        GameController.Instance.SpinStarted += OnSpinStarted;
        GameController.Instance.SpinInterrupted += OnSlowdownSpin;

        startFakeReelPositionY = fakeReels[0].localPosition.y;
        startSubReelPositionY = subReels[0].localPosition.y;
    }

    private void OnSpinStarted()
    {
        StartSpinning();
        isFirstSpin = false;
    }

    private void StartSpinning()
    {
        for (int i = 0; i < fakeReels.Length; i++)
        {
            var delay = i * delayStep;
            var reel = fakeReels[i];
            if (isFirstSpin == false) 
            {
                symbolsManager.SetSymbolsOnReelAlpha(reel, false);
                MoveSubReelOut(reel, delay);
            }
            reel.DOAnchorPosY(boostDistance, boostDuration).SetDelay(delay)
                .SetEase(boostEase).OnComplete(() => LinearSpin(reel));
        }
    }

    private void LinearSpin(RectTransform reel)
    {
        if (reel.GetComponent<ReelInfo>().ReelID == fakeReels.Length) AllReelsStarted?.Invoke();

        reel.DOAnchorPosY(spinDistance, -spinDistance / spinSpeed).SetEase(Ease.Linear)
            .OnComplete(delegate
            {
                SlowdownReelSpin(reel);
                //MoveSubReelIn(reel);
                StopFakeReel(reel, true);
            });
    }  

    private void StopFakeReel(RectTransform reel, bool isStopping)
    {
        reel.GetComponent<ReelInfo>().IsStopping = isStopping;
    }

    public void SlowdownReelSpin(RectTransform reel)
    {
        var currReelPos = reel.localPosition.y;
        symbolsManager.SetSymbolsOnReelAlpha(reel, true);
        symbolsManager.MakeAllSymbolsMutable(true);
        DOTween.Kill(reel);
        var extraDistance = CalculateSlowDownDistance(currReelPos);
        var slowDownDistance = currReelPos - extraDistance;
        var substitutionSpeed = -(slowDownDistance - currReelPos) / slowdownDuration;
        MoveSubReelIn(reel, substitutionSpeed);
        reel.DOAnchorPosY(slowDownDistance, slowdownDuration).SetEase(slowdownEase)
            .OnComplete(delegate
            {
                ResetReelPos(reel);
                if (reel.GetComponent<ReelInfo>().ReelID == fakeReels.Length) AllReelsStopped?.Invoke();
            });
    }

    private void CorrectSubReelPos(float extraDistance, RectTransform reel)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var subReel = subReels[reelID - 1];
        subReel.DOAnchorPosY(extraDistance, 0);
    }

    public void OnSlowdownSpin()
    {
        foreach (RectTransform reel in fakeReels)
        {
            DOTween.Kill(reel);
            SlowdownReelSpin(reel);
            
            //CorrectReelPos(reel);
            StopFakeReel(reel, true);
        }
    }

    private void ResetReelPos(RectTransform reel)
    {
        var reelCurrPos = reel.localPosition;
        if (correctedSlowDownDistance != reelCurrPos.y) cellYCorrection = correctedSlowDownDistance - reelCurrPos.y;
        else cellYCorrection = 0;
        reel.localPosition = new Vector3(reelCurrPos.x, startFakeReelPositionY, reelCurrPos.z);
        StopFakeReel(reel, false);
        symbolsManager.ResetSymbolsPosition(correctedSlowDownDistance, cellYCorrection, startFakeReelPositionY, reel);
    }

    private float CalculateSlowDownDistance(float currReelPos)
    {
        traveledDistance = startFakeReelPositionY - currReelPos;
        var symbolsScrolled = traveledDistance / symbolHeigth;
        var integerPart = Mathf.Floor(symbolsScrolled);
        var fractionalPart = symbolsScrolled - integerPart;
        var extraDistance = symbolHeigth * visibleSymbolsOnReelCount + (1 - fractionalPart) * symbolHeigth;
        print("Extra dist = " + extraDistance);
        var slowDownDistance = currReelPos - extraDistance;
        print("SlowDistance = " + slowDownDistance);

        //return slowDownDistance;
        return extraDistance;
    }

    private void MoveSubReelIn(RectTransform reel, float subSpeed)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var subReel = subReels[reelID - 1];
        var subDistance = startSubReelPositionY - middlePosition;
        print(subSpeed);
        print(subDistance / subSpeed);
        subReel.DOAnchorPosY(middlePosition, subDistance / subSpeed).SetEase(slowdownEase);
    }

    private void MoveSubReelOut(RectTransform reel, float delay)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var subReel = subReels[reelID - 1];
        subReel.DOAnchorPosY(boostDistance, boostDuration)
            .SetEase(boostEase).SetDelay(delay)
            .OnComplete(() => ResetSubReelPos(subReel, reelID));
    }

    private void ResetSubReelPos(RectTransform reel, int reelID)
    {
        var reelCurrPos = reel.localPosition;
        reel.localPosition = new Vector3(reelCurrPos.x, startSubReelPositionY, reelCurrPos.z);
        if (reelID == subReels.Length)
            reelsSymbolManager.FillReels();
    }
}