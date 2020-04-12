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
        gameController.countryControllers[0].SetParameter("population", 1, 2);
        displayController.populationText.text = "Population : " + gameController.countryControllers[0].GetParameter("Population");
        //displayController.activeCasesText.text = "Active Cases: " + gameController.GetParameter(0, 0, "ActiveCases");
        //displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
    }


    public void NextDay()
    {
       // gameController.NextDay();
       // displayController.activeCasesText.text = "Active Cases: " + gameController.GetParameter(0, 0, "ActiveCases");
       // displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
    }
}
