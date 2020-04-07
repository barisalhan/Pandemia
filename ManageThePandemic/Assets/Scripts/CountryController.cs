using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountryController : MonoBehaviour
{
    private int population;

    private int vulnerablePopulation;

    private int quarantinedPopulation;

    private int activeCases;

    private int quarantinedActiveCases;

    private List<StateController> stateControllers;

    private EconomyModel economyModel;

    private SocietyModel societyModel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ExecuteEvent(MTPEvent mtpEvent)
    {
        throw new System.NotImplementedException();
    }
}
