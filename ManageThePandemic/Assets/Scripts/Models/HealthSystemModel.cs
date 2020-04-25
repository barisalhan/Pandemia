using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.Audio.Google;
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
    // [Independent]
    // Number of ICU units.
    [SerializeField]
    private int icuCapacity;


    // [Dependent]
    // It is actually number of new ICU cases for 14 days later. It will be added to
    // 14 days later in dictionary of ICU cases.
    private int dailyNewICUCases;

    // [Independent]
    // After DelayICU number of days, a patient under ICU either recovers or dies.
    private int delayICU = 5;

    // [Independent]
    // Number of available places for Active Cases. Defined as ACU(Active Case Unit) 
    private int activeCaseUnitCapacity;

    // [Independent]
    // After DelayACU number of days, a patient either recovers or gets in ICU.
    private int delayACU = 14;

    // [Dependent]
    // By using dailyNewICUCases, this number is projected to DelayICU days later.
    private int dailyNewDeaths;

    // [Dependent]
    // By using dailyNewICUCases, this number is projected to DelayICU days later.
    private int dailyNewRecoveredsFromICU;

    // [Dependent]
    // By using dailyNewACUCases, this number is projected to DelayACU days later.
    private int dailyNewRecoveredsFromACU;


    // [Dependent]
    // Daily ICU Cases as a function of day, kept as a dictionary.
    private Dictionary<int, int> dailyICUCases = new Dictionary<int, int>();


    // [Independent]
    // [Parameter]
    // Effect of medicine which determines recovery/ICU ratio for active cases.
    private double medicineEffectACU;

    // [Independent]
    // [Parameter]
    // Effect of medicine which determines recovery/Death ratio for active cases.
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
    }

    /*
     * Updates dependent model parameters.
     */ 
     public void UpdateParameters(Dictionary<int, int> aggregateActiveCases, Dictionary<int, int> aggregateICUCases)
    {
        UpdateRecoveryRatioUnderACU(aggregateActiveCases);
        UpdateRecoveryRatioUnderICU(aggregateICUCases);
    }

    /*
     * Calculate new deaths, recoveries, ICU cases and updates related fields(dictionaries).
     * TODO: Consider splitting this function into two as calculation and update fields.
     */
    public void CalculateNewDeathsRecoveriesAndUpdateFields(int dailyNewActiveCases,
        Dictionary<int, int> aggregateActiveCases,
        Dictionary<int, int> aggregateICUCases,
        Dictionary<int, int> aggregateRecoveredCases,
        Dictionary<int, int> aggregateDeaths)
    {
        CalculateDailyNewICUCases(dailyNewActiveCases);
        CalculateDailyNewDeaths();
        CalculateDailyNewRecoveredsFromACU(dailyNewActiveCases);
        CalculateDailyNewRecoveredsFromICU();

        UpdateCaseAndDeathLists(dailyNewActiveCases,
        aggregateActiveCases,
        aggregateICUCases,
        aggregateRecoveredCases,
        aggregateDeaths);
    }











    /*
     *
     * 
     */ 

    public void UpdateRecoveryRatioUnderICU(Dictionary<int, int> aggregateICUCases)
    {
        int today = Time.GetInstance().GetDay();

        double ratioHasAccessToICU = Math.Min(icuCapacity / (double)aggregateICUCases[today], 1.0);

        recoveryRatioUnderICU = (ratioHasAccessToICU) * (medicineEffectICU);
    }

    public void UpdateRecoveryRatioUnderACU(Dictionary<int, int> aggregateActiveCases)
    {
        int today = Time.GetInstance().GetDay();

        double ratioHasAccessToACU = Math.Min(activeCaseUnitCapacity / (double)aggregateActiveCases[today], 1.0);

        recoveryRatioUnderACU = (ratioHasAccessToACU) * (medicineEffectACU);
    }

    // This is projected to DelayACU days later.
    public void CalculateDailyNewICUCases(int dailyNewActiveCases)
    {
        dailyNewICUCases = (int)((1 - recoveryRatioUnderACU) * (dailyNewActiveCases));
    }
    // This is projected to DelayACU days later.  
    public void CalculateDailyNewDeaths()
    {
        int today = Time.GetInstance().GetDay();
        dailyNewDeaths = (int)((1 - recoveryRatioUnderICU) * dailyICUCases[today - 1]);
    }

    // This is projected to DelayACU days later.  
    public void CalculateDailyNewRecoveredsFromACU(int dailyNewActiveCases)
    {
        dailyNewRecoveredsFromACU = (int)(recoveryRatioUnderACU * dailyNewActiveCases);
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

    public void UpdateCaseAndDeathLists(int dailyNewActiveCases, 
        Dictionary<int,int> aggregateActiveCases,
        Dictionary<int, int> aggregateICUCases,
        Dictionary<int, int> aggregateRecoveredCases,
        Dictionary<int, int> aggregateDeaths)
    {
        int today = Time.GetInstance().GetDay();

        // Passes projected dailyNewICUCases to the corresponding date. 
        AddToDictionary(today, delayACU, dailyICUCases, dailyNewICUCases); 


        // Passes yesterday's aggregate value to today.
        AddToDictionary(today, 0, aggregateActiveCases, aggregateActiveCases[today - 1]);
        // Adds today's dailyNewActiveCases to aggregate dictionary.
        AddToDictionary(today, 0, aggregateActiveCases, dailyNewActiveCases);
        // Substracts today's new cases from aggregateActiveCases (delayACU day later).
        // Because they will either ICU or recover.
        AddToDictionary(today, delayACU, aggregateActiveCases, -dailyNewActiveCases);


        // Passes yesterday's aggregate value to today.
        AddToDictionary(today, 0, aggregateICUCases, aggregateICUCases[today - 1]);
        // Adds today's dailyNewICUCases to aggregate dictionary.
        AddToDictionary(today, delayACU, aggregateICUCases, dailyNewICUCases);
        // Substracts today's new cases from aggregateICUCases (delayICU day later).
        // Because they will either die or recover.
        AddToDictionary(today, delayICU, aggregateICUCases, -dailyICUCases[today]);


        // Passes yesterday's aggregate value to today.
        AddToDictionary(today, 0, aggregateRecoveredCases, aggregateRecoveredCases[today - 1]);
        // Adds today's dailyNewRecoveredsFromACU to aggregate dictionary.
        AddToDictionary(today, delayACU, aggregateRecoveredCases, dailyNewRecoveredsFromACU);
        // Adds today's dailyNewRecoveredsFromICU to aggregate dictionary.
        AddToDictionary(today, delayICU, aggregateRecoveredCases, dailyNewRecoveredsFromICU);


        // Passes yesterday's aggregate value to today.
        AddToDictionary(today, 0, aggregateDeaths, aggregateDeaths[today - 1]);
        // Adds today's dailyNewDeaths(projected to delayICU days later) to aggregate dictionary.
        AddToDictionary(today, delayICU, aggregateDeaths, dailyNewDeaths);      
    }

}

