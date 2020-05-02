using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Controls the relationships between Action Buttons.
 */
public class ActionsController : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> actions;

    private List<ActionYesButtonHandler> actionYesButtonHandlers = new List<ActionYesButtonHandler>();

    public void Awake()
    {
        GetButtonHandlersFromActions();
        SubscribeButtonHandlersToPublishers();
    }


    private void GetButtonHandlersFromActions()
    {
        foreach (var action in actions)
        {
            actionYesButtonHandlers.Add(action.GetComponent<ActionYesButtonHandler>());
        }
    }


    private void SubscribeButtonHandlersToPublishers()
    {
        foreach (var actionButtonHandler in actionYesButtonHandlers)
        {
            foreach (var dependentAction in actionButtonHandler.dependentActions)
            {
                // TODO: Check here if the loading time is too much.
                ActionYesButtonHandler dependentYesButtonHandler = dependentAction.GetComponent<ActionYesButtonHandler>();
                actionButtonHandler.ButtonClicked += dependentYesButtonHandler.OnOtherButtonClicked;
            }
        }
    }





    /*// [Name of Action, Index of that Action in the actions list.]
    public Dictionary<Name, int> indexTable = new Dictionary<Name, int>();

    /*
     * Names of all possible actions.
     *
     * Update this enum manually as new actions are added.
     #1#
    public enum Name
    {
        AAction,
        BAction
    }

    /*
     * Creates an index table to enable easy access to actions
     * by using Name enum.
    #1#
    public void CreateIndexTable()
    {
        for (int index = 0; index < actions.Count; index++)
        {
            Name currentAction;
            if (Enum.TryParse<Name>(actions[index].name, out currentAction))
            {
                indexTable.Add(currentAction, index);
            }
            else
            {
                Debug.Log("There is an inconsistency between actionName and Name enum list. " +
                          actions[index].name + " is tried to be reached");
            }
        }
    }

    public GameObject GetAction(Name actionName)
    {
        int index = indexTable[actionName];
        return actions[index];
    }*/
}
