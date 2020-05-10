using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;


//TODO: Consider adapting to the name convention.

/*
 * A DAY:
 *  1.Events of the day in the calendar are executed.[SetParameter()]
 *  2.Instant events are executed.[SetParameter()]
 *  3.[UpdateParameters()]
 *  4.Calculate new daily cases of today at the end of the day. We use (active case
 *    number of yesterday) and (parameters of today) while doing this calculation.
 *  5.END
 *
 *  Convention:
 *      All parameters must be classified as independent and dependent.
 *      To restrict access, all dependent parameters must be private and
 *                          all independent parameters must be protected.
 */
[CreateAssetMenu(menuName = "ManageThePandemic/HealthSystemModel")]
public class HealthSystemModel : MTPScriptableObject
{
    private const double MAX_MEDICINE_EFFECT_ICU = 0.9;
    private const double MIN_MEDICINE_EFFECT_ICU = 0.6;

    private const double MAX_MEDICINE_EFFECT_ACU = 0.9;
    private const double MIN_MEDICINE_EFFECT_ACU = 0.6;

    // [Independent]
    // Number of ICU units.
    [SerializeField]
    private int ICUCapacity;


    // [Dependent]
    // It is actually number of new ICU cases for 14 days later. It will be added to
    // 14 days later in dictionary of ICU cases.
    private int dailyNewICUCases;

    // [Independent]
    // After DelayICU number of days, a patient under ICU either recovers or dies.
    private int delayICU = 5;

    // [Independent]
    // Number of available places for Active Cases. Defined as ACU(Active Case Unit) 
    [SerializeField]
    private int ACUCapacity;

    // [Independent]
    // After DelayACU number of days, a patient either recovers or gets in ICU.
    [SerializeField]
    private int delayACU = 14;

    // [Dependent]
    // By using dailyNewICUCases, this number is projected to DelayICU days later.
    private int dailyNewDeaths;

    // [Dependent]
    // By using dailyNewICUCases, this number is projected to DelayICU days later.
    private int dailyNewRecoveredsFromICU = 0;

    // [Dependent]
    // By using dailyNewACUCases, this number is projected to DelayACU days later.
    private int dailyNewRecoveredsFromACU = 0;

    // [Dependent]
    // Aggregate Active Cases as a function of day, kept as a dictionary.
    private Dictionary<int, int> aggregateACUCases = new Dictionary<int, int>();

    // [Dependent]
    // Aggregate Death Cases as a function of day, kept as a dictionary.
    public Dictionary<int, int> aggregateDeathCases = new Dictionary<int, int>();

    // [Dependent]
    // Aggregate Recovered Cases as a function of day, kept as a dictionary.
    public Dictionary<int, int> aggregateRecoveredCases = new Dictionary<int, int>();

    // [Dependent]
    // Number of current ICU cases as a function of days. Given as a dictionary. 
    private Dictionary<int, int> aggregateICUCases = new Dictionary<int, int>();

    // [Dependent]
    // Daily ICU Cases as a function of day, kept as a dictionary.
    private Dictionary<int, int> dailyACUCases = new Dictionary<int, int>();

    // [Dependent]
    // Daily ICU Cases as a function of day, kept as a dictionary.
    private Dictionary<int, int> dailyDeathCases = new Dictionary<int, int>();

    // [Dependent]
    // Daily RecoveredCases as a function of day, kept as a dictionary.
    private Dictionary<int, int> dailyRecoveredCases = new Dictionary<int, int>();

    // [Dependent]
    // Daily ICU Cases as a function of day, kept as a dictionary.
    private Dictionary<int, int> dailyICUCases = new Dictionary<int, int>();

    // [Dependent]
    // Daily ICU Cases as a function of day, kept as a dictionary.
    private Dictionary<int, int> dailyNetICUCases = new Dictionary<int, int>();


    // [Independent]
    // [Parameter]
    // Effect of medicine which determines recovery/ICU ratio for active cases.
    [SerializeField]
    private double medicineEffectACU;

    // [Independent]
    // [Parameter]
    // Effect of medicine which determines recovery/Death ratio for active cases.
    [SerializeField]
    private double medicineEffectICU;

    // [Independent]
    // [Parameter]
    // Amount of Mask Supply which will be used a parameter for infection probability.
    private int maskCapacity;

    // [Dependent]
    // [Parameter]
    // Recovery Ratio for active cases.
    private double recoveryRatioUnderACU;

    // [Dependent]
    // [Parameter]
    // Recovery ratio for ICU cases.
    private double recoveryRatioUnderICU;


    /*
     * Assigns 0 to the 0th day of all dictionaries.
     */
    public void SetDefaultModel()
    {
        dailyICUCases.Add(0, 0);
        dailyNetICUCases.Add(0, 0);
        dailyACUCases.Add(0, 0);
        dailyDeathCases.Add(0, 0);
        dailyRecoveredCases.Add(0, 0);

        aggregateDeathCases.Add(0, 0);
        aggregateACUCases.Add(0, 0);
        aggregateICUCases.Add(0, 0);
        aggregateRecoveredCases.Add(0, 0);
    }

