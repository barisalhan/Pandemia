using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Pandemia/EconomyModel")]
public class EconomyModel : MTPScriptableObject
{
    public static Double[] LIMITS_EconomicSituation = { 0.0, 1.0 };
    // UNUSED!
    public static Double[] LIMITS_TaxCoefficient = { 0.0, 0.0 };
    
    private const double INITIAL_EconomicDevelopmentCoefficient = 1;
    private const double INITIAL_InputEconomicSituation = 1;

    private const double INITIAL_TaxCoefficient = 1;
    private const double INITIAL_Normalization = 1 / 2e6;

    //[Independent]
    [SerializeField]
    private double economicDevelopmentCoefficient;


    private double inputEconomicSituation;
    public double InputEconomicSituation
    {
        get { return inputEconomicSituation; }
        set
        {
            inputEconomicSituation = value;
            EconomicSituation = Sigmoid(value, LIMITS_EconomicSituation[0], 
                LIMITS_EconomicSituation[1], Temperatures.T_EconomicSituation);
        }
    }
    //[Independent]
    [SerializeField]
    private double economicSituation;
    public double EconomicSituation
    {
        get { return economicSituation; }
        set
        {
            economicSituation = value;
            OnEconomicSituationChanged();
        }
    }


    // There is no upper limit for it.
    //[Independent]
    [SerializeField]
    private double taxCoefficient;
    public double TaxCoefficient
    {
        get { return taxCoefficient; }
        set { taxCoefficient = value; }
    }


    // We assume that a million people give 1 game money as a tax.
    private double normalization;


    public EventHandler<EconomicSituationArgs> EconomicSituationChanged;

    public void SetDefaultModel()
    {
        economicDevelopmentCoefficient = INITIAL_EconomicDevelopmentCoefficient;
        InputEconomicSituation = INITIAL_InputEconomicSituation;
        taxCoefficient = INITIAL_TaxCoefficient;
        normalization = INITIAL_Normalization;
    }

    public int CalculateTax(int population)

    {
        
        int tax = (int) (population * normalization * economicDevelopmentCoefficient
                         * economicSituation * taxCoefficient);
        return tax ;
        
    }


    
    // TODO: connect this with UI.
    protected virtual void OnEconomicSituationChanged()
    {
        if(EconomicSituationChanged != null)
        {
            EconomicSituationChanged(this, new EconomicSituationArgs(economicSituation));
        }
    }

}


public class EconomicSituationArgs
{
    public double economicSituation;

    public EconomicSituationArgs(double economicSituation)
    {
        this.economicSituation = economicSituation;
    }

}
