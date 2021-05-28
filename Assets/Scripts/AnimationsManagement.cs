using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationsManagement : MonoBehaviour
{
    [SerializeField] private float putForwardTweenDuration;
    [SerializeField] private float pulseTweenDuration;
    [SerializeField] private float backTweenDuration;
    [SerializeField] private WinLinesCheck winLinesChecker;
    [SerializeField] private RectTransform front;


    [SerializeField] private Vector3 scale = new Vector3(1.1f, 1.1f);
    [SerializeField] private Vector3 pulseScale = new Vector3(1.2f, 1.2f);
    private readonly Vector3 defaultSymboleScale = new Vector3(1, 1, 1);

    public void ShowWinAnimation(List<SlotSymbol> winningSymbols)
    {        
        foreach (var winningSymbol in winningSymbols)
        {
            winningSymbol.transform.SetParent(front);
            var symbolRT = winningSymbol.GetComponent<RectTransform>();
            var putForwardTweener = symbolRT.DOScale(scale, putForwardTweenDuration).OnComplete(() =>
            {
                var pulseTweener = symbolRT.DOScale(pulseScale, pulseTweenDuration).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
                {
                    var backTweener = symbolRT.DOScale(defaultSymboleScale, backTweenDuration).OnComplete(() => { winLinesChecker.ResetWinCheck(); });
                });
            });            
        }
    }
}
