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

    // If true, there is at least one case in the state.
    private bool isInfected;

    // The probability for virus to emerge in the state, \
    // if the state is not infected.
    public double outbreakProbability;

    // If true, state is under quarantine.
    private bool isQuarantined;

    // [Day, Aggregate active case number]
    public Dictionary<int, int> activeCases = new Dictionary<int, int>();

    private int dailyNewCaseNumber;

    // Number of population who can get infected.(Population - (active cases + recovered cases))
    private int vulnerablePopulation;

    public HealthSystemModel healthSystemModel;

    public VirusModel virusModel;


    // TODO: extend here for all models.
    public void SetDefaultEnvironment()
    {
        dailyNewCaseNumber = 1;

        virusModel.SetDefaultModel();
        healthSystemModel.SetDefaultModel();
        healthSystemModel.UpdateParameters();

        activeCases.Add(0, 1);
        vulnerablePopulation = population - 1;
        virusModel.UpdateParameters(population, vulnerablePopulation);
    }


    // TODO: Extend this method for other models.
    public void NextDay()
    {

        virusModel.UpdateParameters(population, vulnerablePopulation);

        healthSystemModel.UpdateParameters();

        healthSystemModel.CalculateAndUpdateDailyDictionaries(dailyNewCaseNumber);

        healthSystemModel.UpdateAggregateDictionaries();

        dailyNewCaseNumber = virusModel.CalculateDailyNewCase((population / 100) * 20,
                                                              activeCases[Time.GetInstance().GetDay() - 1]);

        UpdateFields();
    }


    public void UpdateFields()
    {
        CalculateActiveCaseNumber();
        UpdateVulnerablePopulation();
    }

    public void ExecuteEvents()
    {

    }

    void UpdateVulnerablePopulation()
    {
        vulnerablePopulation = population - activeCases[Time.GetInstance().GetDay()];
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