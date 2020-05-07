using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Different states of an action:
 *      Action is not enabled because prerequisite actions are not done yet.
 *      Action is enabled but it cannot be taken because budget is not sufficient.
 *      Action is ready to be taken.
 *      Action has duration time and it is in use currently.
 *      Action is one time action and it had taken.
 */
public class ActionUIController : MonoBehaviour
{
    private Button button;


    public void Awake()
    {
        // Warning: It is dependent to the order in the editor.
        button = gameObject.GetComponentInChildren<Button>();
    }


    public void OnDisabled()
    {

    }


    public void OnLowBudget()
    {
        button.interactable = false;
    }


    public void OnReady()
    {

    }


    public void OnUse()
    {

    }


    public void OnDone()
    {

    }
}
