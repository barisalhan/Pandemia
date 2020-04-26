using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/SocietyModel")]
public class SocietyModel : MTPScriptableObject
{
    // All parameters are in scale of 0-25, 25 is the best.

    public int economicWellBeing = 25;

    // Measure of people's trust on government's politics
    public int faithInGovernment = 25;

    public int virusSituation = 25;

    // Measure of people's social and recreational activities
    public int personalWellBeing = 25;

    public int CalculateHappiness()
    {
        int happiness = economicWellBeing + faithInGovernment + virusSituation + personalWellBeing;
        return happiness;
    }
}
