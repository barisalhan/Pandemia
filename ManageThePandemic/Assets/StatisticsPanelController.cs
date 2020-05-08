using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanelController : MonoBehaviour
{
    [SerializeField]
    private Text activeCaseText;

    [SerializeField]
    private Text recoveredText;

    [SerializeField]
    private Text ripText;


    public StatisticsPanelController(GameController gameController)
    {
        
    }

    public void OnNextDayClicked(object source, DailyStatisticsArgs eventArgs)
    {
        Debug.Log("StatisticsPanel saw that next day button is clicked.");

        activeCaseText.text = eventArgs.activeCases.ToString();
        recoveredText.text = eventArgs.recoveredCases.ToString();
        ripText.text = eventArgs.deathCases.ToString();
    }


}
