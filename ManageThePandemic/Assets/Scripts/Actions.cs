using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Holds the list of all actions and
 * retrieves the desired action.
 */
public class Actions : MonoBehaviour
{
    // It is set from the inspector.
    public List<Action> actions;

    // [Name of Action, Index of that Action in the actions list.]
    public Dictionary<Name, int> indexTable = new Dictionary<Name, int>();


    /*
     * Names of all possible actions.
     *
     * Update this enum manually as new actions are added.
     */
    public enum Name
    {
        PropagandaToRaiseAwareness
    }
    

    public void Start()
    {
        CreateIndexTable();
    }

    
    /*
     * Creates an index table to enable easy access to actions
     * by using Name enum.
     */
    public void CreateIndexTable()
    {
        for (int index = 0; index < actions.Count; index++)
        {
            Name currentAction;
            if(Enum.TryParse<Name>(actions[index].actionName, out currentAction))
            {
                indexTable.Add(currentAction, index);
            }
            else
            {
                Debug.Log("There is an inconsistency between actionName and Name enum list. " +
                          actions[index].actionName + " is tried to be reached");
            }
        }
    }


    public Action GetAction(Name actionName)
    {
        int index = indexTable[actionName];
        return actions[index];
    }
}
