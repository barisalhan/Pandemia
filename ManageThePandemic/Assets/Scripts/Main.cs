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

    [SerializeField]
    private CountryController countryController;

    [SerializeField] 
    private GameObject UIGameObject;

    private UI UI;

    public GameObject panel;

    private SpriteRenderer currentlyOpenRegionInPanel = null;

    void Awake()
    {
        gameController = GetComponent<GameController>();
        displayController = GetComponent<DisplayController>();
        prefabs = GetComponent<Prefabs>();
        actions = GetComponent<Actions>();
        UI = UIGameObject.GetComponent<UI>();
    }

    void Start()
    {
        gameController.SetDefaultEnvironment();
        displayController.populationText.text = "Population : " + gameController.countryControllers[0].GetRegion(CountryController.Name.West).GetParameter("Population");
        displayController.activeCasesText.text = "Active Cases: " + gameController.countryControllers[0].GetRegion(CountryController.Name.West).GetParameter("ActiveCases");
        displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();
        
    }
    
    /*
     * The data is displayed on the screen in the end of the day. Active case number is the number reached by the end of the day.
     * Growth rate parameter and vulnerability ratio are the values which was valid during that day.
     */
    public void NextDay()
    {
        gameController.NextDay();
        displayController.activeCasesText.text = "Active Cases: " + gameController.countryControllers[0].GetRegion(CountryController.Name.West).GetParameter("ActiveCases");
        displayController.currentDayText.text = "Day: " + Time.GetInstance().GetDay();

        int today = Time.GetInstance().GetDay();

        displayController.recoveredText.text = "Recovered Cases: " + gameController.countryControllers[0]
            .GetRegion(CountryController.Name.West).healthSystemModel.aggregateRecoveredCases[today-1];

        /*
        if (Time.GetInstance().GetDay() == 3)
        { 
            GameObject actionAsker = prefabs.GetPrefab(Prefabs.Name.ActionAsker);
            actionAsker.SetActive(true);
            Action action = actions.GetAction(Actions.Name.PropagandaToRaiseAwareness);
            actionAsker.GetComponentInChildren<Text>().text = action.description;
            ActionHolder actionHolder = actionAsker.GetComponent<ActionHolder>();
            actionHolder.action = action;
        }
        */
    }

    public void OnClickYesToAction()
    {
        GameObject actionAsker = prefabs.GetPrefab(Prefabs.Name.ActionAsker);
        actionAsker.SetActive(false);
        ActionHolder actionHolder = actionAsker.GetComponent<ActionHolder>();
        Action action = actionHolder.action;
        gameController.AddActionToCalendar(action);

    }
    
    public void OnClickRegion(string regionName)
    {
        RegionController regionController = countryController.GetRegionController(regionName);

        SpriteRenderer regionSpriteRenderer = UI.regionSprites.GetRegionSprite(regionName);

        currentlyOpenRegionInPanel = regionSpriteRenderer;

        regionSpriteRenderer.color = new Color32(60, 116, 116, 255);

        Text[] texts = panel.GetComponentsInChildren<Text>();

        int today = Time.GetInstance().GetDay();

        texts[1].text = regionController.activeCases[today-1].ToString();
        texts[3].text = regionController.healthSystemModel.aggregateDeathCases[today-1].ToString();
        texts[6].text = regionController.healthSystemModel.aggregateRecoveredCases[today-1].ToString();

        panel.SetActive(true);
    }

    public void OnClickCloseRegionPanel()
    {
        currentlyOpenRegionInPanel.color = new Color32(35, 59, 59, 255);
        currentlyOpenRegionInPanel = null;
        panel.SetActive(false);
    }
}
