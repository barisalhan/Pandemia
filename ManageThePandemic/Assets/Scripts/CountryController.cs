using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


[CreateAssetMenu(menuName = "ManageThePandemic/Country")]
public class CountryController : MTPScriptableObject, ITimeDrivable
{
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

    public int population;

    private int vulnerablePopulation;

    private int quarantinedPopulation;

    private int activeCases;

    private int unquarantinedActiveCases;


    public int happiness;

    [SerializeField]
    private int totalBudget;

    public SocietyModel societyModel;


    // TODO: extends this for country models.
    public void SetDefaultEnvironment()
    {
        population = 0;
        unquarantinedActiveCases = 0;
        activeCases = 0;

        //TODO: Find an error prone method.
        Random rnd = new Random();
        int regionIndex = rnd.Next(0, 3);

        CreateIndexTable();
        foreach (RegionController region in regionControllers)
        { 
            region.SetDefaultEnvironment();
            population += region.GetPopulation();
            unquarantinedActiveCases += region.GetActiveCases();
            activeCases += region.GetActiveCases();
        }

        regionControllers[regionIndex].activeCases[0] += 1;
        
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

        foreach (RegionController region in regionControllers)
        {
            if (!region.isInfected)
            {
                region.getInfected(unquarantinedActiveCases);
            }
        }
        UpdateFields();
    }


    public void UpdateFields()
    {
        //TODO: implement here.
        happiness = societyModel.CalculateHappiness();
    }

    public int GetPopulation()
    {
        return population;
    }

    public int GetActiveCases()
    {
        return activeCases;
    }

    public RegionController GetRegion(Name regionName)
    {
        int index = indexTable[regionName];
        return regionControllers[index];
    }

    //TODO: remove
    public RegionController GetRegionController(string regionName)
    {
        CountryController.Name currentRegion;
        Enum.TryParse<CountryController.Name>(regionName, out currentRegion);
        RegionController regionController = GetRegion(currentRegion);
        Debug.Log(regionController.GetPopulation());
        return regionController;
    }
}

