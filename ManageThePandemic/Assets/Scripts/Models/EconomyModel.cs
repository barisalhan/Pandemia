using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "ManageThePandemic/EconomyModel")]
public class EconomyModel : MTPScriptableObject
{
    //[Independent]
    [SerializeField]
    private double economicDevelopmentCoefficient;

    //[Independent]
    [SerializeField]
    private double economicSituation = 1;

    // There is no upper limit for it.
    //[Independent]
    private double taxCoefficient = 1;


    // We assume that a million people give 1 game money as a tax.
    private double normalization = 1/1e6;


    public EventHandler<EconomicSituationArgs> EconomicSituationChanged;

    public int CalculateTax(int population, bool isQuarantined)
    {
        return (int) (population * normalization * economicDevelopmentCoefficient 
                      * economicSituation * taxCoefficient);
    }


    public void ExecuteEvent(string targetParameter,
                             int effectType,
                             double effectValue)
    {
        if (effectType != 1)
        {
            Debug.Log("Unknown effect type is entered for the economy model.");
            return;
        }

        ExecuteGeometricEvent(targetParameter, effectValue);
    }


    private void ExecuteGeometricEvent(string targetParameter,
                                      double effectValue)
    {
        if (targetParameter == "economicSituation")
        {
            Debug.Log("Executing a geometric event in virus model.");
            if (effectValue > 0)
            {
                economicSituation += (1.0 - economicSituation) * effectValue;
            }
            else
            {
                economicSituation += (economicSituation - 0.0) * effectValue;
            }

            OnEconomicSituationChanged();
        }
        else if (targetParameter == "taxCoefficient")
        {
            Debug.Log("Executing a geometric event in virus model.");
            taxCoefficient *= effectValue;
            if (effectValue < 0)
            {
                Debug.Log("Negative taxCoefficient is given. It is invalid");
            }

        }
        else
        {
            Debug.Log("Unknown parameter type is entered.");
        }
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
