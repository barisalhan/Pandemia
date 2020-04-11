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
        displayController.population.text = "Population : " + gameController.GetParameter(0,0,"Population");
        displayController.activeCases.text = "Active Cases: " + gameController.GetParameter(0, 0, "ActiveCases");
    }

    // TODO: gunu nasil tuttugumuzu duzelt.
    // TODO: Su an next day butonu ekleyecektik. Bu fonksiyon degisecek.
    void Update()
    {
        displayController.activeCases.text = "Active Cases: " + gameController.GetParameter(0, 0, "ActiveCases");
    }
}
