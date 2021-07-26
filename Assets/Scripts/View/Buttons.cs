using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private RectTransform playButtonRT;

    [SerializeField] private Button stopButton;
    [SerializeField] private RectTransform stopButtonRT;

    [SerializeField] private Button resetAnimButton;
    [SerializeField] private RectTransform resetAnimButtonRT;

    public void ValidateButton(ButtonType button, bool? isInteractable = null, Vector3? scale = null)
    {        
        Button currentButton = null;
        RectTransform currentButtonRT = null;
        switch(button)
        {
            case ButtonType.Play:
                currentButton = playButton;
                currentButtonRT = playButtonRT;
                break;
            case ButtonType.Stop:
                currentButton = stopButton;
                currentButtonRT = stopButtonRT;
                break;
            case ButtonType.Reset:
                currentButton = resetAnimButton;
                currentButtonRT = resetAnimButtonRT;
                break;
        }
        if (isInteractable != null)
        {
            currentButton.interactable = currentButton.interactable == (bool)isInteractable ? currentButton.interactable : (bool)isInteractable;
        }

        if (scale != null)
        {
            currentButtonRT.localScale = currentButtonRT.localScale == (Vector3)scale ? currentButtonRT.localScale : (Vector3)scale;
        }  
    }
}
