using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


[CreateAssetMenu(menuName = "ManageThePandemic/Country")]
public class CountryController : MTPScriptableObject, ITimeDrivable
{
    [SerializeField]
    public List<RegionController> regionControllers = new List<RegionController>();

    public Dictionary<Name, int> indexTable = new Dictionary<Name, int>();

    public enum Name
    {
        NorthEast,
        SouthEast,
        SouthWest,
        MidWest,
        NorthWest,
        West
    }

    private int population = 0;

    private int activeCases = 0;

    private int unquarantinedActiveCases = 0;

    [SerializeField]
    public SocietyModel societyModel;

    private int happiness;

    [SerializeField]
    private int totalBudget;

    public EventHandler<BudgetArgs> BudgetChanged;

    public void ChangeBudget(int cost)
    {
        totalBudget -= cost;
        OnBudgetChanged();
    }

    protected virtual void OnBudgetChanged()
    {
        Debug.Log("Budget is changed.");
        if (BudgetChanged != null)
        {
            //WARNING: PERFORMANCE COULD BE IMPROVED!
            BudgetChanged(this, new BudgetArgs(totalBudget));
        }
    }


    public void SetDefaultEnvironment()
    {
        CreateIndexTable();

        foreach (RegionController region in regionControllers)
        { 
            region.SetDefaultEnvironment();

            population += region.GetPopulation();
            unquarantinedActiveCases += region.GetActiveCases();
            activeCases += region.GetActiveCases();
        }

        CreateFirstOutbreak();
    }


    public void NextDay()
    {
        unquarantinedActiveCases = 0;
        activeCases = 0;

        foreach (RegionController region in regionControllers)
        {
            region.NextDay();

            totalBudget += region.dailyTax;
            activeCases += region.GetActiveCases();

            if (!region.isQuarantined)
            {
                unquarantinedActiveCases += region.GetActiveCases();
            }
        }

        OnBudgetChanged();

        // Precondition: Total unquarantined active cases must be calculated.
        foreach (RegionController region in regionControllers)
        {
            if (!region.isInfected)
            {
                region.InfectRegion(unquarantinedActiveCases);
            }
        }
       
        happiness = societyModel.CalculateHappiness();
    }


    public void CreateIndexTable()
    {
        for (int index = 0; index < regionControllers.Count; index++)
        {
            Name currentRegion;
            if (Enum.TryParse<Name>(regionControllers[index].name, out currentRegion))
            {
                indexTable.Add(currentRegion, index);
            }
            else
            {
                Debug.Log("There is an inconsistency between regionName and Name enum list. " +
                          regionControllers[index].name + " is tried to be reached");
            }
        }
    }


    private void CreateFirstOutbreak()
    {
        //TODO: Find an error prone method.
        Random rnd = new Random();

        // The first three region is the most populated ones.
        // Therefore, virus firstly outbreaks only in these regions.
        int regionIndex = rnd.Next(0, 3);

        regionControllers[regionIndex].activeCases[0] += 1;
    }


    public void UpdateFields()
    { 
        
    }


    public int GetBudget()
    {
        return totalBudget;
    }


    public RegionController GetRegionByString(string regionName)
    {
        CountryController.Name currentRegion;
        Enum.TryParse<CountryController.Name>(regionName, out currentRegion);
        RegionController regionController = GetRegionByName(currentRegion);
        return regionController;
    }


    public RegionController GetRegionByName(Name regionName)
    {
        int index = indexTable[regionName];
        return regionControllers[index];
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


public class BudgetArgs : EventArgs
{
    public int budget { get; set; }

    public BudgetArgs(int budget)
    {
        this.budget = budget;
    }
}