using System;
using System.Collections.Generic;
using UnityEngine;


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
[CreateAssetMenu(menuName = "Pandemia/HealthSystemModel")]
public class HealthSystemModel : MTPScriptableObject
{
    private const double INITIAL_InputMedicineEffectACU = 0;
    private const double INITIAL_InputMedicineEffectICU = 0;

    private const double INITIAL_AcuCapacity = 3000;
    private const double INITIAL_IcuCapacity = 3000;

    private const int INITIAL_DelayACU = 3;
    private const int INITIAL_DelayICU = 1;

    public static Double[] LIMITS_MedicineEffectACU = { 0.1, 0.89 };
    public static Double[] LIMITS_MedicineEffectICU = { 0.1, 0.7 };
    // UNUSED!
    public static Double[] LIMITS_AcuCapacity = { 0.0, 0.0 };
    // UNUSED!
    public static Double[] LIMITS_IcuCapacity = { 0.0, 0.0 };




    private double inputMedicineEffectACU;
    public double InputMedicineEffectACU
    {
        get { return inputMedicineEffectACU; }
        set
        {
            inputMedicineEffectACU = value;
            MedicineEffectACU = Sigmoid(value, LIMITS_MedicineEffectACU[0],
                LIMITS_MedicineEffectACU[1], Temperatures.T_MedACU);
        }
    }
    // [Independent] [Parameter]
    // Effect of medicine which determines recovery/ICU ratio for active cases.
    [SerializeField]
    private double medicineEffectACU;
    public double MedicineEffectACU
    {
        get { return medicineEffectACU; }
        set { medicineEffectACU = value; }
    }




    private double inputMedicineEffectICU;
    public double InputMedicineEffectICU
    {
        get { return inputMedicineEffectICU; }
        set
        {
            inputMedicineEffectICU = value;
            MedicineEffectICU = Sigmoid(value, LIMITS_MedicineEffectICU[0], 
                LIMITS_MedicineEffectICU[1], Temperatures.T_MedICU);
        }
    }
    // [Independent] [Parameter]
    // Effect of medicine which determines recovery/Death ratio for active cases.
    [SerializeField]
    private double medicineEffectICU;
    public double MedicineEffectICU
    {
        get { return medicineEffectICU; }
        set { medicineEffectICU = value; }
    }


    // [Independent]
    // Number of available places for Active Cases. Defined as ACU(Active Case Unit) 
    [SerializeField]
    private double acuCapacity;
    public double AcuCapacity
    {
        get { return acuCapacity; }
        set { acuCapacity = value; }
    }


    // [Independent]
    // Number of ICU units.
    [SerializeField]
    private double icuCapacity;
    public double IcuCapacity
    {
        get { return icuCapacity; }
        set { icuCapacity = value; }
    }



    // [Dependent]
    // It is actually number of new ICU cases for 14 days later. It will be added to
    // 14 days later in dictionary of ICU cases.
    private int dailyNewICUCases;

    // [Independent]
    // After DelayICU number of days, a patient under ICU either recovers or dies.
    private int delayICU;

    // [Independent]
    // After DelayACU number of days, a patient either recovers or gets in ICU.
    private int delayACU;

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
        InputMedicineEffectACU = INITIAL_InputMedicineEffectACU;
        InputMedicineEffectICU = INITIAL_InputMedicineEffectICU;
        acuCapacity = INITIAL_AcuCapacity;
        icuCapacity = INITIAL_IcuCapacity;
        delayACU = INITIAL_DelayACU;        
        delayICU = INITIAL_DelayICU;


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


    public double CalculateHospitalOccupancyRate()
    {
        int today = Time.GetInstance().GetDay();
        return aggregateACUCases[today - 1] / acuCapacity;
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
        int today = Time.GetInstance().GetDay();

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
            ratioHasAccessToICU = Math.Min(icuCapacity / (double) aggregateICUCases[today], 1.0);
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
            ratioHasAccessToACU = Math.Min(acuCapacity / (double) aggregateACUCases[today], 1.0);
        }

        recoveryRatioUnderACU = (ratioHasAccessToACU) * (medicineEffectACU);
    }

    // This is projected to DelayACU days later.
    public void CalculateDailyNewICUCases(int dailyNewActiveCases)
    {
        dailyNewICUCases = (int)(Math.Round((1 - recoveryRatioUnderACU) * (dailyNewActiveCases)));
    }

    // This is projected to DelayACU days later.  
    public void CalculateDailyNewRecoveredsFromACU(int dailyNewActiveCases)
    {
        dailyNewRecoveredsFromACU = (int)Math.Round(recoveryRatioUnderACU * dailyNewActiveCases);

    }

    // This is projected to DelayACU days later.  
    public void CalculateDailyNewDeaths()
    {
        int today = Time.GetInstance().GetDay();
        dailyNewDeaths = (int)Math.Round((1 - recoveryRatioUnderICU) * dailyICUCases[today - 1]);

    }

    // This is projected to DelayICU days later.  
    public void CalculateDailyNewRecoveredsFromICU()
    {
        int today = Time.GetInstance().GetDay();
        dailyNewRecoveredsFromICU = (int)Math.Round(recoveryRatioUnderICU * dailyICUCases[today - 1]);

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
}
