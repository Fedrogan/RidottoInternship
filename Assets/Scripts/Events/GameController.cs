using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private BonusGameController bonusGameController;
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

    private Vector3 visibleButtonScale = Vector3.one;
    private Vector3 invisibleButtonScale = Vector3.zero;

    private bool isFirstSpin = true;

    private float prevWin;
    private float currentWin;

    public static GameController Instance { get; private set; }
    public GameType GameType { get => gameType; set => gameType = value; }
    public BalanceHolder BalanceHolder => balanceHolder;

    private void Awake()
    {
        Instance = this;

        stopButton.interactable = false;
        stopButtonRT.localScale = invisibleButtonScale;
    }
    private void Start()
    {
        StartOrdinaryGame();
    }

    public void StartOrdinaryGame()
    {
        SubscribeEvents();

        gameType = GameType.Ordinary;

        playButton.interactable = true;
        playButtonRT.localScale = visibleButtonScale;
    }

    private void OnAnimationsFinished()
    {
        counterAnimator.UpdateValue(prevWin, currentWin, false);
    }

    private void OnStopButtonClicked()
    {
        stopButton.interactable = false;

        stopButtonRT.localScale = invisibleButtonScale;
        playButtonRT.localScale = visibleButtonScale;
        var isForceStop = true;
        reelsScroller.OnSlowdownSpin(isForceStop);
    }

    private void OnAllReelsStopped()
    {             
        StartCoroutine(CoDoPause());        
    }

    private IEnumerator CoDoPause()
    {
        stopButton.interactable = false;
        
        yield return new WaitForSeconds(0.3f);

        var winningLines = winLinesChecker.GetWinLines();

        animationsManager.StartAnimations(winningLines);

        currentWin = prizeCalculator.CalculateWin(winningLines);

        playButton.interactable = true;
        playButtonRT.localScale = visibleButtonScale;

        winLinesChecker.CheckFSGame();

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

        animationsManager.ResetAnimations();

        if (isFirstSpin == false)
        {
            counterAnimator.UpdateValue(prevWin, currentWin, true);
        }
        isFirstSpin = false;

        BalanceHolder.AddPrize(currentWin);
    }

    private void OnFreeSpinsDetected(int scattersDetected)
    {
        UnsubscribeEvents();

        playButton.interactable = false;
        stopButton.interactable = false;

        playButtonRT.localScale = invisibleButtonScale;
        stopButtonRT.localScale = invisibleButtonScale;

        animationsManager.ResetAnimations();

        gameType = GameType.Bonus;

        popUpsContainer.ShowBonusGameStartPopUp(scattersDetected);
    }

    private void UnsubscribeEvents()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        stopButton.onClick.RemoveListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;

        animationsManager.AllAnimationsFinished -= OnAnimationsFinished;

        winLinesChecker.FreeSpinsDetected -= OnFreeSpinsDetected;
    }

    public void SubscribeEvents()
    { 
        playButton.onClick.AddListener(OnPlayButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;

        animationsManager.AllAnimationsFinished += OnAnimationsFinished;

        winLinesChecker.FreeSpinsDetected += OnFreeSpinsDetected;
    }
}
