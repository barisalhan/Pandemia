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

    [SerializeField]
    private GameObject InfoPanel;

    [SerializeField]
    private GameObject ActionTreePanel;

    public void Awake()
    {
        gameController = mainGameObject.GetComponent<GameController>();
        regionSprites = GetComponent<RegionSprites>();
    }
    
    
    public void OnClickRegion(string regionName)
    {
        RegionController regionController = gameController.countryController.GetRegionByString(regionName);

        SpriteRenderer regionSpriteRenderer = regionSprites.GetRegionSprite(regionName);

        if (currentlyOpenRegionInPanel != null)
        {
            //TODO: add a new color variable to generalize it.
            currentlyOpenRegionInPanel.color = new Color32(35, 59, 59, 255);
        }

        currentlyOpenRegionInPanel = regionSpriteRenderer;

        regionSpriteRenderer.color = new Color32(60, 116, 116, 255);

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
            currentlyOpenRegionInPanel.color = new Color32(35, 59, 59, 255);
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
