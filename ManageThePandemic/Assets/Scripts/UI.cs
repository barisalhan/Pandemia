using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject mainGameObject;
    [HideInInspector]
    public GameController gameController;

    [SerializeField]
    private GameObject InfoPanel;

    [SerializeField]
    private GameObject ActionTreePanel;

    private BarController barController;

    private StatisticsPanelController statisticsPanelController;

    private UpperPanelController upperPanelController;

    public void Awake()
    {
        gameController = mainGameObject.GetComponent<GameController>();
        barController = GetComponent<BarController>();
        statisticsPanelController = GetComponent<StatisticsPanelController>();
        upperPanelController = GetComponent<UpperPanelController>();

        SubscribeBarToSocietyModel();
        SubscribeStatisticsToGameController();
        SubscribeUpperPanelToBudget();
    }


    private void SubscribeBarToSocietyModel()
    {
        gameController.countryController.societyModel.HappinessChanged += barController.OnHappinesChanged;
    }


    private void SubscribeStatisticsToGameController()
    {
        gameController.NextDayClicked += statisticsPanelController.OnNextDayClicked;
    }

    private void SubscribeUpperPanelToBudget()
    {
        
    }

   

    public void OnClickCloseActionTreePanel()
    {
        ActionTreePanel.SetActive(false);
    }

    public void OnClickOpenActionTreePanel()
    {
        ActionTreePanel.SetActive(true);
    }

}
