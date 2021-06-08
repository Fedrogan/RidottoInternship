using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class ReelsScroll : MonoBehaviour
{
    public event Action AllReelsStarted;
    public event Action AllReelsStopped;

    [SerializeField] private RectTransform[] reels;
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] [Range(0, 3000)] private float spinSpeed;
    [SerializeField] private float boostDistance, spinDistance;
    [SerializeField] private float boostDuration, slowdownDuration;
    [SerializeField] private Ease boostEase, slowdownEase;
    [SerializeField] private float delayStep;
    [SerializeField] private int symbolHeigth;
    [SerializeField] private RectTransform thirdReelParent;
    [SerializeField] private int visibleSymbolsOnReelCount;

    private float correctedSlowDownDistance;
    private float startReelPositionY;
    private float traveledDistance;    
    private float cellYCorrection;
    
    void Start()
    {
        GameController.Instance.SpinStarted += OnSpinStarted;
        GameController.Instance.SpinInterrupted += OnSlowdownSpin;

        startReelPositionY = reels[0].localPosition.y;
    }

    private void OnSpinStarted()
    {
        StartSpinning();
    }

    public void StartSpinning()
    {
        for (int i = 0; i < reels.Length; i++)
        {            
            var reel = reels[i];            
            reel.DOAnchorPosY(boostDistance, boostDuration).SetDelay(i * delayStep)
                .SetEase(boostEase).OnComplete(() => LinearSpin(reel));
        }
    }

    public void LinearSpin(RectTransform reel)
    {
        if (reel.GetComponent<ReelInfo>().ReelID == reels.Length) AllReelsStarted?.Invoke();

        reel.DOAnchorPosY(spinDistance, -spinDistance / spinSpeed).SetEase(Ease.Linear)
            .OnComplete(delegate
            {
                CorrectReelPos(reel);
                StopReel(reel, true);
            });
    }

    private void CorrectReelPos(RectTransform reel)
    {
        traveledDistance = startReelPositionY - reel.localPosition.y;
        var remaingerOfDivision = traveledDistance % symbolHeigth;
        if (0 <= remaingerOfDivision && remaingerOfDivision <= 199.99f)
        {
            var correction = 199.99f - remaingerOfDivision;
            print(correction / spinSpeed);
            
            symbolsManager.MakeAllSymbolsMutable(false);
            
            var target = reel.localPosition.y - correction;
            print(-target / spinSpeed);
            reel.DOAnchorPosY(target, correction / spinSpeed).SetEase(Ease.Linear)
                .OnComplete(() => SlowdownReelSpin(reel));
        }
        else SlowdownReelSpin(reel);
    }

    private void StopReel(RectTransform reel, bool isStopping)
    {
        reel.GetComponent<ReelInfo>().IsStopping = isStopping;
    }

    public void SlowdownReelSpin(RectTransform reel)
    {
        var currReelPos = reel.localPosition.y;
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
        foreach (RectTransform reel in reels)
        {
            if (reel.GetComponent<ReelInfo>().IsStopping == false)
            {
                DOTween.Kill(reel);
                SlowdownReelSpin(reel);
                StopReel(reel, true);
            }            
        }        
    }

    void ResetReelPos(RectTransform reel)
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
}