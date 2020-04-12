using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

/*
 *  The bridge between gameController and displayController.
 */
public class Main : MonoBehaviour
{
    private GameController gameController;
    private DisplayController displayController;

    void Awake()
    {
        gameController = GetComponent<GameController>();
        displayController = GetComponent<DisplayController>();
    }

    void Start()
    {
        gameController.SetDefaultEnvironment();
        displayController.populationText.text = "Population : " + gameController.countryControllers[0].stateControllers[0].GetParameter("Population");
        displayController.activeCasesText.text = "Active Cases: " + gameController.countryControllers[0].stateControllers[0].GetParameter("ActiveCases");
        displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
    }
    
    /*
     * The data is displayed on the screen in the end of the day. Active case number is the number reached by the end of the day.
     * Growth rate parameter and vulnerability ratio are the values which was valid during that day.
     */
    public void NextDay()
    {
        gameController.NextDay();
        displayController.activeCasesText.text = "Active Cases: " + gameController.countryControllers[0].stateControllers[0].GetParameter("ActiveCases");
        displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
    }
}
