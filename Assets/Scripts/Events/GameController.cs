using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public event Action<bool> SpinStarted;
    public event Action SpinInterrupted;
    public event Action ReelsStarted;
    public event Action SpinFinished;

    [SerializeField] private ReelsScroll reelsScroller;

    [SerializeField] private Button playButton;
    [SerializeField] private RectTransform playButtonRT;
    [SerializeField] private Button stopButton;
    [SerializeField] private RectTransform stopButtonRT;

    private Vector3 visibleButtonScale = new Vector3(1, 1);
    private Vector3 invisibleButtonScale = new Vector3(0, 0);

    private bool isFirstSpin = true;
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

        SpinInterrupted?.Invoke();
    }

    private void OnAllReelsStopped()
    {
        playButton.interactable = true;
        stopButton.interactable = false;

        playButtonRT.localScale = visibleButtonScale;

        SpinFinished?.Invoke();
    }

    private void OnAllReelsStarted()
    {
        stopButton.interactable = true;

        ReelsStarted?.Invoke();
    }

    private void OnPlayButtonClicked()
    {
        playButton.interactable = false;

        playButtonRT.localScale = invisibleButtonScale;
        stopButtonRT.localScale = visibleButtonScale;

        SpinStarted?.Invoke(isFirstSpin);
        isFirstSpin = false;
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        stopButton.onClick.RemoveListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;
    }
}
