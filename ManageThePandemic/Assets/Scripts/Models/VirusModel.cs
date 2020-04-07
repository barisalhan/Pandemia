using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Audio.Google;

[CreateAssetMenu(menuName = "ManageThePandemic/VirusModel")]
public class VirusModel : ScriptableObject
{
    public int day = 0;
    // [Day, Aggregate active case number]
    public Dictionary<int, int> activeCaseNumber = new Dictionary<int, int>();
    // [Day, new case number on that day]
    private Dictionary<int, int> dailyCaseNumber = new Dictionary<int, int>();
    // [Day, Growth Rate on that day]
    private Dictionary<int, double> growthRate = new Dictionary<int, double>();

    public double averageNumberOfContactedPeople;
    public double probabilityOfTransmittingInfection;
    public double vulnerabilityRatio;

    // It will multiplied with the activeCaseNumber.
    // The result is the new dailyCaseNumber;
    public double growthRateParameter;

    public void CalculateGrowthRateParameter()
    {
        growthRateParameter = averageNumberOfContactedPeople *
                              probabilityOfTransmittingInfection;
    }

    // TODO: Think about creating a super-class.
    public void SetDefaultModel()
    {
        day = 1;
        // TODO: parametrize the day
        activeCaseNumber.Add(day, 5);
        dailyCaseNumber.Add(day, 5);
        // TODO: change this
        growthRate.Add(day,5);
    }

    public void Update()
    {
        day++;
        activeCaseNumber.Add(day, activeCaseNumber[day-1]*2);        
    }
}
