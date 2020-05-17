using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Different states of an action:
 *      Action is not enabled because prerequisite actions are not done yet.
 *      Action is ready to be taken.
 *      Action is enabled but it cannot be taken because budget is not sufficient.
 *      Action has duration time and it is in use currently.
 *      Action is one time action and it had taken.
 */
public class ActionUIController : MonoBehaviour
{
    [SerializeField]
    private Button infoViewerButton;

    private Image buttonImage;

    public void Start()
    {
        buttonImage = infoViewerButton.GetComponent<Image>();
    }

    public void OnPassive()
    {
        infoViewerButton.interactable = false;
    }


    public void OnReady()
    {
        Color32 readyColor = new Color32(33, 75, 75, 255);
        buttonImage.color = readyColor;
        infoViewerButton.interactable = true;
    }


    public void OnLowBudget()
    {
        Debug.Log("parayi kontrol ettim.");
        infoViewerButton.interactable = false;
    }

    public void OnConstruction()
    {
        Color32 constructionColor = new Color32(16, 24, 255, 255);
        buttonImage.color = constructionColor;
        infoViewerButton.interactable = false;
    }

    public void OnUse()
    {
        Color32 onUseColor = new Color32(135, 231, 245, 255);
        buttonImage.color = onUseColor;
        infoViewerButton.interactable = false;
    }
    
    
    /*
     * Tek seferlik aksiyonlarin construction time bittikten sonra
     * geldikleri state => OnCompleted().
     */
    public void OnCompleted()
    {
        Color32 completedColor = new Color32(255, 3, 0, 255);
        buttonImage.color = completedColor;
        infoViewerButton.interactable = false;
    }
}
