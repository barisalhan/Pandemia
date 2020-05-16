using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Defines the data structure of actions.
 *
 * In case an action has construction time to be completed,
 * it has nonzero timeToComplete.
 *
 * If the action can be applied directly but observing its effects
 * takes time, we use delay time in the events of the action.
 */
[CreateAssetMenu(menuName = "ManageThePandemic/Action")]
public class ActionData : ScriptableObject
{
    public string actionName;
    [TextArea]
    public string description;

    [TextArea]
    public string hint;

    public int cost;

    public int timeToComplete;

    public List<MTPEvent> events;
}
