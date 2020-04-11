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
public class GameController : MonoBehaviour
{
    public int day = 0;

    private Dictionary<int, List<Event>> calendar;

    public List<CountryController> countryControllers;


    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultEnvironment();
    }

    // TODO: extend this for other models.
    void SetDefaultEnvironment()
    {
        foreach (CountryController country in countryControllers)
        {
            foreach (StateController state in country.stateControllers)
            {
                state.virusModel.SetDefaultModel();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (CountryController country in countryControllers)
        {
            foreach (StateController state in country.stateControllers)
            {
                state.virusModel.Update();
            }
        }
    }

    private void NextDay()
    {
       
    }

    private void ExecuteCurrentEvents()
    {
        throw new System.NotImplementedException();
    }

    private void AddNewAction(Action action)
    {
        throw new System.NotImplementedException();
    }

    private void AddEventsToCalendar(Action action)
    {
        throw new System.NotImplementedException();
    }

   
    /*
     * @fieldName:
     *  - Population
     *  - ActiveCases
     */
    public string GetParameter(int country, int state, string fieldName)
    {
        // Holds the metadata of the class.
        Type type = typeof(StateController);
        string methodName = "Get" + fieldName;
        // Holds kind of a pointer to the method that we want to reach.
        MethodInfo method = type.GetMethod(methodName);

        Object instanceOfTheClass = countryControllers[country].stateControllers[state];

        // Runs the method in the given instance and returns the result.
        var value = method.Invoke(instanceOfTheClass, null);

        return value.ToString();
    }
}

