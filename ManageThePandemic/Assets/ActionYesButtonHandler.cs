using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


/*
 * Handles what will happen when this button is clicked.
 *
 *      Dependent Action: An action which cannot be taken
 *      before taking this action.
 *
 * It specifically holds the list of dependent actions.
 * Then, when this button is clicked, it publishes message
 * to all dependent actions.
 */
public class ActionYesButtonHandler : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> dependentActions;

    // Publisher-Related
    public EventHandler ButtonClicked;


    public virtual void OnClick()
    {
        Debug.Log("Clicked A.");
        OnButtonClicked();
    }


    /*
     * Publisher-Related method
     */
    protected virtual void OnButtonClicked()
    { 
        if (ButtonClicked != null)
        {
            ButtonClicked(this, EventArgs.Empty);
        }
    }


    /*
     * Subscription-Related method
     */
    public virtual void OnOtherButtonClicked(object source, EventArgs eventArgs)
    {
        Debug.Log("Greetings from Greece." + this.name);
        gameObject.SetActive(true);
    }
}
