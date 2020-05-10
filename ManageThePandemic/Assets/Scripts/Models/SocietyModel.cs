using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/SocietyModel")]
public class SocietyModel : MTPScriptableObject
{
    private const double MAX_ECONOMIC_WELL_BEING = 1;
    private const double MIN_ECONOMIC_WELL_BEING = 0;

    private const double MAX_VIRUS_SITUATION = 1;
    private const double MIN_VIRUS_SITUATION = 0;

    private const double MAX_PERSONAL_WELL_BEING = 1;
    private const double MIN_PERSONAL_WELL_BEING = 0;


    public double economicWellBeing = 1;

    public double virusSituation = 1;

    // Measure of people's social and recreational activities
    public double personalWellBeing = 1;

    //[Dependent]
    public double happiness;

    public EventHandler<HappinessArgs> HappinessChanged;


    public void SetDefaultModel()
    {
        economicWellBeing = 1;
        virusSituation = 1;
        personalWellBeing = 1;
    }

    public double CalculateHappiness()
    {
        happiness = Math.Min(Math.Min(economicWellBeing, virusSituation), personalWellBeing);

        OnHappinessChanged();

        return happiness;
    }

    public void OnHappinessChanged()
    {
        if (HappinessChanged != null)
        {
            HappinessChanged(this, new HappinessArgs(happiness));
        }
    }

    public void ExecuteEvent(string targetParameter,
                             int effectType,
                             double effectValue)
    {
        if (effectType != 1 && effectType != 3)
        {
            Debug.Log("Unknown effect type is entered for the society model.");
            return;
        }

        if (effectType == 1)
        {
            ExecuteGeometricEvent(targetParameter, effectValue);
        }
        else if (effectType == 3)
        {
            ExecuteReverseEvent(targetParameter, effectValue);
        }

        CalculateHappiness();
    }

    private void ExecuteGeometricEvent(string targetParameter, 
                                       double effectValue)
    {
        if (targetParameter == "economicWellBeing")
        {
            Debug.Log("Executing a geometric event in society model.");
            if (effectValue > 0)
            {
                economicWellBeing += (MAX_ECONOMIC_WELL_BEING - economicWellBeing) * effectValue;
            }
            else
            {
                economicWellBeing += (economicWellBeing - MIN_ECONOMIC_WELL_BEING) * effectValue;
            }
        }
        else if (targetParameter == "virusSituation")
        {
            Debug.Log("Executing a geometric event in society model.");
            if (effectValue > 0)
            {
                virusSituation += (MAX_VIRUS_SITUATION - virusSituation) * effectValue;
            }
            else
            {
                virusSituation += (virusSituation - MIN_VIRUS_SITUATION) * effectValue;
            }
        }
        else if (targetParameter == "personalWellBeing")
        {
            Debug.Log("Executing a geometric event in society model.");
            if (effectValue > 0)
            {
                personalWellBeing += (MAX_PERSONAL_WELL_BEING - personalWellBeing) * effectValue;
            }
            else
            {
                personalWellBeing += (personalWellBeing - MIN_PERSONAL_WELL_BEING) * effectValue;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }

    private void ExecuteReverseEvent(string targetParameter, double effectValue)
    {
        if (targetParameter == "economicWellBeing")
        {
            Debug.Log("Executing a reverse geometric event in society model.");
            if (effectValue > 0)
            {
                double nominator = economicWellBeing - effectValue * MAX_ECONOMIC_WELL_BEING;
                double denominator = 1 - effectValue;
                economicWellBeing = nominator / denominator;
            }
            else
            {
                double nominator = economicWellBeing + effectValue * MIN_ECONOMIC_WELL_BEING;
                double denominator = 1 + effectValue;
                economicWellBeing = nominator / denominator;
            }
        }
        else if (targetParameter == "virusSituation")
        {
            Debug.Log("Executing a reverse geometric event in society model.");
            if (effectValue > 0)
            {
                double nominator = virusSituation - effectValue * MAX_VIRUS_SITUATION;
                double denominator = 1 - effectValue;
                virusSituation = nominator / denominator;
            }
            else
            {
                double nominator = virusSituation + effectValue * MIN_VIRUS_SITUATION;
                double denominator = 1 + effectValue;
                virusSituation = nominator / denominator;
            }
        }
        else if (targetParameter == "personalWellBeing")
        {
            Debug.Log("Executing a geometric event in society model.");
            if (effectValue > 0)
            {
                double nominator = personalWellBeing - effectValue * MAX_PERSONAL_WELL_BEING;
                double denominator = 1 - effectValue;
                personalWellBeing = nominator / denominator;
            }
            else
            {
                double nominator = personalWellBeing + effectValue * MIN_PERSONAL_WELL_BEING;
                double denominator = 1 + effectValue;
                personalWellBeing = nominator / denominator;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }
}

public class HappinessArgs
{
    public double value;

    public HappinessArgs(double value)
    {
        this.value = value;
    }
}
