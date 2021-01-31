using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pandemia/State")]
public class RegionController : MTPScriptableObject, ITimeDrivable
{
    [SerializeField]
    public string name;

    // Total number of alive people.
    [SerializeField]
    private int population;

    private int normalizedPopulation;

    private double normalizationCoefficent = 5;

    // When multiplied with free active cases it gives probability of outbreak
    [SerializeField] 
    private double travelFlowCoeff; 

    [SerializeField]
    public int dailyTax;

    // If true, there is at least one case in the state.
    public bool isInfected;

    // If true, state is under quarantine.
    public bool isQuarantined;

    // [Day, Aggregate active case number]
    public Dictionary<int, int> activeCases = new Dictionary<int, int>();

    private int dailyNewCaseNumber;

    // Number of population who can get infected.(Population - (active cases + recovered cases))
    private int vulnerablePopulation;


    [SerializeField]
    private HealthSystemModel healthSystemModel;
    public HealthSystemModel HealthSystemModel
    {
        get { return healthSystemModel; }
        set { healthSystemModel = value; }
    }


    [SerializeField]
    private VirusModel virusModel;
    public VirusModel VirusModel
    {
        get { return virusModel; }
        set { virusModel = value; }
    }

    [SerializeField]
    private EconomyModel economyModel;
    public EconomyModel EconomyModel
    {
        get { return economyModel; }
        set { economyModel = value; }
    }

    [SerializeField]
    private SocietyModel societyModel;
    public SocietyModel SocietyModel
    {
        get { return societyModel; }
        set { societyModel = value; }
    }


    /*
     * Sets models to default.
     * Fills 0th day values.
     *
     * We need values of 0th day to start the  game, because in each day,
     * we use numbers of yesterday and parameters of today. 
     */
    public void SetDefaultEnvironment()
    {
        dailyNewCaseNumber = 0;
        normalizedPopulation = (int)(population / normalizationCoefficent);
        isInfected = false;
        isQuarantined = false;

        virusModel.SetDefaultModel();
        healthSystemModel.SetDefaultModel();
        healthSystemModel.UpdateParameters();

        activeCases.Add(0, 0);
        vulnerablePopulation = normalizedPopulation;
        virusModel.UpdateParameters(population, vulnerablePopulation);

        economyModel.SetDefaultModel();

        societyModel.SetDefaultModel();
    }


    // TODO: Extend this method for other models.
    public void NextDay()
    {
        virusModel.UpdateParameters(normalizedPopulation, vulnerablePopulation);

        healthSystemModel.UpdateParameters();

        healthSystemModel.CalculateAndUpdateDailyDictionaries(dailyNewCaseNumber);

        healthSystemModel.UpdateAggregateDictionaries();

        dailyNewCaseNumber = virusModel.CalculateDailyNewCase(normalizedPopulation,
                                                              activeCases[Time.GetInstance().GetDay() - 1]);
        Debug.Log(this.name + "right after it is calculated:" + dailyNewCaseNumber);
        dailyTax = economyModel.CalculateTax(population);
        
        UpdateFields();
        //Debug.Log(name + " " + activeCases[Time.GetInstance().GetDay()-1]);
        //Debug.Log(name + " " + isInfected + " Total cases: " + activeCases[Time.GetInstance().GetDay()] );
    }


    public void UpdateFields()
    {
        CalculateActiveCaseNumber();
        UpdateVulnerablePopulation();
        
        if (activeCases[Time.GetInstance().GetDay()] > 0)
        {
            isInfected = true;
        }
    }

    public void ExecuteEvents()
    {

    }

    //TODO: Use effective population. Consider death and recoveries. 
    void UpdateVulnerablePopulation()
    {
        int today = Time.GetInstance().GetDay();
        vulnerablePopulation = normalizedPopulation - activeCases[today] - healthSystemModel.aggregateDeathCases[today]
                               - healthSystemModel.aggregateRecoveredCases[today];
    }


    /*
     * Preconditions:
     *  - Daily new case number is calculated for today.
     */
    public void CalculateActiveCaseNumber()
    {
        int today = Time.GetInstance().GetDay();
        activeCases[today] = activeCases[today - 1] + dailyNewCaseNumber;
        activeCases[today] -= (healthSystemModel.GetNewRecoveredCases(today) + healthSystemModel.GetNewDeathCases(today));
    }

    /*
     * If region is not infected, this function is called for deciding outbreak in this region.
     */
    public void InfectRegion(int unquarantinedActiveCases)
    {
        double outbreakProbability = travelFlowCoeff * unquarantinedActiveCases;
        int today = Time.GetInstance().GetDay();

        //TODO: Create global static random number generator class like Time.
        System.Random rnd = new System.Random();
        bool result = rnd.NextDouble() <= outbreakProbability ? true : false;

        if (result)
        {
            isInfected = true;
            activeCases[today] = 1;
        }
        
    }


    /* Returns the active case number of yesterday. */
    public int GetActiveCases()
    {
        return activeCases[Time.GetInstance().GetDay() - 1];
    }

    /* Returns the population number of yesterday. */
    public int GetPopulation()
    {
        return population;
    }


}