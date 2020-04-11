using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/State")]
public class StateController : ScriptableObject, ITimeDrivable
{ 
    // Total number of alive people.
    [SerializeField]
    private int population;

    // TODO: think about converting this to daily number.
    // Number of total people who died because of infection up to that day.
    private Dictionary<int, int> AggreagateDeathCases = new Dictionary<int, int>();

    // Number of population who can get infected.(Population - (active cases + recovered cases))
    private int vulnerablePopulation;

    // If true, there is at least one case in the state.
    private bool isInfected;

    // The probability for virus to emerge in the state, \
    // if the state is not infected.
    public double outbreakProbability;

    // If true, state is under quarantine.
    private bool isQuarantined;

    public HealthSystemModel healthSystemModel;

    public VirusModel virusModel;


    public int GetPopulation()
    {
        return population;
    }
    public int GetActiveCases()
    {
        return virusModel.activeCaseNumbers[Time.GetInstance().GetDay()];
    }

    public void NextDay()
    {
        virusModel.SetParameter("averageNumberOfContactedPeople", 12);
        virusModel.UpdateParameters(population, vulnerablePopulation);
        virusModel.NextDay();
    }
}
