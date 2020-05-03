using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Object = System.Object;


/*
 * Manages the time of the game.
 *
 * Handles the actions and records the required events to the calendar.
 * Executes the events.
 */
public class GameController : MonoBehaviour, ITimeDrivable
{
    private Dictionary<int, List<MTPEvent>> calendar = new Dictionary<int, List<MTPEvent>>();

    [SerializeField]
    public CountryController countryController;

    private ActionsController actionsController;

    public void Awake()
    {
        actionsController = GetComponent<ActionsController>();
    }

    public void Start()
    {
        SetDefaultEnvironment();
    }

    public void SetDefaultEnvironment()
    {
        // Global time of the game is created.
        Time.CreateTime();
        
        countryController.SetDefaultEnvironment();

        SubscribeToActions();
    }

    public void NextDay()
    {
        countryController.NextDay();
        Time.NextDay();
    }


    private void SubscribeToActions()
    {
        foreach (var action in actionsController.actions)
        {
            action.GetComponent<SubscriberPublisher>().ButtonClicked += OnActionTaken;
        }
    }

    //TODO: You left in here. Bundan sonra yapialcak sey, buradan gelen dataya gore
    //budget ve calendar uzerinde islemler yaparak aksiyonu gerceklestirmek.
    public void OnActionTaken(object source, ActionDataArgs actionDataArgs)
    {
        Debug.Log("Greetings from GameController.");
    }


    public void UpdateFields()
    {
        throw new NotImplementedException();
    }

 
    //TODO: anlik actionlari kontrol et.
    public void AddActionToCalendar(ActionData actionData)
    {
        foreach (MTPEvent MTPevent in actionData.events)
        {
            AddEventToCalendar(MTPevent);
        }
    }

    private void AddEventToCalendar(MTPEvent MTPevent)
    {
        int today = Time.GetInstance().GetDay();
        int eventDay = today + MTPevent.delayTime;

        if ( !calendar.ContainsKey(eventDay) )
        {
            calendar.Add(eventDay, new List<MTPEvent>());
        }

        calendar[eventDay].Add(MTPevent);
    }

    private void ExecuteCurrentEvents()
    {
        throw new System.NotImplementedException();
    }
}

/*
 * TODO: move this comment!
 * The data is displayed on the screen in the end of the day.
 * Active case number is the number reached by the end of the day.
 * Growth rate parameter and vulnerability ratio are the values,
 * which was valid during the day.
 */

/*
 * TODO: write code-style document
 *        [SerializeField]
 *        [Dependent] [independent]
 *        [HideInInspector]
 * TODO: activate event system
 * TODO: add probability system.
 * TODO: add delay
 */
