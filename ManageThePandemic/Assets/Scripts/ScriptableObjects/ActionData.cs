using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Defines the data structure of actions.
 */
[CreateAssetMenu(menuName = "ManageThePandemic/Action")]
public class ActionData : ScriptableObject
{
    public string actionName;
    [TextArea]
    public string description;

    public List<MTPEvent> events;
}
