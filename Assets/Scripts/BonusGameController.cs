using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusGameController : MonoBehaviour
{
    public event Action<bool> SpinStarted;
    public event Action SpinInterrupted;
    public event Action ReelsStarted;
    public event Action SpinFinished;

    [SerializeField] private GameController gameController;
    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private PrizeCalculator prizeCalculator;
    [SerializeField] private CounterAnimator counterAnimator;

    [SerializeField] private GameConfig gameConfig;

    [SerializeField] private BalanceHolder balanceHolder;

    [SerializeField] private Button stopButton;
    [SerializeField] private RectTransform stopButtonRT;
    [SerializeField] private Button resetAnimButton;
    [SerializeField] private RectTransform resetAnimButtonRT;

    private Vector3 visibleButtonScale = new Vector3(1, 1);
    private Vector3 invisibleButtonScale = new Vector3(0, 0);

    private bool isFirstFreeSpin = true;

    private float prevWin;
    private float currentWin;
    private float bonusGameWin;

    private int freeSpinsLeft;
    public static BonusGameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void StartBonusGame(int scattersDetected) 
    {
        SubscribeEvents();

        freeSpinsLeft = gameConfig.ScattersToFreeSpinsMap[scattersDetected];       
                     
        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = invisibleButtonScale;

        StartNextFreeSpin();

        counterAnimator.ResetCounter();
    }    

    private void FinishBonusGame()
    {
        bonusGameWin = currentWin;
        gameController.BalanceHolder.AddPrize(bonusGameWin);

        UnsubscribeEvents();

        prevWin = 0;
        currentWin = 0;

        stopButton.interactable = false;
        resetAnimButton.interactable = false;
        stopButtonRT.localScale = invisibleButtonScale;         
        resetAnimButtonRT.localScale = invisibleButtonScale;

        gameController.StartOrdinaryGame();
    }

    private void StartNextFreeSpin()
    {
        freeSpinsLeft--;
        isFirstFreeSpin = false;
        stopButton.interactable = false;
        stopButtonRT.localScale = visibleButtonScale;

        reelsScroller.StartSpinning(false);              
    }


    private void OnAllReelsStopped()
    {
        stopButton.interactable = false;
        resetAnimButton.interactable = true;
        stopButtonRT.localScale = invisibleButtonScale;
        resetAnimButtonRT.localScale = visibleButtonScale;        

        var winningLines = winLinesChecker.GetWinLines();

        animationsManager.StartAnimations(winningLines);

        prevWin = currentWin;
        currentWin += prizeCalculator.CalculateWin(winningLines);

        winLinesChecker.CheckFSGame();
    }

    private void OnAllReelsStarted()
    {
        stopButton.interactable = true;
        stopButtonRT.localScale = visibleButtonScale;               
    }
        
    private void OnStopReels()
    {
        stopButton.interactable = false;
        stopButtonRT.localScale = invisibleButtonScale;        
        resetAnimButtonRT.localScale = visibleButtonScale;

        reelsScroller.OnSlowdownSpin();
    }

    private void OnForceStartNextSpin()
    {
        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = invisibleButtonScale;
        stopButtonRT.localScale = visibleButtonScale;

        animationsManager.ResetAnimations();
    }

    private void OnAnimationsFinished()
    {
        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = invisibleButtonScale;

        if (isFirstFreeSpin == false)
        {
            counterAnimator.UpdateValue(prevWin, currentWin, false);
        }

        if (isFirstFreeSpin == false && freeSpinsLeft > 0)
        {            
            StartNextFreeSpin();
        }
        else FinishBonusGame();
    }

    private void OnScattersDetected(int scattersDetected)
    {
        freeSpinsLeft += gameConfig.ScattersToFreeSpinsMap[scattersDetected];
        //currentWin += 10000;
    }

    private void SubscribeEvents()
    {
        winLinesChecker.FreeSpinsDetected += OnScattersDetected;

        stopButton.onClick.AddListener(OnStopReels);
        resetAnimButton.onClick.AddListener(OnForceStartNextSpin);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;

        animationsManager.AllAnimationsFinished += OnAnimationsFinished;
    }


    private void UnsubscribeEvents()
    {
        winLinesChecker.FreeSpinsDetected -= OnScattersDetected;

        stopButton.onClick.RemoveListener(OnStopReels);
        resetAnimButton.onClick.RemoveListener(OnForceStartNextSpin);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;

        animationsManager.AllAnimationsFinished -= OnAnimationsFinished;
    }    
}
