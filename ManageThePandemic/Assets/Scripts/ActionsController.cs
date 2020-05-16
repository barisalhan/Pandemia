using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


/*
 * Controls the relationships between Actions and GameController.
 *
 * An action consists of:
 *  ActionData
 *  SubscriberPublisher
 *  ActionInfoViewer
 *
 */
public class ActionsController : MonoBehaviour
{
    public List<GameObject> actions;

    private GameController gameController;

    public void Awake()
    {
        gameController = GetComponent<GameController>();

        GetActionsInTheGame();

        SubscribeActionsToPrerequisiteActions();
        SubscribeActionsToBudget();
    }


    private void GetActionsInTheGame()
    {
        actions = GameObject.FindGameObjectsWithTag("ActionObject").ToList();
    }


    private void SubscribeActionsToPrerequisiteActions()
    {
        foreach (var action in actions)
        {
            var subscriber = action.GetComponent<SubscriberPublisher>();

            if (subscriber.prerequisiteAction != null)
            {
                SubscriberPublisher publisher = subscriber.prerequisiteAction.GetComponent<SubscriberPublisher>();
                publisher.actionCompleted += subscriber.OnAnotherActionCompleted;
            }
        }
    }


    private void SubscribeActionsToBudget()
    {
        foreach (var action in actions)
        {
            ActionDataHolder actionDataHolder = action.GetComponent<ActionDataHolder>();
            SubscriberPublisher subscriber = action.GetComponent<SubscriberPublisher>();

            if (actionDataHolder.actionData.cost > 0)
            {
                gameController.countryController.BudgetChanged += subscriber.OnBudgetChanged;
            }
        }
    }

}
