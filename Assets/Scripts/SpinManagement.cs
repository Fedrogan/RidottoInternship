using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinManagement : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private List<RectTransform> reels;
    [SerializeField] private List<SymbolPositionAndImageChanger> symbols;
    [SerializeField] private GameManagement gameManager;
    [SerializeField] [Range(0, 10000)] private float spinSpeed;
    [SerializeField] private float boostDistance, spinDistance;
    [SerializeField] private float boostDuration, slowdownDuration;
    [SerializeField] private Ease boostEase, slowdownEase;
    [SerializeField] private float delayStep;
    [SerializeField] private int symbolHeigth;
    [SerializeField] private RectTransform thirdReelParent;

    private float correctedSlowDownDistance;
    private float startReelPositionY;
    private float traveledDistance;    
    private float cellYCorrection;

    //для методов тестирования (автоспина)
    //private int reelsStoppedCount;
    //private bool isSpinStarted;
    #endregion

    void Start()
    {
        startReelPositionY = reels[0].localPosition.y;
    }

    //методы для тестирования (пока для тестирования;))
    //ломает запуск и остановку спинов с кнопок
    //private void Update()
    //{
    //    if (reelsStoppedCount == 3)
    //    {
    //        isSpinStarted = false;
    //    }
    //    if (isSpinStarted == false)
    //    {
    //        AutoSpin();
    //    }
    //}

    //public void AutoSpin()
    //{
    //    reelsStoppedCount = 0;
    //    spinDistance = Random.Range(-10000f, -1000f);
    //    Debug.Log(spinDistance);
    //    gameManager.StartSpin();
    //    print("New Spin Started");
    //}

    public void StartSpinning()
    {
        //isSpinStarted = true; (тестирование)
        for (int i = 0; i < 3; i++)
        {            
            var reel = reels[i];
            reel.DOAnchorPosY(boostDistance, boostDuration).SetDelay(i * delayStep)
                .SetEase(boostEase).OnComplete(() => LinearSpin(reel));
        }
    }

    public void LinearSpin(RectTransform reel)
    {
        if (reel.IsChildOf(thirdReelParent)) gameManager.stopButton.SetActive(true); //закомментить для автоспина (для тестирования)
        print(reel.localPosition.y);
        reel.DOAnchorPosY(spinDistance, -spinDistance / spinSpeed).SetEase(Ease.Linear)
            .OnComplete(delegate
            {
                CorrectReelPos(reel);
                StopReel(reel, true);
            });
    }
        
    private void CorrectReelPos(RectTransform reel)
    {
        /**КОСТЫЛЬ
    * следующий "метод-костыль"(или не костыль... пока не до конца разобрался)
    * избавляет от лишней смены символа,
    * которая происходит из-за неточностей при float арифметике.
    * я пока не совсем понимаю как, но это работает
    */
        traveledDistance = startReelPositionY - reel.localPosition.y;
        var remaingerOfDivision = traveledDistance % symbolHeigth;
        if (0 <= remaingerOfDivision && remaingerOfDivision <= 199.99f)        
        {
            var correction = 199.99f - remaingerOfDivision;
            MakeAllSymbolsMutable(false);
            var target = reel.localPosition.y - correction;
            reel.DOAnchorPosY(target, target / spinSpeed).SetEase(Ease.Linear)
                .OnComplete(() => SlowdownReelSpin(reel));
        }
        else SlowdownReelSpin(reel);                
    }

    private void MakeAllSymbolsMutable(bool isMutable)
    {        
        foreach (SymbolPositionAndImageChanger symbol in symbols)
        {
            symbol.MakeSymbolMutable(isMutable);
        }
    }

    private void StopReel(RectTransform reel, bool isStopping)
    {
        reel.GetComponent<ReelInfo>().isStopping = isStopping;
    }

    public void SlowdownReelSpin(RectTransform reel)
    {
        print("SlowDownPos = " + reel.localPosition.y);
        var currReelPos = reel.localPosition.y;
        //gameManager.SetSlowingDownState(true);
        gameManager.stopButton.SetActive(false);
        MakeAllSymbolsMutable(true);
        DOTween.Kill(reel);
        correctedSlowDownDistance = CalculateSlowDownDistance(currReelPos);       
        reel.DOAnchorPosY(correctedSlowDownDistance, slowdownDuration).SetEase(slowdownEase)
            .OnComplete(delegate
            {
                ReelSetDefaultPos(reel);
            });
    }

    public void SlowdownSpin()
    {
        gameManager.stopButton.SetActive(false);
        DOTween.KillAll();
        foreach (RectTransform reel in reels)
        {
            print("SlowDownPos = " + reel.localPosition.y);
            CorrectReelPos(reel);
            StopReel(reel, true);
        }        
    }

    void ReelSetDefaultPos(RectTransform reel)
    {        
        if (DOTween.TotalPlayingTweens() == 0) gameManager.playButton.SetActive(true);
        var reelCurrPos = reel.localPosition;
        if (correctedSlowDownDistance != reelCurrPos.y) cellYCorrection = correctedSlowDownDistance - reelCurrPos.y;
        else cellYCorrection = 0;
        reel.localPosition = new Vector3(reelCurrPos.x, startReelPositionY, reelCurrPos.z);
        StopReel(reel, false);
        var symbolsCount = reel.childCount;
        for (int i = 0; i < symbolsCount; i++)
        {
            var symbol = reel.GetChild(i);
            var symbolPos = symbol.localPosition;
            var newYPos = Mathf.Round(symbolPos.y + correctedSlowDownDistance - cellYCorrection - startReelPositionY);
            symbol.localPosition =
                new Vector3(symbolPos.x, newYPos, symbolPos.z);
        }        
        //gameManager.SetSlowingDownState(false);
        //reelsStoppedCount++; (тестирование)
    } 
    
    private float CalculateSlowDownDistance(float currReelPos)
    {
        /**
         * Расчет дистанции, необходимой для того, 
         * чтобы все символы обновились.
         * На основании полученной текущей позиции рила рассчитывает,
         * какая часть нижнего символа "выкатилась" с игрового поля,
         * и добавляет оставшуюся часть к коррекции.
         */
        traveledDistance = startReelPositionY - currReelPos;
        var symbolsChanged = traveledDistance / symbolHeigth;        
        var integerPart = Mathf.Floor(symbolsChanged);
        var fractionalPart = symbolsChanged - integerPart;
        var extraDistance = symbolHeigth * 3 + (1 - fractionalPart) * symbolHeigth;
        var slowDownDistance = currReelPos - extraDistance;        
        return slowDownDistance;
    }
}