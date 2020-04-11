using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/State")]
public class StateController : ScriptableObject, ITimeDrivable
{ 
    [SerializeField]
    private int population;

    private int vulnerablePopulation;

    private bool isInfected;

    private bool isQuarantined;

    public double outbreakProbability;

    public HealthSystemModel healthSystemModel;

    public VirusModel virusModel;


    public int GetPopulation()
    {
        return population;
    }
    public int GetActiveCases()
    {
        return virusModel.activeCaseNumber[Time.GetInstance().GetDay()];
    }

    public void NextDay()
    {
        virusModel.NextDay();
    }
}
