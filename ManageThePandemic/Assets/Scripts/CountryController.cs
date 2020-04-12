using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ManageThePandemic/Country")]
public class CountryController : MTPScriptableObject, ITimeDrivable
{
    public List<StateController> stateControllers = new List<StateController>();

    [SerializeField]
    public int population;

    private int vulnerablePopulation;

    private int quarantinedPopulation;

    private int activeCases;

    private int quarantinedActiveCases;

    private EconomyModel economyModel;

    private SocietyModel societyModel;


    // TODO: extends this for country models.
    public void SetDefaultEnvironment()
    {
        foreach (StateController state in stateControllers)
        {
            state.SetDefaultEnvironment();
        }
    }


    public void NextDay()
    {
        foreach (StateController state in stateControllers)
        {
            state.NextDay();
        }

        UpdateFields();
    }


    public void UpdateFields()
    {
        //TODO: implement here.
    }

    public int GetPopulation()
    {
        return population;
    }

    public int GetActiveCases()
    {
        return activeCases;
    }
}

