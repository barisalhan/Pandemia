using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


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
    private Dictionary<int, List<ActionDataArgs>> actionOnUseCalendar = new Dictionary<int, List<ActionDataArgs>>();

    [SerializeField]
    public CountryController countryController;

    private ActionsController actionsController;

    [SerializeField] 
    private GameObject countryObject;

    [SerializeField] 
    private Canvas endGameCanvas;

    [SerializeField]
    private GameObject gameLostPanel;

    [SerializeField]
    private GameObject gameWonPanel;

    [SerializeField]
    private Text dayText;

    [SerializeField]
    private GameObject statisticsPanel;

    private Text[] caseTexts;

    // Budgeti gosteren yerler buna subscribe ediyor.
    [SerializeField]
    private Text[] moneyTexts;

    [SerializeField]
    private GameObject coinPrefab;

    public EventHandler<HospitalArgs> MaxHospitalOccupancyRateChanged;

    private bool isHospitalFull = false;
    private bool isDeathLimitExceeded = false;

    [SerializeField]
    public int[] weeklyDeathLimits;

    //TODO: Consider moving these rip group elements from here.
    [Header("RIP Statistics Group")]
    [SerializeField] 
    private Slider ripSlider;
    [SerializeField] 
    private Text ripStatisticsText;

    private int currentWeek;

    private TimelineController timelineController;

    private AdsController adsController;

    public void Awake()
    {
        actionsController = GetComponent<ActionsController>();
        caseTexts = statisticsPanel.GetComponentsInChildren<Text>();
        timelineController = GetComponent<TimelineController>();
        adsController = GetComponent<AdsController>();
    }

    public void Start()
    {
        SubscribeMoneyTextToBudget();
        SetDefaultEnvironment();

        UpdateHospitalOccupancyRate();
        //timelineController.SetWeeklyDeathLimits(weeklyDeathLimits.ToList());
    }

    private void SubscribeMoneyTextToBudget()
    {
        countryController.BudgetChanged += OnBudgetChanged;
    }

    private void OnBudgetChanged(object source, BudgetArgs budgetArgs)
    {
        foreach (var moneyText in moneyTexts)
        {
            moneyText.text = budgetArgs.budget.ToString() + " M$";
        }
    }


    public void SetDefaultEnvironment()
    {
        // Global time of the game is created.
        Time.CreateTime();
        currentWeek = Time.GetInstance().Week;

        countryController.SetDefaultEnvironment();

        SubscribeToActions();
    }

    public void NextDay()
    {
        countryController.NextDay();

        CreateCoinsOnMap(countryController.dailyTax);
        //DEPRECATED!
        UpdateDailyStatistics();

        Time.NextDay();

        // If we arrived to a new week.
        if (currentWeek != Time.GetInstance().Week)
        {
            OnNextWeek();
        }

        UpdateDayText();

        ExecuteActionConstructionCalendar();
        ExecuteEventCalendar();
        ExecuteActionOnUseCalendar();
        
        UpdateHospitalOccupancyRate();
        UpdateRipRate();

        CheckGameOver();
        if (Time.GetInstance().GetDay()%10 == 0)
        {
            adsController.myButton.interactable = true;
        }
    }

    // TODO: Think about a better place for this method. UI should not be done in GameController.
    private void CreateCoinsOnMap(int money)
    {
        int moneyForEachCoin = 30;
        int numberOfCoins = money / moneyForEachCoin;
        
        // TODO: centralize random.
        Random random = new Random();
        for (int i = 0; i < numberOfCoins; i++)
        {
            // TODO: Change this when adding a new map.
            int max_x = 3;
            int min_x = -2;
            double xPosition = (random.NextDouble() * (max_x - min_x)) + min_x;

            double max_y = 3.2;
            double min_y = 0.5;
            double yPosition = (random.NextDouble() * (max_y - min_y)) + min_y;

            Vector3 position = new Vector3((float) xPosition, (float) yPosition, 10);
            GameObject coinObject = Instantiate(coinPrefab,
                                                position,
                                                Quaternion.identity,
                                                countryObject.transform);
            CoinController coinController = coinObject.GetComponent<CoinController>();
            coinController.money = moneyForEachCoin;
            coinObject.SetActive(true);
        }
    }


    private void OnNextWeek()
    {
        currentWeek = Time.GetInstance().Week;
        //adsController.ShowAdvertisement();
        

        // TODO: Newspaper wil be added from here.


        timelineController.OnNextWeek();
    }


    private void UpdateHospitalOccupancyRate()
    {
        double maxOccupancyRate = 0;
        string regionName = "";

        foreach (var region in countryController.regionControllers)
        {
            double occupancyRate = region.HealthSystemModel.CalculateHospitalOccupancyRate();
            if (occupancyRate > maxOccupancyRate)
            {
                maxOccupancyRate = occupancyRate;
                regionName = region.name;
            }

            if (occupancyRate > 0.99)
            {
                isHospitalFull = true;
            }
        }

        OnOccupancyRateChanged(maxOccupancyRate, regionName);
    }


    private void UpdateRipRate()
    {

        int deathCases = countryController.GetDeathCases();
        int deathLimit = weeklyDeathLimits[Time.GetInstance().Week - 1];

        // Slider value takes the input between 0 and 1. So, we limit the max value.
        float ratio = Math.Min((float)deathCases / deathLimit, 1);

        ripSlider.value = ratio;
        ripStatisticsText.text = String.Format("{0:0}", (ratio * 100)) + "%";

        if (ratio > 0.99)
        {
            isDeathLimitExceeded = true;
        }
    }


    public void OnOccupancyRateChanged(double maxOccupancyRate, string regionName)
    {
        if (MaxHospitalOccupancyRateChanged != null)
        {
            Debug.Log("Capacity cagirildi.");
            MaxHospitalOccupancyRateChanged(this, new HospitalArgs(maxOccupancyRate, regionName));
        }
    }


    /*
     * Warning: It's dependent to the order of case texts in the UI.
     * !!!!DEPRECATED!!!!!
     * When you start to use again, make sure that it is called after Time.NextDay();
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
        if (Time.GetInstance().GetDay()%10 == 2)
        {
            if (Time.GetInstance().GetDay() == 12)
            {
                dayText.text = " 12th" + filler;
            }
            else
            {
                dayText.text = " " + Time.GetInstance().GetDay().ToString() + "nd" + filler;
            }
        }
        else if (Time.GetInstance().GetDay()%10 == 3)
        {
            if (Time.GetInstance().GetDay() == 13)
            {
                dayText.text = " 13th" + filler;
            }
            else
            {
                dayText.text = " " + Time.GetInstance().GetDay().ToString() + "rd" + filler;
            }
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
        if (isHospitalFull ||
            isDeathLimitExceeded ||
            countryController.getAvgHappiness() < 0.2)
        {
            endGameCanvas.gameObject.SetActive(true);
            gameLostPanel.SetActive(true);
        }

        if (Time.GetInstance().Week == 9)
        {
            endGameCanvas.gameObject.SetActive(true);
            gameWonPanel.SetActive(true);
        }
    }


    /*
     * If in the forward mode, it executes the event and
     * add the reverse of the same event to the calendar.
     *
     * Else, it executes the event and set the event to default.
     */
    private void ExecuteEventCalendar()
    {
        int today = Time.GetInstance().GetDay();
        if (eventCalendar.ContainsKey(today))
        {
            foreach (var mtpEvent in eventCalendar[today])
            {
                ExecuteEvent(mtpEvent);

                if (mtpEvent.isReversible == 1)
                {
                    // Forward-mode = 0
                    // Reverse-mode = 1
                    if (mtpEvent.isReverse == 0)
                    {
                        int eventDay = today + mtpEvent.duration;
                        AddEventToCalendar(mtpEvent, eventDay);
                    }

                    mtpEvent.isReverse = 1 - mtpEvent.isReverse;
                }
            }
            eventCalendar[today].Clear();
        }
    }


    /*
        * Modelleri bul
        * Parametreyi getir.
        * Denkleme ver.
        * Paratmetreyi geri koy.
    */
    private void ExecuteEvent(MTPEvent mtpEvent)
    {
        foreach (var regionController in countryController.regionControllers)
        {
            string modelName = mtpEvent.targetModel.ToString();
            string parameterName = mtpEvent.targetParameter.ToString();

            var model = regionController.GetParameterValue<MTPScriptableObject>(modelName);
            //Debug.Log("Model name: " + model.name);

            double parameter = model.GetParameterValue<Double>(parameterName);
            //Debug.Log("Value of " + regionController.name + ": " + parameter);

            double[] limits = model.GetParameterLimits(parameterName);

            double newValue = CalculateNewValue(mtpEvent,
                                                parameter,
                                                limits);

            model.SetParameterValue(parameterName, newValue);
        }
        
    }

    private double CalculateNewValue(MTPEvent mtpEvent,
                                     double parameter,
                                     double[] limits)
    {
        int effectType = mtpEvent.effectType;
        double effectValue = mtpEvent.effectValue;

        if (!MTPEvent.possibleEffectTypes.Contains(effectType))
        {
            Debug.Log("Unknown effect type is entered for the health system model." +
                      "0 value is returned.");
            return 0;
        }

        // Arithmetic
        if (effectType == 0)
        {
            if (mtpEvent.isReversible == 1 && mtpEvent.isReverse == 1)
            {
                parameter -= effectValue;
            }
            else
            {
                parameter += effectValue;
                //Debug.Log("Executing a arithmetic event");
            }
            
        }

        // Geometric
        else if (effectType == 1)
        {
            if (mtpEvent.isReversible == 1 && mtpEvent.isReverse == 1)
            {
                if (effectValue > 0)
                {
                    double nominator = parameter - effectValue * limits[1];
                    double denominator = 1 - effectValue;
                    parameter = nominator / denominator;
                }
                else
                {
                    double nominator = parameter + effectValue * limits[0];
                    double denominator = 1 + effectValue;
                    parameter = nominator / denominator;
                }
            }
            else
            {
                //Debug.Log("Executing a geometric event");
                if (effectValue > 0)
                {
                    parameter += (limits[1] - parameter) * effectValue;
                }
                else
                {
                    parameter += (parameter - limits[0]) * effectValue;
                }
            }
        }

        // Boolean ( We use double for representing boolean. )
        else if (effectType == 2)
        {
            //Debug.Log("Executing a boolean event");
            parameter = 1 - parameter;
        }

        // Limitless geometric
        else if (effectType == 3)
        {
            if (effectValue < 0)
            {
                Debug.Log("Negative coefficient is given for a limitless geometric event.");
            }
            else if (mtpEvent.isReversible == 1 && mtpEvent.isReverse == 1)
            {
                parameter /= effectValue;
            }
            else
            {
                parameter *= effectValue;
            }
        }

        return parameter;
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
            countryController.SpendMoney(cost);
        }


        int timeToConstruct = actionDataArgs.actionData.timeToConstruct;

        if (timeToConstruct > 0)
        {
            int today = Time.GetInstance().GetDay();

            int completionDay = today + timeToConstruct;

            
            AddElementToDictionary(actionConstructionCalendar,
                                   completionDay, 
                                   actionDataArgs);
            
            actionDataArgs.publisher.OnConstruction();

            //Debug.Log("An action with construction time is taken. Now it is under progress.");
        }
        else
        {
           // Debug.Log("An action without construction time is taken.");
            OnActionConstructionCompleted(actionDataArgs);
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

    private void ExecuteActionOnUseCalendar()
    {
        int today = Time.GetInstance().GetDay();
        if (actionOnUseCalendar.ContainsKey(today))
        {
            foreach (var actionDataArgs in actionOnUseCalendar[today])
            {
                actionDataArgs.publisher.SetReadyOrLowBudget();
            }
        }
    }

    public void OnActionConstructionCompleted(ActionDataArgs actionDataArgs)
    {
        AddActionEventsToEventCalendar(actionDataArgs.actionData);

        Debug.Log("Eventleri ekledim.");
        if (actionDataArgs.actionData.type == 0)
        {
            actionDataArgs.publisher.OnActionCompleted();
        }

        else if (actionDataArgs.actionData.type == 1)
        {
            int duration = actionDataArgs.actionData.duration;
            if (duration > 0)
            {
                int today = Time.GetInstance().GetDay();
                int endDay = today + duration;
                AddElementToDictionary(actionOnUseCalendar,
                    endDay,
                    actionDataArgs);
                actionDataArgs.publisher.OnUse();
            }
            else
            {
                actionDataArgs.publisher.SetReadyOrLowBudget();
            }
        }
        else if (actionDataArgs.actionData.type == 2)
        {
            endGameCanvas.gameObject.SetActive(true);
            gameWonPanel.SetActive(true);
        }
    }


    /*
     * Orhan ve Barış'tan sevgiler. 17.05.2020
     */
    public void AddElementToDictionary<T, V>(Dictionary<T, List<V>> dict, T t, V v)
    {
        if (!dict.ContainsKey(t))
        {
            dict.Add(t, new List<V>());
        }
        dict[t].Add(v);
    }


    public void UpdateFields()
    {
        throw new NotImplementedException();
    }

    
    public void AddActionEventsToEventCalendar(ActionData actionData)
    {
        int today = Time.GetInstance().GetDay();
        foreach (MTPEvent MTPevent in actionData.events)
        {
            if(MTPevent.isReversible == 1)
            {
                MTPevent.duration = actionData.duration;
            }

            AddEventToCalendar(MTPevent, today);
        }
        ExecuteEventCalendar();
    }

    private void AddEventToCalendar(MTPEvent MTPevent, int eventDay)
    {
        AddElementToDictionary(eventCalendar, eventDay, MTPevent);
    }


    public void RestartGame()
    {

    }


    public void QuitGame()
    {
        Application.Quit();
        //SceneManager.LoadScene("StartScreen");
    }

}

public class HospitalArgs
{
    public double value;
    public string regionName;

    public HospitalArgs(double value, string regionName)
    {
        this.value = value;
        this.regionName = regionName;
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
