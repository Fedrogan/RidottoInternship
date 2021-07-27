using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO в 7м задании добавить звук показа и уборки попапов

public class GameController : MonoBehaviour
{
    public Action<int> FreeSpinsDetected;
    public Action FreeSpinsStarted;
    public Action FreeSpinsFinished;
    public Action OrdinaryStarted;
    public Action NoWinLinesFound;

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
        FreeSpinsDetected += OnFreeSpinsDetected;
        FreeSpinsFinished += OnFreeSpinsFinished;
        OrdinaryStarted += OnOrdinaryStarted;
        SubscribeEvents();
    }

    private void OnOrdinaryStarted()
    {
        buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: true);
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
        popUpsContainer.TotalFSPrizePopup.ShowPopup(prize: gameModel.FreeSpinsWinning, callBack: OrdinaryStarted);

        buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: true, scale: Vector3.one);
    }

    private void OnFreeSpinsDetected(int freeSpinsLeft)
    {
        print("FreeSpinsStarted");
        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.one);

        buttonsContainer.ValidateButton(ButtonType.Play, scale: Vector3.zero);

        popUpsContainer.StartFSPopup.ShowPopup(freeSpins: freeSpinsLeft, callBack: FreeSpinsStarted);
    }

    private void OnLinesChecked(List<Symbol[]> winLines, float currentWin, int scattersDetected)
    {
        print("Lines Checked");
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
        else
        {
            NoWinLinesFound?.Invoke();
        }

        StartCoroutine(CoUpdateGame());

        if (gameModel.GameType == GameType.Ordinary)
        {
            buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.zero);

            buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: true, scale: Vector3.one);
        }
    }

    private IEnumerator CoUpdateGame()
    {
        while (animationsManager.isAnimationsPlaying == true)
        {
            yield return null;
        }
        while (popUpsContainer.isPopupShowing == true)
        {
            yield return null;
        }

        gameModel.UpdateWinnings(currentWin);

        if (gameModel.GameType == GameType.FreeSpins)
        {
            counterAnimator.UpdateValue(gameModel.PrevWinning, gameModel.FreeSpinsWinning, false);
        }
        else
        {
            counterAnimator.UpdateValue(0, gameModel.LastWinning, false);
        }

        gameModel.UpdateGame(scattersDetected, FreeSpinsDetected, FreeSpinsFinished);
        

        if (gameModel.GameType == GameType.FreeSpins)
        {
            StartCoroutine(CoStartNextFreeSpin());
        }

        buttonsContainer.ValidateButton(ButtonType.Stop, isInteractable: false);
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

    private void OnAllReelsStarted()
    {
        buttonsContainer.ValidateButton(ButtonType.Play, scale: Vector3.zero);
        buttonsContainer.ValidateButton(ButtonType.Stop, isInteractable: true, scale: Vector3.one);

        if (gameModel.GameType != GameType.FreeSpins)
        {
            counterAnimator.ResetCounter();
        }        
    }
    private void OnForceStartNextSpin()
    {
        buttonsContainer.ValidateButton(ButtonType.Reset, isInteractable: false, scale: Vector3.zero);

        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.one);

        animationsManager.ResetAnimations();
    }

    private void OnPlayButtonClicked()
    {
        print("PlayClicked");
        buttonsContainer.ValidateButton(ButtonType.Play, isInteractable: false, scale: Vector3.zero);

        buttonsContainer.ValidateButton(ButtonType.Stop, scale: Vector3.one);

        reelsScroller.StartSpinning(isFirstSpin);

        animationsManager.ResetAnimations();

        isFirstSpin = false;
    }

    public void SubscribeEvents()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);
        resetAnimButton.onClick.AddListener(OnForceStartNextSpin);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
    }
}
