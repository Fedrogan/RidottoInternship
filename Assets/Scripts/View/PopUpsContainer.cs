using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpsContainer : MonoBehaviour
{
    [SerializeField] private StartFreeSpinsPopup startFSPopup;
    [SerializeField] private TotalFSPrizePopup totalFSPrizePopup;    

    public bool isPopupShowing = false;

    public IPopup StartFSPopup => startFSPopup;
    public IPopup TotalFSPrizePopup => totalFSPrizePopup;

    private void Start()
    {
        startFSPopup.ActiveStatusChanged += SetPopupShowingBool;
        totalFSPrizePopup.ActiveStatusChanged += SetPopupShowingBool;
    }

    private void SetPopupShowingBool(bool isActive)
    {
        isPopupShowing = isActive;
    }    
}
