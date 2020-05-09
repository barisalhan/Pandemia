using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoPanelController : MonoBehaviour
{
    private UI UI;

    [SerializeField]
    private GameObject regionPanel;

    private Image regionLittleImageOnPanel;

    private RectTransform regionPanelRectTransform;

    [HideInInspector]
    public RegionSprites regionSprites;
    private SpriteRenderer currentlyOpenRegionInPanel = null;

    //TODO: add wavy color
    private Color32 regionBaseColor = new Color32(57, 136, 136, 255);
    private Color32 regionOnClickColor = new Color32(32, 110, 110, 255);

    private RegionLittleSprites regionLittleSprites;

    //ActionInfoPanels which are children of this panel.
    private List<GameObject> actionInfoPanels = new List<GameObject>();

    public void Awake()
    {
        regionPanelRectTransform = regionPanel.GetComponent<RectTransform>();
        regionSprites = GetComponent<RegionSprites>();
        regionLittleSprites = GetComponent<RegionLittleSprites>();

        UI = GetComponent<UI>();

        GetRegionLittleImages();
        GetChildActionInfoPanels();
    }


    //TODO: it is dependent to the naming in editor. Search a way to change it.
    private void GetRegionLittleImages()
    {
        foreach (Transform child in regionPanel.transform)
        {
            if (child.name == "UpperPanel")
            {
                foreach (Transform grandchild in child)
                {
                    if (grandchild.name == "RegionImage")
                    {
                        regionLittleImageOnPanel = grandchild.gameObject.GetComponent<Image>();
                        return;
                    }
                }
            }
        }
    }


    /*
     * We need actionInfoPanels of our children.
     * To understand it let's create user story:
     *
     *     A user clicks on a region and a region info panel
     *     pops up. Then, user clicks on an action in that panel
     *     to see information about the action. And action info
     *     panel pops up. Finally, without clicking anywhere else,
     *     the user clicks outside of the region info panel.
     *
     * In that scenario when user opens up region info panel again,
     * he should not see action info panel that had previously opened.
     * Therefore, while closing the region info panel, we need to
     * check all children action info panels to see if there is any open 
     * one, and then we need to close that panel as well
     *
     * Algorithm:
     *  1 - Get all children Action gameObjects.
     *  2 - Get all actionInfoPanels of Actions. 
     */
    public void GetChildActionInfoPanels()
    {
        List<Transform> actionObjects = new List<Transform>();

        foreach (Transform child in regionPanel.transform)
        {
            if (child.tag.Equals("ActionObject"))
            {
                actionObjects.Add(child);
            }
        }

        foreach (Transform actionObject in actionObjects)
        {
            foreach (Transform child in actionObject)
            {
                if (child.tag == "ActionInfoPanel")
                {
                    actionInfoPanels.Add(child.gameObject);
                }
            }
        }
    }


    public void Update()
    {
        if (Input.GetMouseButton(0) && regionPanel.activeSelf)
        {
            Debug.Log("HERE");
            CheckRegionPanelToClose();
        }
    }

    public void OnClickRegion(string regionName)
    {
        RegionController regionController = UI.gameController.countryController.GetRegionByString(regionName);

        SpriteRenderer regionSpriteRenderer = regionSprites.GetRegionSprite(regionName);

        if (currentlyOpenRegionInPanel != null)
        {
            currentlyOpenRegionInPanel.color = regionBaseColor;
        }

        currentlyOpenRegionInPanel = regionSpriteRenderer;

        currentlyOpenRegionInPanel.color = regionOnClickColor;

        /*
        Text[] texts = InfoPanel.GetComponentsInChildren<Text>();
        int today = Time.GetInstance().GetDay();
        //TODO: create names!
        
        texts[1].text = regionController.activeCases[today - 1].ToString();
        texts[3].text = regionController.healthSystemModel.aggregateDeathCases[today - 1].ToString();
        texts[5].text = regionController.healthSystemModel.aggregateRecoveredCases[today - 1].ToString();
        */

        regionLittleImageOnPanel.sprite = regionLittleSprites.GetRegionSprite(regionName);

        regionPanel.SetActive(true);
    }

    public void CheckRegionPanelToClose()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(regionPanelRectTransform,
                                                                   Input.mousePosition,
                                                                   Camera.main))
        {
            regionPanel.SetActive(false);

            currentlyOpenRegionInPanel.color = regionBaseColor;
            currentlyOpenRegionInPanel = null;

            foreach (var panel in actionInfoPanels)
            {
                panel.SetActive(false);
            }

        }
    }
}