using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "ManageThePandemic/EconomyModel")]
public class EconomyModel : MTPScriptableObject
{
    private const double MAX_ECONOMIC_SITUATION = 1;
    private const double MIN_ECONOMIC_SITUATION = 0;

    //[Independent]
    [SerializeField]
    private double economicDevelopmentCoefficient;

    //[Independent]
    [SerializeField]
    private double economicSituation;

    // There is no upper limit for it.
    //[Independent]
    [SerializeField]
    private double taxCoefficient;


    // We assume that a million people give 1 game money as a tax.
    private double normalization;


    public EventHandler<EconomicSituationArgs> EconomicSituationChanged;

    public void SetDefaultModel()
    {
        economicDevelopmentCoefficient = 1;
        economicSituation = 1;
        taxCoefficient = 1;
        normalization = 2 / 1e7;
    }

    public int CalculateTax(int population)

    {
        
        int tax = (int) (population * normalization * economicDevelopmentCoefficient
                         * economicSituation * taxCoefficient);
        return tax ;
        
    }


    public void ExecuteEvent(string targetParameter,
                             int effectType,
                             double effectValue)
    {
        if (effectType != 1 && effectType !=3)
        {
            Debug.Log("Unknown effect type is entered for the economy model.");
            return;
        }

        if (effectType == 1)
        {
            ExecuteGeometricEvent(targetParameter, effectValue);
        }

        if (effectType == 3)
        {
            ExecuteReverseEvent(targetParameter, effectValue);
        }

    }

    private void ExecuteGeometricEvent(string targetParameter,
                                      double effectValue)
    {
        if (targetParameter == "economicSituation")
        {
            Debug.Log("Executing a geometric event in economy model.");
            if (effectValue > 0)
            {
                economicSituation += (MAX_ECONOMIC_SITUATION - economicSituation) * effectValue;
            }
            else
            {
                economicSituation += (economicSituation - MIN_ECONOMIC_SITUATION) * effectValue;
            }

            OnEconomicSituationChanged();
        }
        else if (targetParameter == "taxCoefficient")
        {
            Debug.Log("Executing a geometric event in economy model.");
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

    private void ExecuteReverseEvent(string targetParameter, double effectValue)
    {
        if (targetParameter == "economicSituation")
        {
            Debug.Log("Executing a reverse geometric event in economy model.");
            if (effectValue > 0)
            {
                double nominator = economicSituation - effectValue * MAX_ECONOMIC_SITUATION;
                double denominator = 1 - effectValue;
                economicSituation = nominator / denominator;
            }
            else
            {
                double nominator = economicSituation + effectValue * MIN_ECONOMIC_SITUATION;
                double denominator = 1 + effectValue;
                economicSituation = nominator / denominator;
            }

            OnEconomicSituationChanged();
        }
        else if (targetParameter == "taxCoefficient")
        {
            Debug.Log("Executing a reverse geometric event in economy model.");
            taxCoefficient /= effectValue;
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
}


public class EconomicSituationArgs
{
    public double economicSituation;

    public EconomicSituationArgs(double economicSituation)
    {
        this.economicSituation = economicSituation;
    }

}
