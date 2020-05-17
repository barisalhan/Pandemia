using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


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
     *  WARNING: Do NOT change this variable
     *  outside of SetCurrentState method.
     *  It is interconnected with ActionUIController.
     */
    private state currentState = state.Passive;

    // Publisher-Related
    public EventHandler<ActionDataArgs> actionTaken;
    public EventHandler<ActionDataArgs> actionCompleted;
    private ActionDataArgs actionDataArgs;

    private ActionDataHolder actionDataHolder;
    private ActionData actionData;

    private ActionUIController actionUIController;

    [SerializeField]
    private GameObject actionInfoPanel;

    private bool isBudgetSufficient;

    public enum state
    {
        Passive,
        Ready,
        LowBudget,
        OnConstruction,
        OnUse,
        Completed
    }

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
            SetCurrentState(state.Passive);
        }
        else
        {
            SetCurrentState(state.Ready);
        }
    }


    private void SetCurrentState(state newState)
    {
        currentState = newState;

        if (newState.Equals(state.Passive))
        {
            Debug.Log("OnPassive is called with action: " + actionData.actionName);
            actionUIController.OnPassive();
        }
        else if (newState.Equals(state.Ready))
        {
            Debug.Log("OnReady is called with action: " + actionData.actionName);
            actionUIController.OnReady();
        }
        else if (newState.Equals(state.LowBudget))
        {
            Debug.Log("OnLowbudget is called with action: " + actionData.actionName);
            actionUIController.OnLowBudget();
        }
        else if (newState.Equals(state.OnUse))
        {
            Debug.Log("OnUse is called with action: " + actionData.actionName);
            actionUIController.OnUse();
        }
        else if (newState.Equals(state.Completed))
        {
            Debug.Log("OnCompleted is called with action: " + actionData.actionName);
            actionUIController.OnCompleted();
        }
        else if (newState.Equals(state.OnConstruction))
        {
            Debug.Log("OnConstruction is called with action: " + actionData.actionName);
            actionUIController.OnConstruction();
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
     * when the OnActionConstructionCompleted() method is called.
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
        if (actionData.type == 0)
        {
            SetCurrentState(state.Completed);
        }
        else
        {
            Debug.Log("Unexpected action type arrived to OnActionCompleted." +
                      " Action Name: " + actionData.actionName);
        }

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
        SetReadyOrLowBudget();
    }


    /*
    * Subscription-Related method
    *
    */
    public void OnBudgetChanged(object source, BudgetArgs budgetArgs)
    {
        if (currentState.Equals(state.Completed))
        {
            return;
        }

        //Debug.Log("Saw the change in the budget." + this.name);
        if (budgetArgs.budget < actionData.cost)
        {
            isBudgetSufficient = false;
            if (currentState.Equals(state.Ready))
            {
                SetCurrentState(state.LowBudget);
                Debug.Log("Action State: On low budget.");
            }
        }
        else if (budgetArgs.budget >= actionData.cost)
        {
            isBudgetSufficient = true;
            if (currentState.Equals(state.LowBudget))
            {
                SetCurrentState(state.Ready);
                Debug.Log("Action State: On sufficient budget.");
            }
        }
    }

    public void SetReadyOrLowBudget()
    {
        Debug.Log("SetReadyOrLowBudget is called with action: " + actionData.actionName);
        if (isBudgetSufficient)
        {
            SetCurrentState(state.Ready);
        }
        else
        {
            SetCurrentState(state.LowBudget);
        }
    }

    public void OnConstruction()
    {
        SetCurrentState(state.OnConstruction);
    }

    public void OnUse()
    {
        SetCurrentState(state.OnUse);
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

