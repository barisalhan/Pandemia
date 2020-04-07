using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/Action")]
public class Action : ScriptableObject
{
    public string actionName;
    public string description;

    public List<MTPEvent> events;
}
