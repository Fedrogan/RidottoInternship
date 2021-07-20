using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeSpinsController : MonoBehaviour
{    
    //TODO До конца распределить ответственности по классам
    //TODO Fix popups
    //TODO Разобраться с кнопками

    public event Action FreeSpinsStarted;
    public event Action<float> FreeSpinsFinished;

    [SerializeField] private GameController gameController;
    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private PrizeCalculator prizeCalculator;
    [SerializeField] private CounterAnimator counterAnimator;
    [SerializeField] private PopUpsContainer popUpsContainer;

    [SerializeField] private CanvasGroup freeSpinsCounter;
    [SerializeField] private Text freeSpinsLeftCounterText;

    [SerializeField] private GameConfig gameConfig;

    [SerializeField] private Button stopButton;
    [SerializeField] private RectTransform stopButtonRT;
    [SerializeField] private Button resetAnimButton;
    [SerializeField] private RectTransform resetAnimButtonRT;

    private bool isFirstFreeSpin = true;

    private float prevWin;
    private float currentWin;

    private int freeSpinsLeft;
    public static FreeSpinsController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }    

    public void StartFreeSpins(int scattersDetected) 
    {
        prevWin = 0;
        currentWin = 0;

        FreeSpinsStarted?.Invoke();

        SubscribeEvents();        

        freeSpinsLeft = gameConfig.ScattersToFreeSpinsMap[scattersDetected];

        popUpsContainer.ShowFreeSpinsStartPopUp(freeSpinsLeft);
        popUpsContainer.FreeSpinsStartPopUpShown += StartNextFreeSpin;

        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();        

        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = Vector3.zero;        

        counterAnimator.ResetCounter();
    }    

    private void FinishFreeSpins()
    {
        UnsubscribeEvents();
        popUpsContainer.ShowTotalPrizePopUp(currentWin);        

        stopButton.interactable = false;
        resetAnimButton.interactable = false;
        stopButtonRT.localScale = Vector3.zero;         
        resetAnimButtonRT.localScale = Vector3.zero;

        freeSpinsCounter.alpha = 0;
        FreeSpinsFinished?.Invoke(currentWin);        
    }

    private void StartNextFreeSpin()
    {        
        freeSpinsCounter.alpha = 1;
        freeSpinsLeft--;

        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();

        isFirstFreeSpin = false;

        stopButton.interactable = false;
        stopButtonRT.localScale = Vector3.one;
        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = Vector3.zero;

        reelsScroller.StartSpinning(false);              
    }


    private void OnAllReelsStopped()
    {
        stopButton.interactable = false;
        resetAnimButton.interactable = true;
        stopButtonRT.localScale = Vector3.zero;
    }

    private void OnAllReelsStarted()
    {        
        stopButton.interactable = true;
        stopButtonRT.localScale = Vector3.one;               
    }
        
    private void OnStopReels()
    {
        stopButton.interactable = false;
        stopButtonRT.localScale = Vector3.zero;        
        resetAnimButtonRT.localScale = Vector3.one;

        reelsScroller.OnSlowdownSpin();
    }

    private void OnForceStartNextSpin()
    {
        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = Vector3.zero;
        stopButtonRT.localScale = Vector3.one;

        animationsManager.ResetAnimations();
    }

    private void OnAnimationsFinished()
    {
        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = Vector3.zero;

        if (isFirstFreeSpin == false)
        {
            counterAnimator.UpdateValue(prevWin, currentWin, false);
            prevWin = currentWin;
        }

        if (isFirstFreeSpin == false && freeSpinsLeft > 0)
        {
            StartNextFreeSpin();
        }
        else FinishFreeSpins();
    }

    private void OnScattersDetected(int scattersDetected)
    {
        //TODO additive free spins popUp
        freeSpinsLeft += gameConfig.ScattersToFreeSpinsMap[scattersDetected];
        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();
    }
    private void UpdateCurrentWin(float win)
    {
        prevWin = currentWin;
        currentWin += win;
    }

    private void SubscribeEvents()
    {
        winLinesChecker.FreeSpinsDetected += OnScattersDetected;

        stopButton.onClick.AddListener(OnStopReels);
        resetAnimButton.onClick.AddListener(OnForceStartNextSpin);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;

        animationsManager.AllAnimationsFinished += OnAnimationsFinished;

        prizeCalculator.PrizeCalculated += UpdateCurrentWin;
    }

    private void UnsubscribeEvents()
    {
        winLinesChecker.FreeSpinsDetected -= OnScattersDetected;

        stopButton.onClick.RemoveListener(OnStopReels);
        resetAnimButton.onClick.RemoveListener(OnForceStartNextSpin);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;

        animationsManager.AllAnimationsFinished -= OnAnimationsFinished;
        popUpsContainer.FreeSpinsStartPopUpShown -= StartNextFreeSpin;

        prizeCalculator.PrizeCalculated -= UpdateCurrentWin;
    }    
}
