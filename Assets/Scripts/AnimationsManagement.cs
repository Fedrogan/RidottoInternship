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
    public event Action WinLineAnimationShowing;

    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private PrizeCalculator calculator;
    [SerializeField] private ReelsScroll reelsScroll;

    [SerializeField] private SubReel[] subReels;
    [SerializeField] private Image[] reelsBG;

    [SerializeField] private float putForwardTweenDuration;
    [SerializeField] private float pulseTweenDuration;
    [SerializeField] private float backTweenDuration;
    [SerializeField] private float pauseBetweenCoroutines; //putForwardTweenDuration + pulseTweenDuration * pulseLoops + backTweenDuration;  
    [SerializeField] private int pulseLoops;

    [SerializeField] private Vector3 putForwardScale;
    [SerializeField] private Vector3 pulseScale;
    private readonly Vector3 defaultSymboleScale = new Vector3(1, 1, 1);

    private List<Symbol[]> winLinesToShow;
    private List<Symbol> allSymbols;

    private void Awake()
    {
        reelsScroll.ReelStopped += ShineScatters;
        winLinesToShow = new List<Symbol[]>();
        allSymbols = new List<Symbol>();        
    }    

    private void Start()
    {
        foreach(var subReel in subReels)
        {
            var symbols = subReel.VisibleReelSymbols;
            foreach(var symbol in symbols)
            {
                allSymbols.Add(symbol);
            }            
        }
    }

    public void AddWinLineToShowList(Symbol[] winLine)
    {
        var newLine = winLine.Clone() as Symbol[];
        winLinesToShow.Add(newLine);
    }

    public void StartAnimations(List<Symbol[]> winningLines)
    {
        StartCoroutine(CoShowWinLine(winningLines));
        if (winningLines.Count > 0)
        {
            foreach (var reelBG in reelsBG)
            {
                reelBG.color = Color.gray;
            }
        }
    }

    public IEnumerator CoShowWinLine(List<Symbol[]> winLinesToShow)
    {
        foreach (var line in winLinesToShow)
        {
            ShowWinAnimation(line);
            WinLineAnimationShowing?.Invoke();
            yield return new WaitForSecondsRealtime(pauseBetweenCoroutines);
            ResetAllSymbolsAnimations();
        }        
        ResetAnimations();        
    }

    private void ResetReelsBG()
    {
        foreach (var reelBG in reelsBG)
        {
            reelBG.color = Color.white;
        }
    }
    private void ShineScatters(SubReel subReel)
    {
        foreach (var symbol in subReel.VisibleReelSymbols)
        {
            if (symbol.SymbolSO.SymbolType == SymbolType.Scatter)
            {
                symbol.SymbolShiny.Play();
                symbol.SymbolRT.DOScale(new Vector3(1.2f, 1.2f), 0.2f)
                    .OnComplete(() => 
                    {
                        symbol.SymbolRT.DOScale(Vector3.one, 0.2f);
                    });
            }
        }
    }

    public void ShowWinAnimation(Symbol[] winLineSymbols)
    {
        foreach (var symbol in allSymbols)
        {
            if (!winLineSymbols.Contains(symbol))
            {
                symbol.Icon.color = Color.gray;
            }
        }
        foreach (var winningSymbol in winLineSymbols)
        {
            var symbolRT = winningSymbol.SymbolRT;
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
            var symbolRT = symbol.SymbolRT;
            symbolRT.DOKill();
            symbol.ParticleFrame.SetActive(false);
            symbol.ParticleSystem.Stop();
            symbolRT.localScale = defaultSymboleScale;
            symbol.Icon.color = Color.white;
        }
    }

    public void ResetAnimations()
    {        
        ResetReelsBG();

        ResetAllSymbolsAnimations();      

        StopAllCoroutines();

        winLinesToShow.Clear();

        AllAnimationsFinished?.Invoke();
    }    
}
