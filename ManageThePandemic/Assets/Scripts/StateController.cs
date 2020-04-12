using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/State")]
public class StateController : MTPScriptableObject, ITimeDrivable
{ 
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

    // TODO: think about converting this to daily number.
    // Number of total people who died because of infection up to that day.
    private Dictionary<int, int> aggreagateDeathCases = new Dictionary<int, int>();

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
        virusModel.SetDefaultModel();

        activeCases.Add(0, 1);
        vulnerablePopulation = population - 1;
        virusModel.UpdateParameters(population, vulnerablePopulation);
    }


    // TODO: Extend this method for other models.
    public void NextDay()
    {
        virusModel.UpdateParameters(population, vulnerablePopulation);
        dailyNewCaseNumber = virusModel.CalculateDailyNewCase(activeCases[Time.GetInstance().GetDay() - 1]);

        UpdateFields();
    }


    public void UpdateFields()
    { 
        CalculateActiveCaseNumber();
        UpdateVulnerablePopulation();
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
        //TODO: add recovered and death people
        activeCases[today] = activeCases[today - 1] + dailyNewCaseNumber;
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
