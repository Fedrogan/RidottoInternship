using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpsContainer : MonoBehaviour
{
    [SerializeField] private CanvasGroup bonusGameStartPopUp;
    [SerializeField] private Button startFSButton;
    [SerializeField] private RectTransform startFSButtonRT;

    [SerializeField] private CanvasGroup totalBonusGamePopUp;

    public CanvasGroup BonusGameStartPopUp => bonusGameStartPopUp;

    public CanvasGroup TotalBonusGamePopUp => totalBonusGamePopUp;

    public void ShowBonusGameStartPopUp()
    {
        //bonusGameStartPopUp.alpha
    }
}
