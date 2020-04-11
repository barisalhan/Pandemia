using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Audio.Google;

[CreateAssetMenu(menuName = "ManageThePandemic/VirusModel")]
public class VirusModel : ScriptableObject
{
    private int day;
    // [Day, Aggregate active case number]
    public Dictionary<int, int> activeCaseNumber = new Dictionary<int, int>();

    // It will multiplied with the activeCaseNumber.
    // The result is the new dailyCaseNumber;
    public double growthRateParameter;

    // [Day, new case number on that day]
    private Dictionary<int, int> dailyCaseNumber = new Dictionary<int, int>();

    // [Day, Growth Rate on that day]
    private Dictionary<int, double> growthRate = new Dictionary<int, double>();

    public double averageNumberOfContactedPeople;

    public double probabilityOfTransmittingInfection;

    public double vulnerabilityRatio;

    

    public void CalculateGrowthRateParameter()
    {
        growthRateParameter = averageNumberOfContactedPeople *
                              probabilityOfTransmittingInfection;
    }

    // TODO: Think about creating a super-class.
    public void SetDefaultModel()
    {
        day = Time.GetInstance().GetDay();

        activeCaseNumber.Add(day, 5);
        dailyCaseNumber.Add(day, 5);
        // TODO: change this
        growthRate.Add(day,5);
    }

    public void Update()
    {
        day = Time.GetInstance().GetDay();

        activeCaseNumber.Add(day, activeCaseNumber[day-1]*2);        
    }
}
