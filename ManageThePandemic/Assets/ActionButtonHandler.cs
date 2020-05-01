using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

//TODO: En son aksyion ikonuna tikladigimda ve o ikon acildiktan \
// sonra yaptigim hamleleri organize etmeye calsiiyorduk. Bu class
// diger iki class'in base classi olursa duzgun bir yapi oalcak gibi.
// CountryController'in icine publisher ve subscriber olacak.


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
public class ActionButtonHandler : MonoBehaviour
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
    }
}
