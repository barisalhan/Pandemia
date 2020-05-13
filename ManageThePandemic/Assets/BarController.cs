using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public List<Slider> sliders;

    public void OnHappinesChanged(object source, HappinessArgs happinessArgs)
    {
        Debug.Log("BarController saw the change in the happiness.");
        foreach (var slider in sliders)
        {
            slider.value = (float)happinessArgs.value;
        }
    }
    
}