    public int GetNewRecoveredCases(int day)
    {
        return dailyRecoveredCases[day];
    }

    public int GetNewDeathCases(int day)
    {
        return dailyDeathCases[day];
    }

    /*
     * Updates dependent model parameters.
     */
    public void UpdateParameters()
    {
        UpdateRecoveryRatioUnderACU();
        UpdateRecoveryRatioUnderICU();
    }

    /*
     * Calculate new deaths, recoveries, ICU cases and updates daily dictionaries.
     * TODO: Consider splitting this function into two as calculation and update fields.
     */
    public void CalculateAndUpdateDailyDictionaries(int dailyNewActiveCases)
    {
        CalculateDailyNewICUCases(dailyNewActiveCases);
        CalculateDailyNewDeaths();
        CalculateDailyNewRecoveredsFromACU(dailyNewActiveCases);
        CalculateDailyNewRecoveredsFromICU();

        UpdateDailyDictionaries(dailyNewActiveCases);
    }

    public void UpdateRecoveryRatioUnderICU()
    {
        int today = Time.GetInstance().GetDay();
        double ratioHasAccessToICU;

        if (!aggregateICUCases.ContainsKey(today) || aggregateICUCases[today] == 0)
        {
            ratioHasAccessToICU = 1;
        }
        else
        {
            ratioHasAccessToICU = Math.Min(ICUCapacity / (double) aggregateICUCases[today], 1.0);
        }

        recoveryRatioUnderICU = (ratioHasAccessToICU) * (medicineEffectICU);
    }

    public void UpdateRecoveryRatioUnderACU()
    {
        int today = Time.GetInstance().GetDay();
        double ratioHasAccessToACU;
        if (!aggregateACUCases.ContainsKey(today)  || aggregateACUCases[today] == 0)
        {
            ratioHasAccessToACU = 1;
        }
        else
        {
            ratioHasAccessToACU = Math.Min(ACUCapacity / (double) aggregateACUCases[today], 1.0);
        }

        recoveryRatioUnderACU = (ratioHasAccessToACU) * (medicineEffectACU);
    }

    // This is projected to DelayACU days later.
    public void CalculateDailyNewICUCases(int dailyNewActiveCases)
    {
        dailyNewICUCases = (int)((1 - recoveryRatioUnderACU) * (dailyNewActiveCases));
    }

    // This is projected to DelayACU days later.  
    public void CalculateDailyNewRecoveredsFromACU(int dailyNewActiveCases)
    {
        dailyNewRecoveredsFromACU = (int)(recoveryRatioUnderACU * dailyNewActiveCases);
    }

    // This is projected to DelayACU days later.  
    public void CalculateDailyNewDeaths()
    {
        int today = Time.GetInstance().GetDay();
        dailyNewDeaths = (int)((1 - recoveryRatioUnderICU) * dailyICUCases[today - 1]);
    }

    // This is projected to DelayICU days later.  
    public void CalculateDailyNewRecoveredsFromICU()
    {
        int today = Time.GetInstance().GetDay();
        dailyNewRecoveredsFromICU = (int)(recoveryRatioUnderICU * dailyICUCases[today - 1]);
    }

    /*
     * Adds value to the given dictionary taking the given delay into account.
     */
    public void AddToDictionary(int day, int delay, Dictionary<int, int> myDictionary, int val)
    {
        if (myDictionary.ContainsKey(day + delay))
        {
            myDictionary[day + delay] += val;
        }
        else
        {
            myDictionary.Add(day + delay, val);
        }
    }

    public void UpdateDailyDictionaries(int dailyNewActiveCases)
    {
        int today = Time.GetInstance().GetDay();
        // If nothing happened that day, it ensures keeping 0 at that index rather than None.
        AddToDictionary(today, 0, dailyICUCases, 0);
        AddToDictionary(today, 0, dailyNetICUCases, 0);
        AddToDictionary(today, 0, dailyACUCases, 0);
        AddToDictionary(today, 0, dailyRecoveredCases, 0);
        AddToDictionary(today, 0, dailyDeathCases, 0);

        // Passes projected dailyNewICUCases to the corresponding date. 
        AddToDictionary(today, delayACU, dailyICUCases, dailyNewICUCases);


        // Adds today's dailyNewActiveCases to daily dictionary.
        AddToDictionary(today, 0, dailyACUCases, dailyNewActiveCases);
        // Substracts today's new cases from aggregateActiveCases (delayACU day later).
        // Because they will either ICU or recover.
        AddToDictionary(today, delayACU, dailyACUCases, -dailyNewActiveCases);


        // Adds today's dailyNewICUCases to daily dictionary.
        AddToDictionary(today, delayACU, dailyNetICUCases, dailyNewICUCases);
        // Substracts today's new cases from  (delayICU day later).
        // Because they will either die or recover.
        AddToDictionary(today, delayICU, dailyNetICUCases, -dailyICUCases[today]);


        // Adds today's dailyNewRecoveredsFromACU to daily dictionary.
        AddToDictionary(today, delayACU, dailyRecoveredCases, dailyNewRecoveredsFromACU);
        // Adds today's dailyNewRecoveredsFromICU to aggregate dictionary.
        AddToDictionary(today, delayICU, dailyRecoveredCases, dailyNewRecoveredsFromICU);


        // Adds today's dailyNewDeaths(projected to delayICU days later) to daily dictionary.
        AddToDictionary(today, delayICU, dailyDeathCases, dailyNewDeaths);
    }


