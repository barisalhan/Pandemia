using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/SocietyModel")]
public class SocietyModel : MTPScriptableObject
{
    public double economicWellBeing = 1;

    public double virusSituation = 1;

    // Measure of people's social and recreational activities
    public double personalWellBeing = 1;

    //[Dependent]
    public double happiness;

    public double CalculateHappiness()
    {
        happiness = Math.Min(Math.Min(economicWellBeing, virusSituation), personalWellBeing);
        return happiness;
    }

    public void ExecuteEvent(string targetParameter,
                             int effectType,
                             double effectValue)
    {
        if (effectType != 1)
        {
            Debug.Log("Unknown effect type is entered for the society model.");
            return;
        }
        
        ExecuteGeometricEvent(targetParameter, effectValue);

        CalculateHappiness();
    }

    private void ExecuteGeometricEvent(string targetParameter, 
                                       double effectValue)
    {
        if (targetParameter == "economicWellBeing")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                economicWellBeing += (1.0 - economicWellBeing) * effectValue;
            }
            else
            {
                economicWellBeing += (economicWellBeing - 0.0) * effectValue;
            }
        }
        else if (targetParameter == "virusSituation")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                virusSituation += (1.0 - virusSituation) * effectValue;
            }
            else
            {
                virusSituation += (virusSituation - 0.0) * effectValue;
            }
        }
        else if (targetParameter == "personalWellBeing")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                personalWellBeing += (1.0 - personalWellBeing) * effectValue;
            }
            else
            {
                personalWellBeing += (personalWellBeing - 0.0) * effectValue;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }
}
