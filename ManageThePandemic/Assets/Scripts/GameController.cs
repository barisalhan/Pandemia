using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Object = System.Object;


/*
 * Manages the time of the game.
 * Handles the actions and records the required events to the calendar.
 * Executes the events.
 * Manages the country controllers.
 */
public class GameController : MonoBehaviour, ITimeDrivable
{
    private Dictionary<int, List<MTPEvent>> calendar = new Dictionary<int, List<MTPEvent>>();

    public List<CountryController> countryControllers;


    // TODO: extend this for other models.
    /*
     * Creates the time.
     * Fills 0th day values.
     * Sets models to default.
     *
     * It is called in the very beginning of the game from Main.Start().
     *
     * We need values of 0th day to start the  game, because in each day,
     * we use numbers of yesterday and parameters of today. 
     */
    public void SetDefaultEnvironment()
    {
        // Global time of the game is created.
        Time.CreateTime();

        foreach (CountryController country in countryControllers)
        {
            country.SetDefaultEnvironment();
        }
    }


    // TODO: Make models tunable.
    // Update is called once per frame
    public void NextDay()
    {
        foreach (CountryController country in countryControllers)
        {
            country.NextDay();
        }
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

    private void AddNewAction(Action action)
    {
        throw new System.NotImplementedException();
    }

    
}

