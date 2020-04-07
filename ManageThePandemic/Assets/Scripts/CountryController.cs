using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ManageThePandemic/Country")]
public class CountryController : ScriptableObject
{
    public List<StateController> stateControllers = new List<StateController>();

    private int population;

    private int vulnerablePopulation;

    private int quarantinedPopulation;

    private int activeCases;

    private int quarantinedActiveCases;

    private EconomyModel economyModel;

    private SocietyModel societyModel;
}

