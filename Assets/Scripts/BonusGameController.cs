using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusGameController : MonoBehaviour
{
    public event Action BonusGameStarted;
    public event Action BonusGameFinished;

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

    private Vector3 visibleButtonScale = new Vector3(1, 1);
    private Vector3 invisibleButtonScale = new Vector3(0, 0);

    private bool isFirstFreeSpin = true;

    private float prevWin;
    private float currentWin;

    private int freeSpinsLeft;
    public static BonusGameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        popUpsContainer.BonusGameStartPopUpShown += StartBonusGame;
        popUpsContainer.TotalPrizePopUpShown += FinishBonusGame;
    }

    public void StartBonusGame(int numberOfFreeSpins) 
    {
        BonusGameStarted?.Invoke();
        SubscribeEvents();

        freeSpinsLeft = numberOfFreeSpins; 

        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();
        freeSpinsCounter.alpha = 1;

        resetAnimButton.interactable = false;
        resetAnimButtonRT.localScale = invisibleButtonScale;

        StartNextFreeSpin();

        counterAnimator.ResetCounter();
    }    

    private void FinishBonusGame()
    {
        gameController.BalanceHolder.AddPrize(currentWin);

        UnsubscribeEvents();

        prevWin = 0;
        currentWin = 0;

        stopButton.interactable = false;
        resetAnimButton.interactable = false;
        stopButtonRT.localScale = invisibleButtonScale;         
        resetAnimButtonRT.localScale = invisibleButtonScale;

        freeSpinsCounter.alpha = 0;
        BonusGameFinished?.Invoke();
        gameController.StartOrdinaryGame();
    }

    private void StartNextFreeSpin()
    {
        freeSpinsLeft--;

        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();

        isFirstFreeSpin = false;

        stopButton.interactable = false;
        stopButtonRT.localScale = visibleButtonScale;

        reelsScroller.StartSpinning(false);              
    }


    private void OnAllReelsStopped()
    {
        
        StartCoroutine(CoDoPause());
        
    }
    private IEnumerator CoDoPause()
    {

        stopButton.interactable = false;
        var pause = winLinesChecker.CheckScattersOnReel(reelsScroller.SubReels[2]) > 0 ? 0.3f : 0;
        yield return new WaitForSeconds(pause);
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
        var isForceStop = true;
        stopButton.interactable = false;
        stopButtonRT.localScale = invisibleButtonScale;        
        resetAnimButtonRT.localScale = visibleButtonScale;

        reelsScroller.OnSlowdownSpin(isForceStop);
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
            prevWin = currentWin;
        }

        if (isFirstFreeSpin == false && freeSpinsLeft > 0)
        {
            StartNextFreeSpin();
        }
        else popUpsContainer.ShowTotalPrizePopUp(currentWin);
    }

    private void OnScattersDetected(int scattersDetected)
    {
        //TODO additive free spins popUp
        freeSpinsLeft += gameConfig.ScattersToFreeSpinsMap[scattersDetected];
        freeSpinsLeftCounterText.text = freeSpinsLeft.ToString();
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
