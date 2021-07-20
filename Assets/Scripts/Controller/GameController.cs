using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameModel gameModel;
    [SerializeField] private FreeSpinsController freeSpinsController;
    [SerializeField] private ReelsScroll reelsScroller;
    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private PrizeCalculator prizeCalculator;
    [SerializeField] private CounterAnimator counterAnimator;

    [SerializeField] private PopUpsContainer popUpsContainer;

    [SerializeField] private BalanceHolder balanceHolder;

    [SerializeField] private Button playButton;
    [SerializeField] private RectTransform playButtonRT;
    [SerializeField] private Button stopButton;
    [SerializeField] private RectTransform stopButtonRT;

    private GameType gameType = GameType.Ordinary;

    private bool isFirstSpin = true;

    private int scattersDetected;

    private float currentWin;

    public static GameController Instance { get; private set; }
    public GameType GameType { get => gameType; set => gameType = value; }
    public BalanceHolder BalanceHolder => balanceHolder;

    private void Awake()
    {
        Instance = this;

        stopButton.interactable = false;
        stopButtonRT.localScale = Vector3.zero;
    }
    private void Start()
    {
        freeSpinsController.FreeSpinsFinished += StartOrdinaryGame;
        StartOrdinaryGame(0);
    }

    public void StartOrdinaryGame(float freeSpinsWin)
    {
        scattersDetected = 0;

        balanceHolder.AddPrize(freeSpinsWin);

        SubscribeEvents();

        gameType = GameType.Ordinary;

        playButton.interactable = true;
        playButtonRT.localScale = Vector3.one;
    }

    private void OnAnimationsFinished()
    {
        counterAnimator.UpdateValue(0, currentWin, false);
    }

 private void OnStopButtonClicked()
    {
        stopButton.interactable = false;

        stopButtonRT.localScale = Vector3.zero;
        playButtonRT.localScale = Vector3.one;

        reelsScroller.OnSlowdownSpin();
    }

    private void OnAllReelsStopped()
    {
        playButton.interactable = true;
        stopButton.interactable = false;

        playButtonRT.localScale = Vector3.one;
        if (scattersDetected > 2)
        {
            PrepareForFreeSpins();
        }
    }

    private void UpdateCurrentWin(float win)
    {
        currentWin = win;
    }

    private void OnAllReelsStarted()
    {
        stopButton.interactable = true;

        counterAnimator.ResetCounter();
    }

    private void OnPlayButtonClicked()
    {
        playButton.interactable = false;

        playButtonRT.localScale = Vector3.zero;
        stopButtonRT.localScale = Vector3.one;

        reelsScroller.StartSpinning(isFirstSpin);

        animationsManager.ResetAnimations();

        if (isFirstSpin == false)
        {
            counterAnimator.UpdateValue(0, currentWin, true);
        }
        isFirstSpin = false;

        BalanceHolder.AddPrize(currentWin);
        currentWin = 0;
    }

    private void PrepareForFreeSpins()
    {
        UnsubscribeEvents();
        playButton.interactable = false;
        stopButton.interactable = false;

        playButtonRT.localScale = Vector3.zero;
        stopButtonRT.localScale = Vector3.zero;

        animationsManager.ResetAnimations();

        gameType = GameType.FreeSpins;

        freeSpinsController.StartFreeSpins(scattersDetected);
    }
    private void OnScattersDetected(int scattersDetected)
    {
        this.scattersDetected = scattersDetected;      
    }

    private void UnsubscribeEvents()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        stopButton.onClick.RemoveListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;

        animationsManager.AllAnimationsFinished -= OnAnimationsFinished;

        winLinesChecker.FreeSpinsDetected -= OnScattersDetected;

        prizeCalculator.PrizeCalculated -= UpdateCurrentWin;
    }

    public void SubscribeEvents()
    { 
        playButton.onClick.AddListener(OnPlayButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;

        animationsManager.AllAnimationsFinished += OnAnimationsFinished;

        winLinesChecker.FreeSpinsDetected += OnScattersDetected;

        prizeCalculator.PrizeCalculated += UpdateCurrentWin;
    }
}
