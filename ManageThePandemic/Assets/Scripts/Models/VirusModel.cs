using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.Audio.Google;
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
[CreateAssetMenu(menuName = "ManageThePandemic/VirusModel")]
public class VirusModel : MTPScriptableObject
{
    //[Independent]
    // Daily number of the contacted people of a person
    [SerializeField]
    protected double averageNumberOfContactedPeople;

    // [Dependent]
    // Ratio of population who can be get infected. [vulnerable population / population]
    [SerializeField]
    private double vulnerabilityRatio;

    // [Independent]
    // Probability of transmitting to a vulnerable person through physical contact.
    [SerializeField]
    protected double probabilityOfTransmittingInfection;

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
        UpdateGrowthRateParameter();
        UpdateVulnerabilityRatio(population, vulnerablePopulation);
    }


    /*
     * Updates the value, according to death and recovered numbers of yesterday.
     */
    public void UpdateVulnerabilityRatio(int population, int vulnerablePopulation)
    {
        if (population != 0)
        {
            vulnerabilityRatio = (double) vulnerablePopulation / (double) population;
        }
        else
        {
            vulnerabilityRatio = 0;
            Debug.Log("Vulnerability ratio is tried to be calculated while population is zero.");
        }
    }


    /*
     * Preconditions:
     *  - VulnerabilityRatio is updated.
     */
    public void UpdateGrowthRateParameter()
    {
        growthRateParameter =
            (averageNumberOfContactedPeople * vulnerabilityRatio) * probabilityOfTransmittingInfection;
    }


    /*
     * Preconditions:
     *  - Parameters are adjusted according to events of today.
     * TODO: error-prone method.
     */
    public int CalculateDailyNewCase(int activeCaseNumberOfYesterday)
    {
        int today = Time.GetInstance().GetDay();

        dailyNewCaseNumbers[today] = (int)(growthRateParameter * activeCaseNumberOfYesterday); ;

        return dailyNewCaseNumbers[today];
    }
}