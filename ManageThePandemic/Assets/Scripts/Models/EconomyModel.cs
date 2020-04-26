using UnityEngine;

[CreateAssetMenu(menuName = "ManageThePandemic/EconomyModel")]
public class EconomyModel : MTPScriptableObject
{
    //[Independent]
    [SerializeField]
    private double economicDevelopmentParameter;

    //[Independent]
    [SerializeField]
    private double quarantineEffect;

    // We assume that a million people give 1 game money as a tax.
    private double normalization = 1/1e6;


    public int CalculateTax(int population, bool isQuarantined)
    {
        if (isQuarantined)
        {
            return (int) (quarantineEffect * population * normalization * economicDevelopmentParameter);
        }
        else
        {
            return (int) (population * normalization * economicDevelopmentParameter);
        }
    }
}
