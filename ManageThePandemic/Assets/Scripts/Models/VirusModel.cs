using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusModel : MonoBehaviour
{
    // [Day, Aggregate active case number]
    private Dictionary<int, int> activeCaseNumber;
    // [Day, new case number on that day]
    private Dictionary<int, int> dailyCaseNumber;
    // [Day, Growth Rate on that day]
    private Dictionary<int, double> growthRate;
    // It will multiplied with the activeCaseNumber.
    // The result is the new dailyCaseNumber;
    private double growthRateParameter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CalculateDailyCaseNumber()
    {
        throw new System.NotImplementedException();
    }
}
