using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


[CreateAssetMenu(menuName = "Pandemia/Country")]
public class CountryController : MTPScriptableObject, ITimeDrivable
{
    [SerializeField]
    public List<RegionController> regionControllers = new List<RegionController>();

    [SerializeField]
    private const int INITIAL_BUDGET = 80;
    
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


    private double avgHappiness;
    private double avgEconomicWellBeing;
    private double avgPersonalWellBeing;

    [SerializeField]
    private int totalBudget;

    public EventHandler<BudgetArgs> BudgetChanged;

    public EventHandler<AvgHappinessArgs> AvgHappinessChanged;

    public int dailyTax;

    public void AddMoney(int gain)
    {
        totalBudget += gain;
        OnBudgetChanged();
    }

    public void SpendMoney(int cost)
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
        AddMoney(INITIAL_BUDGET);

        CreateIndexTable();

        foreach (RegionController region in regionControllers)
        {
            region.SocietyModel.HappinessChanged += OnHappinessChanged;

            region.SetDefaultEnvironment();

            population += region.GetPopulation();
            unquarantinedActiveCases += region.GetActiveCases();
            activeCases += region.GetActiveCases();
        }

        CreateFirstOutbreak();
    }


    public void OnHappinessChanged(object source, EventArgs args)
    {
        CalculateAvgHappiness();
        SetAvgHappiness(avgEconomicWellBeing, avgPersonalWellBeing);
        // Barlara bagla
    }


    public void CalculateAvgHappiness()
    {
        double sumOfEconomicWellBeing = 0;
        double sumOfPersonalWellBeing = 0;

        foreach (var region in regionControllers)
        {
           sumOfEconomicWellBeing += region.SocietyModel.EconomicWellBeing;
           sumOfPersonalWellBeing += region.SocietyModel.PersonalWellBeing;
        }

        avgEconomicWellBeing = sumOfEconomicWellBeing / regionControllers.Count;
        avgPersonalWellBeing = sumOfPersonalWellBeing / regionControllers.Count;
    }


    public void SetAvgHappiness(double avgEconomicWellBeing, double avgPersonalWellBeing)
    {
        avgHappiness = Math.Min(avgEconomicWellBeing, avgPersonalWellBeing);

        AvgHappinessArgs.Cause cause;
        if (avgEconomicWellBeing < avgPersonalWellBeing)
        {
            cause = AvgHappinessArgs.Cause.economicWellBeing;
        }
        else
        {
            cause = AvgHappinessArgs.Cause.personalWellBeing;
        }

        OnAvgHappinessChanged(avgHappiness, cause);
    }


    public void OnAvgHappinessChanged(double avgHappiness, AvgHappinessArgs.Cause cause)
    {
        if (AvgHappinessChanged != null)
        {
            AvgHappinessChanged(this, new AvgHappinessArgs(avgHappiness, cause));
        }
    }

    public void NextDay()
    {
        unquarantinedActiveCases = 0;
        activeCases = 0;

        dailyTax = 0;
        foreach (RegionController region in regionControllers)
        {
            region.NextDay();
            //Debug.Log("Daily Tax: "+region.name+" "+ region.dailyTax.ToString());
            dailyTax += region.dailyTax;
            activeCases += region.GetActiveCases();

            if (!region.isQuarantined)
            {
                unquarantinedActiveCases += region.GetActiveCases();
            }
        }

        // Precondition: Total unquarantined active cases must be calculated.
        foreach (RegionController region in regionControllers)
        {
            if (!region.isInfected)
            {
                region.InfectRegion(unquarantinedActiveCases);
            }
        }
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

    public int GetRecoveredCases()
    {
        int today = Time.GetInstance().GetDay();
        int result = 0;
        foreach (var regionController in regionControllers)
        {
            result += regionController.HealthSystemModel.aggregateRecoveredCases[today];
        }
        return result;
    }


    /*
     * Precondition: NextDay is called beforehand. Otherwise, it lags
     *               the real data with one day.
     */
    public int GetDeathCases()
    {
        int today = Time.GetInstance().GetDay();
        int result = 0;
        foreach (var regionController in regionControllers)
        {
            result += regionController.HealthSystemModel.aggregateDeathCases[today - 1];
        }
        return result;
    }

    public double getAvgHappiness()
    {
        return avgHappiness;
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

public class AvgHappinessArgs
{
    [HideInInspector]
    public double value;

    [HideInInspector]
    public enum Cause
    {
        economicWellBeing,
        personalWellBeing
    }

    [HideInInspector]
    public Cause cause;

    public AvgHappinessArgs(double value, Cause cause)
    {
        this.value = value;
        this.cause = cause;
    }
}