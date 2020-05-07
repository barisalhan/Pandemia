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
 *      To restrict access, all dependent parameters must be private and
 *                          all independent parameters must be protected.
 */
//TODO: change virusModel.
[CreateAssetMenu(menuName = "ManageThePandemic/VirusModel")]
public class VirusModel : MTPScriptableObject
{
    // [Dependent]
    // Ratio of population who can be get infected. [vulnerable population / population]
    [SerializeField]
    private double vulnerabilityRatio;

    // [Independent]    
    // Probability of transmitting to a vulnerable person through physical contact.
    [SerializeField]
    private double effectRatio;

    // [Dependent]
    // It is calculated as a result of independent parameters.
    // It will multiplied with the activeCaseNumber.
    // The result is the new dailyCaseNumber;
    [SerializeField]
    private double growthRateParameter;

    // [Dependent]
    // [Day, new case number on that day]
    private Dictionary<int, int> dailyNewCaseNumbers = new Dictionary<int, int>();

    // [Dependent]
    // [Day, Growth Rate on that day]
    private Dictionary<int, double> growthRates = new Dictionary<int, double>();


    // TODO: Think about creating a super-class.
    /*
     * It is called on the first day of simulation.
     */
    public void SetDefaultModel()
    {
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
    public void UpdateVulnerabilityRatio(int population, int vulnerablePopulation)
    {
        if (population != 0)
        {
            vulnerabilityRatio = (double)vulnerablePopulation / ((double)population/5);
        }
        else
        {
            vulnerabilityRatio = 0;
            Debug.Log("Vulnerability ratio is tried to be calculated while population is zero.");
        }
    }


    //TODO: think about converting activeCaseNumberOfYesterday to double.
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
        if (effectType != 1)
        {
            Debug.Log("Unknown effect type is entered for the virus model.");
            return;
        }

        if (effectType == 1)
        {
            ExecuteGeometricEvent(targetParameter, effectValue);
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
                growthRateParameter += (0.5 - growthRateParameter) * effectValue;
            }
            else
            {
                growthRateParameter += (growthRateParameter - 0.1) * effectValue;
            }
        }
        else if (targetParameter == "effectRatio")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                effectRatio += (1 - effectRatio) * effectValue;
            }
            else
            {
                effectRatio += (effectRatio - 0.1) * effectValue;
            }
        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
    }
}