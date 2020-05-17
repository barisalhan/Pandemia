using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;
using Object = System.Object;
using UnityEngine.SceneManagement;

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
    private Dictionary<int, List<ActionDataArgs>> actionConstructionCalendar = new Dictionary<int, List<ActionDataArgs>>();

    [SerializeField]
    public CountryController countryController;

    private ActionsController actionsController;

    [SerializeField]
    private GameObject endGamePanel;

    [SerializeField]
    private Text dayText;

    [SerializeField]
    private GameObject statisticsPanel;

    private Text[] caseTexts;

    [SerializeField]
    private Text[] moneyTexts;

    public void Awake()
    {
        actionsController = GetComponent<ActionsController>();
        caseTexts = statisticsPanel.GetComponentsInChildren<Text>();
    }

    public void Start()
    {
        SubscribeMoneyTextToBudget();
        SetDefaultEnvironment();
    }

    private void SubscribeMoneyTextToBudget()
    {
        countryController.BudgetChanged += OnBudgetChanged;
    }

    private void OnBudgetChanged(object source, BudgetArgs budgetArgs)
    {
        //TODO: add M to moneyText.
        foreach (var moneyText in moneyTexts)
        {
            moneyText.text = budgetArgs.budget.ToString() + " M$";
        }
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

        UpdateDailyStatistics();

        Time.NextDay();

        UpdateDayText();

        ExecuteActionConstructionCalendar();
        ExecuteEventCalendar();
        CheckGameOver();
    }


    /*
     * Warning: It's dependent to the order of case texts in the UI.
     */
    private void UpdateDailyStatistics()
    {
        caseTexts[0].text = countryController.GetActiveCases().ToString();
        caseTexts[1].text = countryController.GetRecoveredCases().ToString();
        caseTexts[2].text = countryController.GetDeathCases().ToString();
    }


    private void UpdateDayText()
    {
        string filler = " Day of Pandemia";
        if (Time.GetInstance().GetDay() == 2)
        {
            dayText.text = " 2nd"+ filler;
        }
        else if (Time.GetInstance().GetDay() == 3)
        {
            dayText.text = " 3rd" + filler;
        }
        else
        {
            dayText.text = " " + Time.GetInstance().GetDay().ToString() + "th" + filler;
        }
    }


    private void UpdateCaseTexts()
    {
        
    }

    private void CheckGameOver()
    {
        if (countryController.GetActiveCases() > 60)
        {
            endGamePanel.SetActive(true);
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


    private void SubscribeToActions()
    {
        foreach (var action in actionsController.actions)
        {
            action.GetComponent<SubscriberPublisher>().actionTaken += OnActionTaken;
        }
    }


    /*
     * Subscription-related method.
     *
     */
    public void OnActionTaken(object source, ActionDataArgs actionDataArgs)
    {
        int cost = actionDataArgs.actionData.cost;
        if (cost > countryController.GetBudget())
        {
            Debug.Log("Action is tried to be taken even though the budget is insufficient.");
            return;
        }
        if (cost > 0)
        {
            countryController.ChangeBudget(cost);
        }


        int timeToConstruct = actionDataArgs.actionData.timeToConstruct;

        if (timeToConstruct > 0)
        {
            int today = Time.GetInstance().GetDay();

            int completionDay = today + timeToConstruct;

            if (!actionConstructionCalendar.ContainsKey(completionDay))
            {
                actionConstructionCalendar.Add(completionDay, new List<ActionDataArgs>());
            }

            actionConstructionCalendar[completionDay].Add(actionDataArgs);
            actionDataArgs.publisher.OnConstruction();

            Debug.Log("An action with construction time is taken. Now it is under progress.");
        }
        else
        {
            Debug.Log("An immediate action is taken.");
            if (actionDataArgs.actionData.type == 0)
            {
                OnActionConstructionCompleted(actionDataArgs);
            }
            
        }
    }


    private void ExecuteActionConstructionCalendar()
    {
        int today = Time.GetInstance().GetDay();
        if (actionConstructionCalendar.ContainsKey(today))
        {
            foreach (var actionDataArgs in actionConstructionCalendar[today])
            {
                OnActionConstructionCompleted(actionDataArgs);
            }
        }
    }

    public void OnActionConstructionCompleted(ActionDataArgs actionDataArgs)
    {
        AddActionEventsToEventCalendar(actionDataArgs.actionData);

        if (actionDataArgs.actionData.type == 0)
        {
            actionDataArgs.publisher.OnActionCompleted();
        }
        
        else if (actionDataArgs.actionData.type == 1)
        {
            Debug.Log("Daha implemente etmedik.");
        }
        
    }


    public void UpdateFields()
    {
        throw new NotImplementedException();
    }

    
    public void AddActionEventsToEventCalendar(ActionData actionData)
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


    public void RestartGame()
    {

    }


    public void QuitGame()
    {
        Application.Quit();
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
