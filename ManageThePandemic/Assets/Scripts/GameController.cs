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
//TODO: Make a party about changing the model equation calculations to a central place!
public class GameController : MonoBehaviour, ITimeDrivable
{
    private Dictionary<int, List<MTPEvent>> eventCalendar = new Dictionary<int, List<MTPEvent>>();
    private Dictionary<int, List<SubscriberPublisher>> actionCalendar = new Dictionary<int, List<SubscriberPublisher>>();

    [SerializeField]
    public CountryController countryController;

    private ActionsController actionsController;

    public EventHandler<DailyStatisticsArgs> NextDayClicked;

    public void Awake()
    {
        actionsController = GetComponent<ActionsController>();
    }

    public void Start()
    {
        SetDefaultEnvironment();
        countryController.ChangeBudget(0);
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
        //TODO: check if it is safe or not.
        OnNextDayClicked();

        Time.NextDay();

        ExecuteActionCalendar();
        ExecuteEventCalendar();
    }

    private void ExecuteActionCalendar()
    {
        int today = Time.GetInstance().GetDay();
        if (actionCalendar.ContainsKey(today))
        {
            foreach (var publisher in actionCalendar[today])
            {
                publisher.OnActionCompleted();
            }
        }
    }


    private void ExecuteEventCalendar()
    {
        int today = Time.GetInstance().GetDay();
        if (eventCalendar.ContainsKey(today))
        {
            foreach (var mtpEvent in eventCalendar[today])
            {
                ExecuteEvent(mtpEvent);
            }
            eventCalendar[today].Clear();
        }

    }


    // TODO: refactor this method.
    private void ExecuteEvent(MTPEvent mtpEvent)
    {
        if (mtpEvent.targetModelName == "virus")
        {
            foreach (var regionController in countryController.regionControllers)
            {
                regionController.virusModel.ExecuteEvent(mtpEvent.targetParameter,
                                                         mtpEvent.effectType,
                                                         mtpEvent.effectValue);
            }
        }
        else if (mtpEvent.targetModelName == "society")
        {

            countryController.societyModel.ExecuteEvent(mtpEvent.targetParameter,
                                                        mtpEvent.effectType,
                                                        mtpEvent.effectValue);
        }
        else if (mtpEvent.targetModelName == "health")
        {
            foreach (var regionController in countryController.regionControllers)
            {
                regionController.healthSystemModel.ExecuteEvent(mtpEvent.targetParameter,
                                                                mtpEvent.effectType,
                                                                mtpEvent.effectValue);
            }
        }
        else if (mtpEvent.targetModelName == "economy")
        {
            foreach (var regionController in countryController.regionControllers)
            {
                regionController.economyModel.ExecuteEvent(mtpEvent.targetParameter,
                                                           mtpEvent.effectType,
                                                           mtpEvent.effectValue);
            }
        }
        else
        {
            Debug.Log("Unknown model type is entered.");
        }
    }


    protected virtual void OnNextDayClicked()
    {
        if (NextDayClicked != null)
        {
            var statistics = GetDailyStatistics();
            DailyStatisticsArgs dailyStatistics = new DailyStatisticsArgs(statistics.Item1,
                                                                        statistics.Item2,
                                                                           statistics.Item3);
            NextDayClicked(this, dailyStatistics);
        }
    }

    private void SubscribeToActions()
    {
        foreach (var action in actionsController.actions)
        {
            action.GetComponent<SubscriberPublisher>().buttonClicked += OnActionTaken;
        }
    }


    /*
     * Subscription-related method.
     *
     */
    public void OnActionTaken(object source, ActionDataArgs actionDataArgs)
    {
        //TODO: check in the beginning of the game if we have enough money to take the action.
        int cost = actionDataArgs.actionData.cost;
        if (cost > 0)
        {
            countryController.ChangeBudget(cost);
        }


        int timeToComplete = actionDataArgs.actionData.timeToComplete;

        if (timeToComplete > 0)
        {
            //TODO: change Time class to get today.
            int today = Time.GetInstance().GetDay();

            int completionDay = today + timeToComplete;

            if (!actionCalendar.ContainsKey(completionDay))
            {
                actionCalendar.Add(completionDay, new List<SubscriberPublisher>());
            }

            actionCalendar[completionDay].Add(actionDataArgs.publisher);

            Debug.Log("An action with initialization time is taken. Now it is under progress.");
        }
        else
        {
            Debug.Log("An immediate action is taken.");
            OnActionCompleted(actionDataArgs);
        }
    }


    public void OnActionCompleted(ActionDataArgs actionDataArgs)
    {
        actionDataArgs.publisher.OnActionCompleted();

        AddActionToCalendar(actionDataArgs.actionData);
    }


    public void UpdateFields()
    {
        throw new NotImplementedException();
    }

    
    public void AddActionToCalendar(ActionData actionData)
    {
        foreach (MTPEvent MTPevent in actionData.events)
        {
            AddEventToCalendar(MTPevent);
        }
        ExecuteEventCalendar();
    }

    private void AddEventToCalendar(MTPEvent MTPevent)
    {
        int today = Time.GetInstance().GetDay();
        int eventDay = today + MTPevent.delayTime;

        if ( !eventCalendar.ContainsKey(eventDay) )
        {
            eventCalendar.Add(eventDay, new List<MTPEvent>());
        }

        eventCalendar[eventDay].Add(MTPevent);
    }


    public Tuple<int,int,int> GetDailyStatistics()
    {
        // TODO: check if connections are correct or not.
        int activeCases = countryController.GetActiveCases();
        int recoveredCases = countryController.GetRecoveredCases();
        int deathCases = countryController.GetDeathCases();

        var result = Tuple.Create(activeCases, recoveredCases, deathCases);

        return result;
    }

}

public class DailyStatisticsArgs
{
    public int activeCases = 0;
    public int recoveredCases = 0;
    public int deathCases = 0;

    public DailyStatisticsArgs(int activeCases,
                               int recoveredCases,
                               int deathCases)
    {
        this.activeCases = activeCases;
        this.recoveredCases = recoveredCases;
        this.deathCases = deathCases;
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
