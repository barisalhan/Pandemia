using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A data type for events.
 */
[CreateAssetMenu(menuName = "ManageThePandemic/Event")]
public class MTPEvent : ScriptableObject
{
    public string targetModelName;
    public string targetParameter;

    /*
     * Holds how effectValue is affecting targetParameter.
     *
     * 0: arithmetic effect
     * 1: geometric effect
     * 2: boolean value change
     *      Ex: Change the value of isQuarantined.
     * 3: reverse effect
     *      Currently, reverse effect is only valid for
     *    geometric events. It reverts the effect of a
     *    temporary action when the time ends.
     */
    public int effectType;

    /*
     * If effectType is
     *  -arithmetic: add this value to the targetParameter.
     *  -geometric: multiply this to the targetParameter.
     *  -boolean: false or true( 0 or 1)
     */
    public double effectValue;

    /*
     * Holds how many days later the event will affect the world.
     * Its unit is 'day'.
     */
    public int delayTime;
}