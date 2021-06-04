using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;

public class AnimationsManagement : MonoBehaviour
{
    public event Action AllAnimationsFinished;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private SymbolsManagement symbolsManager;
    [SerializeField] private WinningAmountCalculation calculator;

    [SerializeField] private Image[] reelsBG;

    [SerializeField] private float putForwardTweenDuration;
    [SerializeField] private float pulseTweenDuration;
    [SerializeField] private float backTweenDuration;
    [SerializeField] private float pauseBetweenCoroutines; //putForwardTweenDuration + pulseTweenDuration * pulseLoops + backTweenDuration;  
    [SerializeField] private int pulseLoops;

    [SerializeField] private Vector3 putForwardScale;
    [SerializeField] private Vector3 pulseScale;
    private readonly Vector3 defaultSymboleScale = new Vector3(1, 1, 1);

    private List<SlotSymbol[]> winLinesToShow;
    private SlotSymbol[] allSymbols;

    private void Awake()
    {
        winLinesToShow = new List<SlotSymbol[]>();
        allSymbols = symbolsManager.GetAllSymbols();
    }
    private void Start()
    {
        GameController.Instance.SpinStarted += ResetAnimations;
    }

    public void AddWinLineToShowList(SlotSymbol[] winLine)
    {
        print("WinLine Added");
        var newLine = winLine.Clone() as SlotSymbol[];
        winLinesToShow.Add(newLine);
    }

    public void StartAnimations()
    {
        StartCoroutine(CoShowWinLine());
        if (winLinesToShow.Count > 0)
        {
            foreach (var reelBG in reelsBG)
            {
                reelBG.color = Color.gray;
            }
        }
    }

    public IEnumerator CoShowWinLine()
    {
        foreach (var line in winLinesToShow)
        {
            ShowWinAnimation(line);
            yield return new WaitForSecondsRealtime(pauseBetweenCoroutines);
            ResetAllSymbolsAnimations();
        }        
        ResetAnimations();
        AllAnimationsFinished?.Invoke();
    }

    private void ResetReelsBG()
    {
        foreach (var reelBG in reelsBG)
        {
            reelBG.color = Color.white;
        }
    }

    public void ShowWinAnimation(SlotSymbol[] winLineSymbols)
    {
        foreach (var symbol in allSymbols)
        {
            if (!winLineSymbols.Contains(symbol))
            {
                symbol.GetComponent<Image>().color = Color.gray;
            }
        }
        foreach (var winningSymbol in winLineSymbols)
        {
            var symbolRT = winningSymbol.GetComponent<RectTransform>();
            winningSymbol.ParticleFrame.SetActive(true);
            winningSymbol.ParticleSystem.Play();
            var putForwardTweener = symbolRT.DOScale(putForwardScale, putForwardTweenDuration).OnComplete(() =>
            {
                var pulseTweener = symbolRT.DOScale(pulseScale, pulseTweenDuration).SetLoops(pulseLoops, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    var backTweener = symbolRT.DOScale(defaultSymboleScale, backTweenDuration);
                });
            });
        }
    }

    public void ResetAllSymbolsAnimations()
    {
        foreach (var symbol in allSymbols)
        {
            var symbolRT = symbol.GetComponent<RectTransform>();
            symbolRT.DOKill();
            symbol.ParticleFrame.SetActive(false);
            symbol.ParticleSystem.Stop();
            symbolRT.localScale = defaultSymboleScale;
            symbol.GetComponent<Image>().color = Color.white;
        }
    }

    public void ResetAnimations()
    {            
        ResetReelsBG();

        ResetAllSymbolsAnimations();

        calculator.CalculateWin(winLinesToShow);        

        StopAllCoroutines();

        winLinesToShow.Clear();
    }
}