    /*
     * Takes the daily change info from daily dictionaries and applies to aggregate ones.
     */
    public void UpdateAggregateDictionaries()
    {
        int today = Time.GetInstance().GetDay();

        AddToDictionary(today, 0, aggregateICUCases, dailyICUCases[today]);
        AddToDictionary(today, 0, aggregateICUCases, aggregateICUCases[today - 1]);

        AddToDictionary(today, 0, aggregateACUCases, dailyACUCases[today]);
        AddToDictionary(today, 0, aggregateACUCases, aggregateACUCases[today - 1]);

        AddToDictionary(today, 0, aggregateRecoveredCases, dailyRecoveredCases[today]);
        AddToDictionary(today, 0, aggregateRecoveredCases, aggregateRecoveredCases[today - 1]);

        AddToDictionary(today, 0, aggregateDeathCases, dailyDeathCases[today]);
        AddToDictionary(today, 0, aggregateDeathCases, aggregateDeathCases[today - 1]);

    }


    public void ExecuteEvent(string targetParameter,
                             int effectType,
                             double effectValue)
    {
        if (effectType != 0 && effectType != 1 && effectType!=3)
        {
            Debug.Log("Unknown effect type is entered for the health system model.");
            return;
        }

        if (effectType == 1)
        {
            ExecuteGeometricEvent(targetParameter, effectValue);
        }
        else if (effectType == 0)
        {
            ExecuteArithmeticEvent(targetParameter, effectValue);
        }
        else if (effectType == 3)
        {
            ExecuteReverseEvent(targetParameter, effectValue);
        }
    }

    private void ExecuteArithmeticEvent(string targetParameter, double effectValue)
    {
        if (targetParameter == "ACUCapacity")
        {
            ACUCapacity += (int)effectValue;
        }
        else if (targetParameter == "ICUCapacity")
        {
            ICUCapacity += (int)effectValue;
        }
    }


    private void ExecuteGeometricEvent(string targetParameter,
        double effectValue)
    {
        if (targetParameter == "medicineEffectACU")
        {
            Debug.Log("Executing a geometric event in health system model.");
            if (effectValue > 0)
            {
                medicineEffectACU += (MAX_MEDICINE_EFFECT_ACU - medicineEffectACU) * effectValue;
            }
            else
            {
                medicineEffectACU += (medicineEffectACU - MIN_MEDICINE_EFFECT_ACU) * effectValue;
            }
        }
        else if (targetParameter == "medicineEffectICU")
        {
            Debug.Log("Executing a geometric event in health system model.");
            if (effectValue > 0)
            {
                medicineEffectICU += (MAX_MEDICINE_EFFECT_ICU - medicineEffectICU) * effectValue;
            }
            else
            {
                medicineEffectICU += (medicineEffectICU - MIN_MEDICINE_EFFECT_ICU) * effectValue;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }

    private void ExecuteReverseEvent(string targetParameter, double effectValue)
    {
        if (targetParameter == "medicineEffectACU")
        {
            Debug.Log("Executing a reverse geometric event in health system model.");
            if (effectValue > 0)
            {
                double nominator = medicineEffectACU - effectValue * MAX_MEDICINE_EFFECT_ACU;
                double denominator = 1 - effectValue;
                medicineEffectACU = nominator / denominator;
            }
            else
            {
                double nominator = medicineEffectACU + effectValue * MIN_MEDICINE_EFFECT_ACU;
                double denominator = 1 + effectValue;
                medicineEffectACU = nominator / denominator;
            }
        }
        else if (targetParameter == "medicineEffectICU")
        {
            Debug.Log("Executing a reverse geometric event in health system model.");
            if (effectValue > 0)
            {
                double nominator = medicineEffectICU - effectValue * MAX_MEDICINE_EFFECT_ICU;
                double denominator = 1 - effectValue;
                medicineEffectICU = nominator / denominator;
            }
            else
            {
                double nominator = medicineEffectICU + effectValue * MIN_MEDICINE_EFFECT_ICU;
                double denominator = 1 + effectValue;
                medicineEffectICU = nominator / denominator;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }
}
