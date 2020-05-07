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
    [SerializeField]
    public List<GameObject> actions;

    private GameController gameController;

    public void Awake()
    {
        gameController = GetComponent<GameController>();

        GetActionsInTheGame();

        SubscribeActionsToOtherActions();
        SubscribeActionsToBudget();
    }


    private void GetActionsInTheGame()
    {
        actions = GameObject.FindGameObjectsWithTag("ActionObject").ToList();
    }


    private void SubscribeActionsToOtherActions()
    {
        foreach (var action in actions)
        {
            var publisher = action.GetComponent<SubscriberPublisher>();
          
            foreach (var dependentAction in publisher.dependentActions)
            {
                // TODO: Check here if the loading time is too much.
                SubscriberPublisher subscriber = dependentAction.GetComponent<SubscriberPublisher>();
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
