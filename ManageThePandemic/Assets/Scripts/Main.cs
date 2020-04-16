﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


/*
 *  The bridge between gameController and displayController.
 * TODO: activate event system
 * TODO: Implement HealthSystem model.
 * TODO: Ekonomik model
 * TODO: Society model
 * TODO: add probability system.
 * TODO: add delay
 * TODO: connect UI
 */
public class Main : MonoBehaviour
{
    private GameController gameController;
    private DisplayController displayController;
    private Prefabs prefabs;
    private Actions actions;

    void Awake()
    {
        gameController = GetComponent<GameController>();
        displayController = GetComponent<DisplayController>();
        prefabs = GetComponent<Prefabs>();
        actions = GetComponent<Actions>();
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

        if (Time.GetInstance().GetDay() == 3)
        { 
            GameObject actionAsker = prefabs.InstantiatePrefab(Prefabs.Name.ActionAsker);
            actionAsker.GetComponentInChildren<Text>().text = actions.GetAction(Actions.Name.PropagandaToRaiseAwareness).description;
        }
    }
}
