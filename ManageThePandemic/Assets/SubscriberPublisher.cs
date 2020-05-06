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
    public EventHandler<ActionDataArgs> actionCompleted;
    public EventHandler<ActionDataArgs> buttonClicked;

    private ActionDataHolder actionDataHolder;
    private ActionData actionData;

    private ActionDataArgs actionDataArgs;


    public void Awake()
    {
        actionDataHolder = GetComponent<ActionDataHolder>();
        actionData = actionDataHolder.actionData;

        actionDataArgs = new ActionDataArgs(this, actionData);
    }


    public void OnClick()
    {
        Debug.Log("Clicked."+this.name);
        OnButtonClicked();
    }


    /*
     * Publisher-Related method
     *
     * In the future, if someone wants to connect animations to actions,
     * the correct place is this method! And it should end the animation
     * when the OnActionCompleted() method is called.
     */
    protected virtual void OnButtonClicked()
    { 
        if (buttonClicked != null)
        {
            buttonClicked(this, actionDataArgs);
        }
    }


    /*
     * Publisher-Related method
     *
     */
    public virtual void OnActionCompleted()
    {
        if (actionCompleted != null)
        {
            actionCompleted(this, actionDataArgs);
        }
    }


    /*
     * Subscription-Related method
     *
     * It is called when the action that we depend on is completed.
     */
    public void OnAnotherActionCompleted(object source, ActionDataArgs actionDataArgs)
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
    public SubscriberPublisher publisher;
    public ActionData actionData { get; set; }

    public ActionDataArgs(SubscriberPublisher publisher,
                          ActionData actionData)
    {
        this.publisher = publisher;
        this.actionData = actionData;
    }
}

