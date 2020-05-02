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


    public void SetDefaultEnvironment()
    {
        // Global time of the game is created.
        Time.CreateTime();
        
        countryController.SetDefaultEnvironment();
    }


    public void NextDay()
    {
        countryController.NextDay();
        Time.NextDay();
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

