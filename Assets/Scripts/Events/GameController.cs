using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public event Action SpinStarted;
    public event Action SpinInterrupted;
    public event Action ReelsStarted;
    public event Action SpinFinished;


    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;

    [SerializeField] private ReelsScroll reelsScroller;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        playButton.onClick.AddListener(OnPlayButtonClicked);
        stopButton.onClick.AddListener(OnStopButtonClicked);

        stopButton.interactable = false;

        stopButton.GetComponent<RectTransform>().localScale = new Vector3(0, 0);

        reelsScroller.AllReelsStarted += OnAllReelsStarted;
        reelsScroller.AllReelsStopped += OnAllReelsStopped;
        
    }

    private void OnStopButtonClicked()
    {
        stopButton.interactable = false;

        stopButton.GetComponent<RectTransform>().localScale = new Vector3(0, 0);
        playButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1);

        SpinInterrupted?.Invoke();
    }

    private void OnAllReelsStopped()
    {
        playButton.interactable = true;
        stopButton.interactable = false;

        playButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1);

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

        playButton.GetComponent<RectTransform>().localScale = new Vector3(0, 0);
        stopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1);

        SpinStarted?.Invoke();
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        stopButton.onClick.RemoveListener(OnStopButtonClicked);

        reelsScroller.AllReelsStarted -= OnAllReelsStarted;
        reelsScroller.AllReelsStopped -= OnAllReelsStopped;
    }
}
