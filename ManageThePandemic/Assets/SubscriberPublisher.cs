using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
public class SubscriberPublisher : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> dependentActions;
    
    // Publisher-Related
    public EventHandler<ActionDataArgs> ButtonClicked;

    private ActionDataHolder actionDataHolder;
    private ActionData actionData;

    public void Awake()
    {
        actionDataHolder = GetComponent<ActionDataHolder>();
        actionData = actionDataHolder.actionData;
    }
    public void OnClick()
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
            ButtonClicked(this, new ActionDataArgs(actionData));
        }
    }


    /*
     * Subscription-Related method
     */
    public void OnOtherButtonClicked(object source, ActionDataArgs actionDataArgs)
    {
        Debug.Log("Greetings from Greece." + this.name +  " " +actionDataArgs.actionData.actionName);
        gameObject.SetActive(true);
    }


    public void OnBudgetChanged(object source, BudgetArgs budgetArgs)
    {
        Debug.Log("Saw the change in the budget." + this.name);
    }
}

public class ActionDataArgs
{
    public ActionData actionData { get; set; }

    public ActionDataArgs(ActionData actionData)
    {
        this.actionData = actionData;
    }
}