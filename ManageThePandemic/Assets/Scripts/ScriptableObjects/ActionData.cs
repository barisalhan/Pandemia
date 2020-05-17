using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: add duration

/*
 * Defines the data structure of actions.
 *
 * In case an action has construction time to be completed,
 * it has nonzero timeToConstruct.
 *
 * If the action can be applied directly but observing its effects
 * takes time, we use delay time in the events of the action.
 */
[CreateAssetMenu(menuName = "ManageThePandemic/Action")]
public class ActionData : ScriptableObject
{
    [SerializeField]
    public string actionName;

    [TextArea]
    public string description;

    [TextArea]
    public string hint;

    /*
     * 0: One-time use actions
     * 1: Reusable
     */
    [SerializeField]
    public int type;

    [SerializeField]
    public int cost;

    /*
     * Construction time required for action to show its effects. 
     */
    [SerializeField]
    public int timeToConstruct;

    /*
     * Time that action stays effective.
     * It is valid only for reusable actions.
     */
    [SerializeField] 
    public int duration;

    [SerializeField]
    public List<MTPEvent> events;
}
