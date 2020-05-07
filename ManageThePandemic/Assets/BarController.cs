using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public Slider mainSlider;

    public void OnHappinesChanged(object source, HappinessArgs happinessArgs)
    {
        Debug.Log("BarController saw the change in the happiness.");
        mainSlider.value = (float)happinessArgs.value;
    }
    
}
