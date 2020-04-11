using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.Audio.Google;
using Object = System.Object;

[CreateAssetMenu(menuName = "ManageThePandemic/VirusModel")]
public class VirusModel : ScriptableObject, ITimeDrivable
{
    //[Independent]
    // Daily number of the contacted people of a person
    public double averageNumberOfContactedPeople;

    //[Dependent]
    // Ratio of population who can be get infected. [vulnerable population / population]
    public double vulnerabilityRatio;

    //[Independent]
    // Probability of transmitting to a vulnerable person through physical contact.
    public double probabilityOfTransmittingInfection;

    //[Dependent]
    // It will multiplied with the activeCaseNumber.
    // The result is the new dailyCaseNumber;
    public double growthRateParameter;

    // [Day, Aggregate active case number]
    public Dictionary<int, int> activeCaseNumbers = new Dictionary<int, int>();

    // [Day, new case number on that day]
    private Dictionary<int, int> dailyCaseNumbers = new Dictionary<int, int>();

    // [Day, Growth Rate on that day]
    private Dictionary<int, double> growthRates = new Dictionary<int, double>();


    //TODO: make it safer.
    /*
     * Sets the value of an independent parameter.
     * Parameters:
     *  - averageNumberOfContactedPeople
     *  - probabilityOfTransmittingInfection
     */
    public void SetParameter(string parameterName, double value)
    {
        Type type = typeof(VirusModel);
        Object instance = this;

        FieldInfo fieldInfo= type.GetField(parameterName);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(instance, value);
        }
        else
        {
            Debug.Log("Non-existing field is tried to be reached.");
        }
    }


    /*
     * Updates the dependent parameters.
     */
    public void UpdateParameters(int population, int vulnerablePopulation)
    {
        CalculateGrowthRateParameter(population, vulnerablePopulation);
    }


    // TODO: death populationdan dusuyor.
    public void CalculateGrowthRateParameter(int population, int vulnerablePopulation)
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

        growthRateParameter =
            (averageNumberOfContactedPeople * vulnerabilityRatio) * probabilityOfTransmittingInfection;
    }


    // TODO: Think about creating a super-class.
    public void SetDefaultModel()
    {
        int day = Time.GetInstance().GetDay();

        activeCaseNumbers.Add(day, 5);
        dailyCaseNumbers.Add(day, 5);
        // TODO: change this
        growthRates.Add(day,5);
    }

    // TODO: Think about creating a super-class.
    public void NextDay()
    {
        int day = Time.GetInstance().GetDay();

        activeCaseNumbers.Add(day, activeCaseNumbers[day-1]*2);        
    }
}
