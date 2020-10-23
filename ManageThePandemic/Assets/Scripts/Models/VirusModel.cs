using System;
using System.Collections.Generic;
using UnityEngine;


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
[CreateAssetMenu(menuName = "Pandemia/VirusModel")]
public class VirusModel : MTPScriptableObject
{
    public static Double[] LIMITS_GrowthRateParameter = { 0.5, 0.8 };
    public static Double[] LIMITS_EffectRatio = { 0.25, 0.55 };

    private const double INITIAL_InputGrowthRateParameter = 0;
    private const double INITIAL_InputEffectRatio = 0;


    // [Dependent]
    // Ratio of population who can be get infected. [vulnerable population / Normalized population]
    [SerializeField]
    private double vulnerabilityRatio;



    private double inputGrowthRateParameter;
    public double InputGrowthRateParameter
    {
        get { return inputGrowthRateParameter; }
        set
        {
            inputGrowthRateParameter = value;
            GrowthRateParameter = Sigmoid(value,
                                LIMITS_GrowthRateParameter[0],
                                LIMITS_GrowthRateParameter[1],
                               Temperatures.T_GrowthRateParameter);
        }
    }
    // TODO: It is not parameter. Change its name both from here and from actions.
    // [Dependent]
    [SerializeField]
    [Help("GrowthRateParameter : 0.4 - 0.7")]
    private double growthRateParameter;
    public double GrowthRateParameter
    {
        get { return growthRateParameter; }
        set { growthRateParameter = value; }
    }



    private double inputEffectRatio;
    public double InputEffectRatio
    {
        get { return inputGrowthRateParameter; }
        set
        {
            inputEffectRatio = value;
            EffectRatio = Sigmoid(value, LIMITS_EffectRatio[0],
                LIMITS_EffectRatio[1], Temperatures.T_EffectRatio);
        }
    }
    
    // [Independent]    
    // Ratio of population who is active in daily life.
    [SerializeField]
    [Help("Effect Ratio : 0.25 - 0.55")]
    private double effectRatio;
    public double EffectRatio
    {
        get { return effectRatio; }
        set { effectRatio = value; }
    }




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
        InputGrowthRateParameter = INITIAL_InputGrowthRateParameter;
        InputEffectRatio = INITIAL_InputEffectRatio;
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

}