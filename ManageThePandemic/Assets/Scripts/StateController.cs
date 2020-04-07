using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private int population;

    private int vulnerablePopulation;

    private bool isInfected;

    private bool isQuarantined;

    private double outbreakProbability;

    private HealthSystemModel healthSystemModel;

    private VirusModel virusModel;

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
