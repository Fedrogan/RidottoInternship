using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO в 7м задании добавить звук показа и уборки попапов

public class GameController : MonoBehaviour
{
    public Action<int> FreeSpinsStarted;
    public Action FreeSpinsFinished;

    [SerializeField] private GameModel gameModel;
    [SerializeField] private ReelsScroll reelsScroller;

    [SerializeField] private GameLogicController gameLogicController;

    [SerializeField] private FreeSpinsCounter freeSpinsCounter;

    [SerializeField] private AnimationsManagement animationsManager;
    [SerializeField] private CounterAnimator counterAnimator;
    [SerializeField] private PopUpsContainer popUpsContainer;
    [SerializeField] private Buttons buttonsContainer;

    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button resetAnimButton;

    private float currentWin;
    private int scattersDetected;

    private bool isFirstSpin = true;

    private void Awake()
    {
        buttonsContainer.ValidateButton(ButtonType.Stop, isInteractable: false, scale: Vector3.zero);

        buttonsContainer.ValidateButton(ButtonType.Reset, isInteractable: false, scale: Vector3.zero);        
    }
    private void Start()
    {
        gameLogicController.LinesChecked += OnLinesChecked;
        FreeSpinsStarted += OnFreeSpinsStarted;
        FreeSpinsFinished += OnFreeSpinsFinished;
        SubscribeEvents();
    }

    private void OnFreeSpinsFinished()
    {
        freeSpinsCounter.HideCounter();

        StartCoroutine(CoShowFinishPopup());

        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.zero);
        
        buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: false);
    }

    public IEnumerator CoShowFinishPopup()
    {
        while (animationsManager.isAnimationsPlaying == true)
        {
            yield return null;
        }
        popUpsContainer.TotalFSPrizePopup.ShowPopup(prize: gameModel.FreeSpinsWinning);

        buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: true);

        yield return null;
    }

    private void OnFreeSpinsStarted(int freeSpinsLeft)
    {
        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.one);

        buttonsContainer.ValidateButton(ButtonType.Play, scale: Vector3.zero);

        animationsManager.ResetAnimations();

        popUpsContainer.StartFSPopup.ShowPopup(freeSpins: freeSpinsLeft);
    }

    private void OnLinesChecked(List<Symbol[]> winLines, float currentWin, int scattersDetected)
    {
        this.currentWin = currentWin;
        this.scattersDetected = scattersDetected;

        if (winLines.Count > 0)
        {
            animationsManager.StartAnimations(winLines);
            if (gameModel.GameType == GameType.FreeSpins)
            {
                buttonsContainer.ValidateButton(ButtonType.Reset, isInteractable: true, scale: Vector3.one);
            }
        }
    }

    public IEnumerator CoStartNextFreeSpin()
    {
        while (animationsManager.isAnimationsPlaying == true)
        {
            yield return null;
        }
        while (popUpsContainer.isPopupShowing == true)
        {
            yield return null;
        }
        freeSpinsCounter.ShowCounter();
        freeSpinsCounter.SetFreeSpinsValue(gameModel.FreeSpinsLeft);
        reelsScroller.StartSpinning(isFirstSpin);
    }

    private void OnAnimationsFinished()
    {
        if (gameModel.GameType == GameType.FreeSpins)
        {
            counterAnimator.UpdateValue(gameModel.PrevWinning, gameModel.FreeSpinsWinning, false);
        }
        else
        {
            counterAnimator.UpdateValue(0, gameModel.LastWinning, false);
        }
    }

    private void OnStopButtonClicked()
    { 
        buttonsContainer.ValidateButton(ButtonType.Stop, isInteractable: false);

        if (gameModel.GameType == GameType.Ordinary)
        {
            buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.zero);
            
            buttonsContainer.ValidateButton(ButtonType.Play, scale: Vector3.one);
        }

        reelsScroller.OnSlowdownSpin(true);
    }

    private void OnAllReelsStopped()
    {
        gameModel.UpdateGame(currentWin, scattersDetected, FreeSpinsStarted, FreeSpinsFinished);

        if (gameModel.GameType == GameType.FreeSpins)
        {
            StartCoroutine(CoStartNextFreeSpin());
        }

        buttonsContainer.ValidateButton(ButtonType.Stop, isInteractable: false);

        if (gameModel.GameType == GameType.Ordinary)
        {
            buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.zero);
            
            buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: true, scale: Vector3.one);
        }
    }

    private void OnAllReelsStarted()
    {        
        buttonsContainer.ValidateButton(ButtonType.Stop, isInteractable: true, scale: Vector3.one);

        counterAnimator.ResetCounter();
    }
    private void OnForceStartNextSpin()
    {
        buttonsContainer.ValidateButton(ButtonType.Reset, isInteractable: false, scale: Vector3.zero);
        
        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.one);

        animationsManager.ResetAnimations();
    }

    private void OnPlayButtonClicked()
    {
        buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: false, scale: Vector3.zero);
        
        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.one);

        reelsScroller.StartSpinning(isFirstSpin);

        animationsManager.ResetAnimations();

        if (isFirstSpin == false)
        {
            counterAnimator.UpdateValue(0, gameModel.LastWinning, true);
        }
        isFirstSpin = false;
    }

    public void SubscribeEvents()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);
        resetAnimButton.onClick.AddListener(OnForceStartNextSpin);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;

        animationsManager.AllAnimationsFinished += OnAnimationsFinished;
    }
}
