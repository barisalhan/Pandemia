using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/State")]
public class RegionController : MTPScriptableObject, ITimeDrivable
{
    public string name;

    // Total number of alive people.
    [SerializeField]
    private int population;

    private int normalizedPopulation;

    // When multiplied with free active cases it gives probability of outbreak
    [SerializeField] 
    private double travelFlowCoeff; 

    [SerializeField]
    public int dailyTax;

    // If true, there is at least one case in the state.
    public bool isInfected;

    // If true, state is under quarantine.
    public bool isQuarantined;

    // [Day, Aggregate active case number]
    public Dictionary<int, int> activeCases = new Dictionary<int, int>();

    private int dailyNewCaseNumber;

    // Number of population who can get infected.(Population - (active cases + recovered cases))
    private int vulnerablePopulation;

    public HealthSystemModel healthSystemModel;

    public VirusModel virusModel;

    public EconomyModel economyModel;


    /*
     * Sets models to default.
     * Fills 0th day values.
     *
     * We need values of 0th day to start the  game, because in each day,
     * we use numbers of yesterday and parameters of today. 
     */
    public void SetDefaultEnvironment()
    {
        dailyNewCaseNumber = 0;
        normalizedPopulation = population / 5;
        isInfected = false;
        isQuarantined = false;

        virusModel.SetDefaultModel();
        healthSystemModel.SetDefaultModel();
        healthSystemModel.UpdateParameters();

        activeCases.Add(0, 0);
        vulnerablePopulation = population/5 - 1;
        virusModel.UpdateParameters(population, vulnerablePopulation);

        economyModel.SetDefaultModel();
    }


    // TODO: Extend this method for other models.
    public void NextDay()
    {
        virusModel.UpdateParameters(population, vulnerablePopulation);

        healthSystemModel.UpdateParameters();

        healthSystemModel.CalculateAndUpdateDailyDictionaries(dailyNewCaseNumber);

        healthSystemModel.UpdateAggregateDictionaries();

        dailyNewCaseNumber = virusModel.CalculateDailyNewCase(normalizedPopulation,
                                                              activeCases[Time.GetInstance().GetDay() - 1]);
        dailyTax = economyModel.CalculateTax(population);
        
        UpdateFields();
        //Debug.Log(name + " " + activeCases[Time.GetInstance().GetDay()-1]);
        Debug.Log(name + " " + isInfected + " Total cases: " + activeCases[Time.GetInstance().GetDay()] );
    }


    public void UpdateFields()
    {
        CalculateActiveCaseNumber();
        UpdateVulnerablePopulation();
        
        if (activeCases[Time.GetInstance().GetDay()] > 0)
        {
            isInfected = true;
        }
    }

    public void ExecuteEvents()
    {

    }

    //TODO: Use effective population. Consider death and recoveries. 
    void UpdateVulnerablePopulation()
    {
        int today = Time.GetInstance().GetDay();
        vulnerablePopulation = normalizedPopulation - activeCases[today] - healthSystemModel.aggregateDeathCases[today]
                               - healthSystemModel.aggregateRecoveredCases[today];
    }


    /*
     * Preconditions:
     *  - Daily new case number is calculated for today.
     */
    public void CalculateActiveCaseNumber()
    {
        int today = Time.GetInstance().GetDay();
        activeCases[today] = activeCases[today - 1] + dailyNewCaseNumber;
        activeCases[today] -= (healthSystemModel.GetNewRecoveredCases(today) + healthSystemModel.GetNewDeathCases(today));
    }

    /*
     * If region is not infected, this function is called for deciding outbreak in this region.
     */
    public void InfectRegion(int unquarantinedActiveCases)
    {
        double outbreakProbability = travelFlowCoeff * unquarantinedActiveCases;
        int today = Time.GetInstance().GetDay();

        //TODO: Create global static random number generator class like Time.
        System.Random rnd = new System.Random();
        bool result = rnd.NextDouble() <= outbreakProbability ? true : false;

        if (result)
        {
            isInfected = true;
            activeCases[today] = 1;
        }
        
    }


    /* Returns the active case number of yesterday. */
    public int GetActiveCases()
    {
        return activeCases[Time.GetInstance().GetDay() - 1];
    }

    /* Returns the population number of yesterday. */
    public int GetPopulation()
    {
        return population;
    }


}