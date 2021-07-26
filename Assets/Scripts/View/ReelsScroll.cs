using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class ReelsScroll : MonoBehaviour
{
    public event Action AllReelsStarted;
    public event Action AllReelsStopped;
    public event Action<SubReel> ReelStopped;
    public event Action SpinStarted;

    public event Action Anticipation;

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private AnticipationAnimation anticipationAnimation;
    [Space]
    [SerializeField] private RectTransform[] fakeReelsRT;
    [SerializeField] private FakeReel[] fakeReels;
    [Space]
    [SerializeField] private RectTransform[] subReelsRT;
    [SerializeField] private SubReel[] subReels;

    private Dictionary<RectTransform, FakeReel> fakeDictionary;
    private Dictionary<RectTransform, SubReel> subDictionary;
    private Dictionary<RectTransform, RectTransform> fakeSubConnection;
    [Space]
    [SerializeField] private Ease boostEase, slowdownEase;
    [SerializeField] private float delayStep;
    [SerializeField] private int symbolHeigth;

    private bool _isForceStop = false;

    [SerializeField] [Range(2000, 4000)] private float reelLinearSpeed;
    [SerializeField] private float boostDistance;
    [SerializeField] private float boostDuration, spinDuration, slowdownDuration;
    private float spinDistance;

    [SerializeField] [Range(4000, 10000)] private float reelAnticipationSpeed;
    [SerializeField] private float anticipationDuration;
    private float anticipationDistance;
    [SerializeField] private RectTransform anticipationReel;

    private float startSubReelPositionY;
    private readonly float middlePosition = 0;
    private float startFakeReelPositionY;

    private float traveledDistance;

    public SubReel[] SubReels => subReels;

    private void Awake()
    {
        fakeDictionary = new Dictionary<RectTransform, FakeReel>();
        subDictionary = new Dictionary<RectTransform, SubReel>();
        fakeSubConnection = new Dictionary<RectTransform, RectTransform>();
        for (int i = 0; i < fakeReels.Length; i++)
        {
            fakeDictionary.Add(fakeReelsRT[i], fakeReels[i]);
            subDictionary.Add(subReelsRT[i], SubReels[i]);
            fakeSubConnection.Add(fakeReelsRT[i], subReelsRT[i]);
        }
        spinDistance = -spinDuration * reelLinearSpeed;
        anticipationDistance = -anticipationDuration * reelAnticipationSpeed;
        startFakeReelPositionY = fakeReelsRT[0].localPosition.y;
        startSubReelPositionY = subReelsRT[0].localPosition.y;
    }    

    private void Update()
    {
        for (int i = 0; i < subReelsRT.Length; i++)
        {
            if (SubReels[i].ReelState == ReelState.Stopping)
            {
                if (subReelsRT[i].localPosition.y <= startSubReelPositionY + symbolHeigth / 2)
                    fakeReels[i].HideSymbolsOnFakeReel(true);
            }
            else
            {
                fakeReels[i].HideSymbolsOnFakeReel(false);
            }
        }
    }

    public void OnSlowdownSpin(bool isForceStop)
    {
        _isForceStop = isForceStop;
        foreach (RectTransform fakeReelRT in fakeReelsRT)
        {
            if (fakeDictionary[fakeReelRT].ReelState == ReelState.Spin)
            {
                DOTween.Kill(fakeReelRT);
                SlowdownFakeReel(fakeReelRT);
            }
        }
    }

    public void StartSpinning(bool isFirstSpin)
    {
        for (int i = 0; i < fakeReels.Length; i++)
        {
            
            var delay = i * delayStep;
            var fakeReelRT = fakeReelsRT[i];            

            fakeDictionary[fakeReelRT].ReelState = ReelState.Spin;
            if (isFirstSpin == false)
            {
                MoveSubReelOut(fakeSubConnection[fakeReelRT], delay);
            }
            fakeReelRT.DOAnchorPosY(boostDistance, boostDuration).SetDelay(delay)
                .OnStart(() => SpinStarted?.Invoke())
                .SetEase(boostEase)
                .OnComplete(() =>
                {
                    LinearSpin(fakeReelRT);
                });
        }
    }

    private void LinearSpin(RectTransform fakeReelRT)
    {
        if (fakeDictionary[fakeReelRT].ReelID == fakeReels.Length) AllReelsStarted?.Invoke();

        fakeReelRT.DOAnchorPosY(spinDistance, spinDuration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SlowdownFakeReel(fakeReelRT);
            });
    }

    public void AnticipationSpin()
    {
        anticipationReel.DOKill();
        anticipationReel.DOAnchorPosY(anticipationDistance, anticipationDuration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SlowdownFakeReel(anticipationReel);
            });
    }

    private void SlowdownFakeReel(RectTransform fakeReelRT)
    {
<<<<<<< HEAD:Assets/Scripts/ReelsScroll.cs
        
=======
        //anticipationAnimation.Deactivate();
>>>>>>> 9a736a84cc41ff1bd4adccf53607a50af66d772d:Assets/Scripts/View/ReelsScroll.cs
        fakeDictionary[fakeReelRT].ReelState = ReelState.Stopping;
        var currentFakeReelPos = fakeReelRT.localPosition.y;
        DOTween.Kill(fakeReelRT);
        var extraDistance = CalculateSlowDownDistance(currentFakeReelPos);
        var slowDownDistance = currentFakeReelPos - extraDistance;

        CorrectSubReel(fakeSubConnection[fakeReelRT], extraDistance);
        fakeReelRT.DOAnchorPosY(slowDownDistance, slowdownDuration).SetEase(slowdownEase)
            .OnComplete(() =>
            {
                anticipationAnimation.Deactivate();
                fakeDictionary[fakeReelRT].ReelState = ReelState.Stop;
                PrepareFakeReel(fakeReelRT);
                if (fakeDictionary[fakeReelRT].ReelID == fakeReels.Length - 1 && _isForceStop == false)
                {
                    TryToStartAnticipation();
                }
                if (fakeDictionary[fakeReelRT].ReelID == fakeReels.Length)
                {
                    anticipationAnimation.Deactivate();
                    _isForceStop = false;
                    AllReelsStopped?.Invoke();
                }
            });
        MoveSubReelIn(fakeSubConnection[fakeReelRT]);

        
    }

    private void PrepareFakeReel(RectTransform fakeReelRT)
    {
        var currentFakeReelPos = fakeReelRT.localPosition;
         var cellYCorrection = -currentFakeReelPos.y;
        fakeReelRT.localPosition = new Vector3(currentFakeReelPos.x, startFakeReelPositionY, currentFakeReelPos.z);
        fakeDictionary[fakeReelRT].ResetSymbolsPosition(cellYCorrection, startFakeReelPositionY);
        fakeDictionary[fakeReelRT].MakeAllSymbolsTransparent();
    }

    private float CalculateSlowDownDistance(float currentFakeReelPos)
    {
        traveledDistance = startFakeReelPositionY - currentFakeReelPos;
        var symbolsScrolled = traveledDistance / symbolHeigth;
        var integerPart = Mathf.Floor(symbolsScrolled);
        var fractionalPart = symbolsScrolled - integerPart;
        var extraDistance = symbolHeigth * gameConfig.VisibleSymbolsOnReel + (1 - fractionalPart) * symbolHeigth;
        return extraDistance;
    }

    public void CorrectSubReel(RectTransform subReelRT, float offset)
    {
        var subReel = subDictionary[subReelRT];
        subReel.ReelState = ReelState.Stopping;
        Vector2 position = subReelRT.anchoredPosition;
        position.y = offset;
        subReelRT.anchoredPosition = position;
    }

    private void MoveSubReelIn(RectTransform subReelRT)
    {
        subReelRT.DOAnchorPosY(middlePosition, slowdownDuration)
            .SetEase(slowdownEase)
            .OnComplete(() =>
            {
                ReelStopped?.Invoke(subDictionary[subReelRT]);
            });
    }

    private void TryToStartAnticipation()
    {
        if (winLinesChecker.CheckAnticipation(SubReels[0], SubReels[1]) == true)
        {
            print("Anticipation");
            anticipationAnimation.Activate();
            Anticipation?.Invoke();
            AnticipationSpin();
        }
    }

    private void MoveSubReelOut(RectTransform subReelRT, float delay)
    {
        var subReel = subDictionary[subReelRT];
        subReel.ReelState = ReelState.Spin;
        subReelRT.DOAnchorPosY(boostDistance, boostDuration)
            .SetEase(boostEase).SetDelay(delay)
            .OnComplete(() =>
            {
                PrepareSubReel(subReelRT, subReel.ReelID);
            });
    }

    private void PrepareSubReel(RectTransform subReelRT, int reelID)
    {
        var reelCurrPos = subReelRT.localPosition;
        subReelRT.localPosition = new Vector3(reelCurrPos.x, startSubReelPositionY, reelCurrPos.z);
        if (reelID == SubReels.Length)
        {
            foreach (var subReel in SubReels)
            {
                subReel.FillReel();
            }
        }
    }
}