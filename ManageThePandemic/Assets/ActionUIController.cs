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
    private Button button;


    public void Awake()
    {
        // WARNING: It is dependent to the order in the editor.
        button = gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            Debug.Log("nerede bu buton? burada");
        }
    }

    //TODO: You're here. OnPassive is called before Awake.


    public void OnPassive()
    {
        Debug.Log("GELDIM YOKTUNUZ.");
        button.interactable = false;
    }


    public void OnReady()
    {
        button.interactable = true;
    }

    public void OnLowBudget()
    {
        button.interactable = false;
    }


    public void OnUse()
    {

    }


    public void OnCompleted()
    {
        button.interactable = false;
    }
}
