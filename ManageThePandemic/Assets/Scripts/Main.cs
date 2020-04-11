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
        displayController.populationText.text = "Population : " + gameController.GetParameter(0,0,"Population");
        displayController.activeCasesText.text = "Active Cases: " + gameController.GetParameter(0, 0, "ActiveCases");
        displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
    }


    public void NextDay()
    {
        gameController.NextDay();
        displayController.activeCasesText.text = "Active Cases: " + gameController.GetParameter(0, 0, "ActiveCases");
        displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
    }
}
