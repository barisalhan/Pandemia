using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ManageThePandemic/Country")]
public class CountryController : MTPScriptableObject, ITimeDrivable
{
    public List<StateController> stateControllers = new List<StateController>();

    public Dictionary<Name, int> indexTable = new Dictionary<Name, int>();

    public enum Name
    {
        Arizona,
        California,
        NewMexico
    }

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
        CreateIndexTable();
        foreach (StateController state in stateControllers)
        { 
            state.SetDefaultEnvironment();
        }
    }

    public void CreateIndexTable()
    {
        for (int index = 0; index < stateControllers.Count; index++)
        {
            Name currentState;
            if (Enum.TryParse<Name>(stateControllers[index].name, out currentState))
            {
                indexTable.Add(currentState, index);
            }
            else
            {
                Debug.Log("There is an inconsistency between stateName and Name enum list. " +
                          stateControllers[index].name + " is tried to be reached");
            }
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

    public StateController GetState(Name stateName)
    {
        int index = indexTable[stateName];
        return stateControllers[index];
    }
}

