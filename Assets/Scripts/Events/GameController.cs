﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public event Action<bool> SpinStarted;
    public event Action SpinInterrupted;
    public event Action ReelsStarted;
    public event Action SpinFinished;

    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private PrizeCalculator prizeCalculator;
    [SerializeField] private CounterAnimator counterAnimator;

    [SerializeField] private BalanceHolder balanceHolder;

    [SerializeField] private Button playButton;
    [SerializeField] private RectTransform playButtonRT;
    [SerializeField] private Button stopButton;
    [SerializeField] private RectTransform stopButtonRT;

    private Vector3 visibleButtonScale = new Vector3(1, 1);
    private Vector3 invisibleButtonScale = new Vector3(0, 0);

    private bool isFirstSpin = true;

    private float prevWin;
    private float currentWin;
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        playButton.onClick.AddListener(OnPlayButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);

        stopButton.interactable = false;
        stopButtonRT.localScale = invisibleButtonScale;

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;
    }

    private void OnStopButtonClicked()
    {
        stopButton.interactable = false;

        stopButtonRT.localScale = invisibleButtonScale;
        playButtonRT.localScale = visibleButtonScale;

        reelsScroller.OnSlowdownSpin();
    }

    private void OnAllReelsStopped()
    {
        playButton.interactable = true;
        stopButton.interactable = false;

        playButtonRT.localScale = visibleButtonScale;

        var winningLines = winLinesChecker.GetWinLines();
        animationsManager.StartAnimations(winningLines);
        currentWin = prizeCalculator.CalculateWin(winningLines);
        animationsManager.AllAnimationsFinished += delegate
        {
            counterAnimator.UpdateValue(prevWin, currentWin, false);
        };
    }

    private void OnAllReelsStarted()
    {
        stopButton.interactable = true;
        counterAnimator.ResetCounter();
    }

    private void OnPlayButtonClicked()
    {
        playButton.interactable = false;

        playButtonRT.localScale = invisibleButtonScale;
        stopButtonRT.localScale = visibleButtonScale;
        reelsScroller.StartSpinning(isFirstSpin);
        animationsManager.ResetAnimations(isFirstSpin);
        if (isFirstSpin == false)
        {
            counterAnimator.UpdateValue(prevWin, currentWin, true);
        }
        isFirstSpin = false;

        balanceHolder.AddPrize(currentWin);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        stopButton.onClick.RemoveListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;
    }
}
