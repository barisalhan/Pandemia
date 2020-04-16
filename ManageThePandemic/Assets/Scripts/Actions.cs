using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: comment
public class Actions : MonoBehaviour
{
    public List<Action> actions;

    // [Name of Action, Index of that Action in the actions list.]
    public Dictionary<string, int> indexTable = new Dictionary<string, int>();

    public void FindIndexes()
    {
        for (int index = 0; index <  actions.Count; index++)
        {
            indexTable.Add(actions[index].actionName, index);
        }
    }

    public Action GetAction(string actionName)
    {
        int index = indexTable[actionName];
        return actions[index];
    }
}
