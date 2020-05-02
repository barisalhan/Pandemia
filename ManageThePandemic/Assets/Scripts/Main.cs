using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


/*
 * TODO: write code-style document
 *        [SerializeField]
 *        [Dependent] [independent]
 *        [HideInInspector]
 * TODO: activate event system
 * TODO: add probability system.
 * TODO: add delay
 */

/*
 * 
 */
 public class Main : MonoBehaviour
{
    [HideInInspector]
    public GameController gameController;

    void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    void Start()
    {
        gameController.SetDefaultEnvironment();
    }
    
    public void NextDay()
    {
        gameController.NextDay();
    }
}


/*
     * TODO: move this comment!
     * The data is displayed on the screen in the end of the day.
     * Active case number is the number reached by the end of the day.
     * Growth rate parameter and vulnerability ratio are the values,
     * which was valid during the day.
     */
