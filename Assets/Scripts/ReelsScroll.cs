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
    [SerializeField] private RectTransform[] substitutionReels;
    [Space]

    [SerializeField] [Range(0, 4000)] private float spinSpeed;
    [SerializeField] private float boostDistance, spinDistance;
    [SerializeField] private float boostDuration, slowdownDuration;
    [SerializeField] private Ease boostEase, slowdownEase;
    [SerializeField] private float delayStep;
    [SerializeField] private int symbolHeigth;
    [SerializeField] private int visibleSymbolsOnReelCount;

    
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

        startReelPositionY = fakeReels[0].localPosition.y;
        startFakeReelPositionY = substitutionReels[0].localPosition.y;
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
                MoveSubstitutionReelOut(reel, delay);
            }
            reel.DOAnchorPosY(boostDistance, boostDuration).SetDelay(delay)
                .SetEase(boostEase).OnComplete(() => { LinearSpin(reel); print(reel.anchoredPosition); });
        }
    }

    private void LinearSpin(RectTransform reel)
    {
        if (reel.GetComponent<ReelInfo>().ReelID == fakeReels.Length) AllReelsStarted?.Invoke();

        reel.DOAnchorPosY(spinDistance, -spinDistance / spinSpeed).SetEase(Ease.Linear)
            .OnComplete(delegate
            {
                print(reel.anchoredPosition);
                SlowdownReelSpin(reel);
                MoveSubstitutionReelIn(reel);
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
        print(reel.anchoredPosition.y);
        print(reel.localPosition.y);
        symbolsManager.SetSymbolsOnReelAlpha(reel, true);
        symbolsManager.MakeAllSymbolsMutable(true);
        DOTween.Kill(reel);
        correctedSlowDownDistance = CalculateSlowDownDistance(currReelPos);
        reel.DOAnchorPosY(correctedSlowDownDistance, slowdownDuration).SetEase(slowdownEase)
            .OnComplete(delegate
            {
                ResetReelPos(reel);
                if (reel.GetComponent<ReelInfo>().ReelID == fakeReels.Length) AllReelsStopped?.Invoke();
            });
    }

    public void OnSlowdownSpin()
    {
        DOTween.KillAll();
        foreach (RectTransform reel in fakeReels)
        {
            SlowdownReelSpin(reel);
            MoveSubstitutionReelIn(reel);
            //CorrectReelPos(reel);
            StopFakeReel(reel, true);
        }
    }

    private void ResetReelPos(RectTransform reel)
    {
        var reelCurrPos = reel.localPosition;
        if (correctedSlowDownDistance != reelCurrPos.y) cellYCorrection = correctedSlowDownDistance - reelCurrPos.y;
        else cellYCorrection = 0;
        reel.localPosition = new Vector3(reelCurrPos.x, startReelPositionY, reelCurrPos.z);
        StopFakeReel(reel, false);
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
        print(slowDownDistance);
        return slowDownDistance;
    }

    private void MoveSubstitutionReelIn(RectTransform reel)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var fakeReel = substitutionReels[reelID - 1];
        fakeReel.DOAnchorPosY(middlePosition, slowdownDuration).SetEase(slowdownEase);
    }

    private void MoveSubstitutionReelOut(RectTransform reel, float delay)
    {
        var reelID = reel.GetComponent<ReelInfo>().ReelID;
        var substitutionReel = substitutionReels[reelID - 1];
        substitutionReel.DOAnchorPosY(boostDistance, boostDuration).SetEase(boostEase).SetDelay(delay).OnComplete(() => ResetSubstitutionReelPos(substitutionReel, reelID));
    }

    private void ResetSubstitutionReelPos(RectTransform reel, int reelID)
    {
        var reelCurrPos = reel.localPosition;
        reel.localPosition = new Vector3(reelCurrPos.x, startFakeReelPositionY, reelCurrPos.z);
        if (reelID == 3)
            reelsSymbolManager.FillReels();
    }
}