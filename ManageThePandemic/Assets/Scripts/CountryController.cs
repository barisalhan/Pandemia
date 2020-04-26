using System;
using System.Collections.Generic;
using UnityEngine;


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
        foreach (RegionController region in regionControllers)
        { 
            region.SetDefaultEnvironment();
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


    public void NextDay()
    {
        foreach (RegionController region in regionControllers)
        {
            region.NextDay();
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

