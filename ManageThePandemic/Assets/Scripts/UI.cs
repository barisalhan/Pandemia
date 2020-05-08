using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject mainGameObject;
    private GameController gameController;

    [HideInInspector]
    public RegionSprites regionSprites;
    private SpriteRenderer currentlyOpenRegionInPanel = null;

    //TODO: add wavy color
    private Color32 regionBaseColor = new Color32(57, 136, 136, 255);
    private Color32 regionOnClickColor = new Color32(32, 110, 110, 255);

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
        regionSprites = GetComponent<RegionSprites>();
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

    public void OnClickRegion(string regionName)
    {
        RegionController regionController = gameController.countryController.GetRegionByString(regionName);

        SpriteRenderer regionSpriteRenderer = regionSprites.GetRegionSprite(regionName);

        if (currentlyOpenRegionInPanel != null)
        {
            //TODO: add a new color variable to generalize it.
            currentlyOpenRegionInPanel.color = regionBaseColor;
        }

        currentlyOpenRegionInPanel = regionSpriteRenderer;

        regionSpriteRenderer.color = regionOnClickColor;

        Text[] texts = InfoPanel.GetComponentsInChildren<Text>();

        int today = Time.GetInstance().GetDay();

        //TODO: create names!
        texts[1].text = regionController.activeCases[today - 1].ToString();
        texts[3].text = regionController.healthSystemModel.aggregateDeathCases[today - 1].ToString();
        texts[5].text = regionController.healthSystemModel.aggregateRecoveredCases[today - 1].ToString();

        InfoPanel.SetActive(true);
    }
    
    public void OnClickCloseRegionPanel()
    {
        if (currentlyOpenRegionInPanel != null)
        {
            currentlyOpenRegionInPanel.color = regionBaseColor;
            currentlyOpenRegionInPanel = null;
        }

        InfoPanel.SetActive(false);
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
