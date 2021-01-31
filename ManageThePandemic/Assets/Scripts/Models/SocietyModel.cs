using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Pandemia/SocietyModel")]
public class SocietyModel : MTPScriptableObject
{
    private const double INITIAL_InputEconomicWellBeing = 12;
    private const double INITIAL_InputPersonalWellBeing = 12;

    public static Double[] LIMITS_EconomicWellBeing = {0.0, 1.0};
    public static Double[] LIMITS_PersonalWellBeing = { 0.0, 1.0 };

    /*
    public double T_EconomicWellBeing = 2;
    public double T_PersonalWellBeing = 1;
    */

    private double inputEconomicWellBeing;

    public double InputEconomicWellBeing
    {
        get { return inputEconomicWellBeing; }
        set
        {
            inputEconomicWellBeing = value;
            EconomicWellBeing = Sigmoid(value, LIMITS_EconomicWellBeing[0],
                    LIMITS_EconomicWellBeing[1], Temperatures.T_EconomicWellBeing);
        }
    }

    [SerializeField]
    private double economicWellBeing = 1;
    public double EconomicWellBeing
    {
        get { return economicWellBeing; }
        set
        {
            economicWellBeing = value;
            OnHappinessChanged();
        }
    }

    private double inputPersonalWellBeing;

    public double InputPersonalWellBeing
    {
        get { return inputEconomicWellBeing; }
        set
        {
            inputPersonalWellBeing = value;
            PersonalWellBeing = Sigmoid(value, LIMITS_PersonalWellBeing[0],
                    LIMITS_PersonalWellBeing[1], Temperatures.T_PersonalWellBeing);
        }
    }

    // Measure of people's social and recreational activities
    [SerializeField] 
    private double personalWellBeing = 1;
    public double PersonalWellBeing
    {
        get { return personalWellBeing; }
        set
        {
            personalWellBeing = value; 
            OnHappinessChanged();
        }
    }


    public EventHandler HappinessChanged;


    public void SetDefaultModel()
    {
        InputEconomicWellBeing = INITIAL_InputEconomicWellBeing;
        InputPersonalWellBeing = INITIAL_InputPersonalWellBeing;
    }


    /*
     * Publisher-related method.
     */
    public void OnHappinessChanged()
    {
        if (HappinessChanged != null)
        {
            HappinessChanged(this, EventArgs.Empty);
        }
    }
}

