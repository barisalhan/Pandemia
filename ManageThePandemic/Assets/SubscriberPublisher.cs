using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/*
 * 1- Kodda subscription publish olayini duzenle +
 * 2- Dependent actions'un tanimini cok kesin bir sekilde yap. +
 * 3- Editorde var olan seyleri degistir
 * 4- OnClick methodunu degistir.
 * 5- Yusuf'a soyle.
 *
 */

/*
 * Handles what will happen when this button is clicked.
 *
 *      Prerequisite Action: This action cannot be taken
 *      before taking prerequisite action.
 *
 * It holds the prerequisite action. Then, subscribes
 * the related method of that action to be notified
 * when it is completed.
 */
public class SubscriberPublisher : MonoBehaviour
{
    [SerializeField]
    public GameObject prerequisiteAction = null;

    /*
     * 0: not ready - prerequisites are not sufficient
     * 1: ready
     * 2: low-budget
     * 3: on use
     * 4: done
     */
    private int currentState = 0;

    // Publisher-Related
    public EventHandler<ActionDataArgs> actionTaken;
    public EventHandler<ActionDataArgs> actionCompleted;
    private ActionDataArgs actionDataArgs;

    private ActionDataHolder actionDataHolder;
    private ActionData actionData;

    private ActionUIController actionUIController;

    [SerializeField]
    private GameObject actionInfoPanel;


    public void Awake()
    {
        actionUIController = GetComponent<ActionUIController>();

        actionDataHolder = GetComponent<ActionDataHolder>();
        actionData = actionDataHolder.actionData;

        actionDataArgs = new ActionDataArgs(this, actionData);

        SetDefaultState();
    }


    private void SetDefaultState()
    {
        if (prerequisiteAction != null)
        {
            SetCurrentState(0);
        }
        else
        {
            SetCurrentState(1);
        }
    }


    private void SetCurrentState(int newState)
    {
        currentState = newState;

        if (newState == 0)
        {
            actionUIController.OnPassive();
        }
        else if (newState == 1)
        {
            actionUIController.OnReady();
        }
        else if (newState == 2)
        {
            actionUIController.OnLowBudget();
        }
        else if (newState == 3)
        {
            actionUIController.OnUse();
        }
        else if (newState == 4)
        {
            actionUIController.OnCompleted();
        }
        else
        {
            Debug.Log("Unknown state type is tried to be set to" +
                      " SubscriberPublisher of an action. Action: " +
                      actionData.actionName);
        }
    }


    public void OnActionTaken()
    {
        Debug.Log("Clicked."+this.name);
        OnButtonClicked();
        actionInfoPanel.SetActive(false);
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
        if (actionTaken != null)
        {
            actionTaken(this, actionDataArgs);
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
     * It is called when the prerequisite action is completed.
     */
    public void OnAnotherActionCompleted(object source, ActionDataArgs actionDataArgs)
    {
        Debug.Log("Greetings from Greece." + this.name +  " " +actionDataArgs.actionData.actionName);
        gameObject.SetActive(true);
    }


    /*
    * Subscription-Related method
    *
    */
    public void OnBudgetChanged(object source, BudgetArgs budgetArgs)
    {
        Debug.Log("Saw the change in the budget." + this.name);
        if (budgetArgs.budget < actionData.cost)
        {
            actionUIController.OnLowBudget();
            Debug.Log("State: On low budget.");
        }
        else
        {
            actionUIController.OnReady();
        }
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

