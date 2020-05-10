using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using Object = System.Object;


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
 */
[CreateAssetMenu(menuName = "ManageThePandemic/VirusModel")]
public class VirusModel : MTPScriptableObject
{
    private const double MAX_GROWTH_RATE_PARAMETER = 0.5;
    private const double MIN_GROWTH_RATE_PARAMETER = 0.1;

    private const double MAX_EFFECT_RATIO = 1.0;
    private const double MIN_EFFECT_RATIO = 0.1;

    // [Dependent]
    // Ratio of population who can be get infected. [vulnerable population / Normalized population]
    [SerializeField]
    private double vulnerabilityRatio;

    // [Independent]    
    // Ratio of population who is active in daily life.
    [SerializeField]
    private double effectRatio;


    // TODO: It is not parameter. Change its name both from here and from actions.
    // [Dependent]
    [SerializeField]
    private double growthRateParameter;
    
    // [Dependent]
    // [Day, new case number on that day]
    private Dictionary<int, int> dailyNewCaseNumbers = new Dictionary<int, int>();

    // [Dependent]
    // [Day, Growth Rate on that day]
    private Dictionary<int, double> growthRates = new Dictionary<int, double>();


    /*
     * It is called on the first day of simulation.
     */
    public void SetDefaultModel()
    {
        effectRatio = 1;
        growthRateParameter = 0.5;
        dailyNewCaseNumbers.Add(0, 1);
    }


    /*
     * Updates the dependent parameters.
     */
    public void UpdateParameters(int population, int vulnerablePopulation)
    {
        UpdateVulnerabilityRatio(population, vulnerablePopulation);
    }


    /*
     * Updates the value, according to death and recovered numbers of yesterday.
     */
    public void UpdateVulnerabilityRatio(int normalizedPopulation, int vulnerablePopulation)
    {
        if (normalizedPopulation != 0)
        {
            vulnerabilityRatio = (double)vulnerablePopulation / (double)normalizedPopulation;
        }
        else
        {
            vulnerabilityRatio = 0;
            Debug.Log("Vulnerability ratio is tried to be calculated while population is zero.");
        }
    }


    public int CalculateDailyNewCase(int normalizedPopulation, int activeCaseNumberOfYesterday)
    {
        double formula = ((double)normalizedPopulation / (double)activeCaseNumberOfYesterday) - 1;
        double delay = Math.Log(formula) / (growthRateParameter * vulnerabilityRatio);

        double denominator = 1 + Math.Exp((-(growthRateParameter * vulnerabilityRatio)) * (1 - delay));
        double aggregateActiveCaseNumber = normalizedPopulation / denominator;

        // TODO: aggregateActiveCaseNumber > activeCaseNumberOfYesterday [varsayim]
        return (int)Math.Ceiling(effectRatio*(aggregateActiveCaseNumber - activeCaseNumberOfYesterday));
    }

    public void ExecuteEvent(string targetParameter,
                             int effectType,
                             double effectValue)
    {
        if (effectType != 1 && effectType != 3)
        {
            Debug.Log("Unknown effect type is entered for the virus model.");
            return;
        }

        if (effectType == 1)
        {
            ExecuteGeometricEvent(targetParameter, effectValue);
        }
        if (effectType == 3)
        {
            ExecuteReverseEvent(targetParameter, effectValue);
        }
    }


    private void ExecuteGeometricEvent(string targetParameter,
                                       double effectValue)
    {
        if(targetParameter == "growthRateParameter")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                growthRateParameter += (MAX_GROWTH_RATE_PARAMETER - growthRateParameter) * effectValue;
            }
            else
            {
                growthRateParameter += (growthRateParameter - MIN_GROWTH_RATE_PARAMETER) * effectValue;
            }
        }
        else if (targetParameter == "effectRatio")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                effectRatio += (MAX_EFFECT_RATIO  - effectRatio) * effectValue;
            }
            else
            {
                effectRatio += (effectRatio - MIN_EFFECT_RATIO) * effectValue;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }

    private void ExecuteReverseEvent(string targetParameter, double effectValue)
    {
        if (targetParameter == "growthRateParameter")
        {
            Debug.Log("Executing a reverse geometric event in virus model.");
            if (effectValue > 0)
            {
                double nominator = growthRateParameter - effectValue * MAX_GROWTH_RATE_PARAMETER;
                double denominator = 1 - effectValue;
                growthRateParameter = nominator / denominator;
            }
            else
            {
                double nominator = growthRateParameter + effectValue * MIN_GROWTH_RATE_PARAMETER;
                double denominator = 1 + effectValue;
                growthRateParameter = nominator / denominator;
            }
        }
        else if (targetParameter == "effectRatio")
        {
            Debug.Log("Executing a reverse geometric event in virus model.");
            if (effectValue > 0)
            {
                double nominator = effectRatio - effectValue * MAX_EFFECT_RATIO;
                double denominator = 1 - effectValue;
                effectRatio = nominator / denominator;
            }
            else
            {
                double nominator = effectRatio + effectValue * MIN_EFFECT_RATIO;
                double denominator = 1 + effectValue;
                effectRatio = nominator / denominator;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }

}