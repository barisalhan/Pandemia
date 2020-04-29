using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject mainGameObject;
    private Main main;

    [HideInInspector]
    public RegionSprites regionSprites;
    private SpriteRenderer currentlyOpenRegionInPanel = null;

    [SerializeField]
    private GameObject InfoPanel;

    public void Awake()
    {
        main = mainGameObject.GetComponent<Main>();
        regionSprites = GetComponent<RegionSprites>();
    }
    
    
    public void OnClickRegion(string regionName)
    {
        RegionController regionController = main.countryController.GetRegionController(regionName);

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

        texts[1].text = regionController.activeCases[today - 1].ToString();
        texts[3].text = regionController.healthSystemModel.aggregateDeathCases[today - 1].ToString();
        texts[6].text = regionController.healthSystemModel.aggregateRecoveredCases[today - 1].ToString();

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
}
