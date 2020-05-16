using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

/*
 * Controls the activeness of page areas.
 *
 * For ex: When "Social Policies" tab button is
 *         clicked, the related page is enabled
 *         and others(if any) are closed.
 *
 * The keystone in this class is selectedIndex.
 */ 
public class PageAreaController : MonoBehaviour
{
    private int selectedIndex = 0;

    [SerializeField]
    private GameObject pageGroup;

    private List<GameObject> actionPages = new List<GameObject>();

    private TabButton[] tabButtons;

    private List<Image> tabImages = new List<Image>();

    private Color32 tabBaseColor = new Color32(0, 0, 0, 255);
    private Color32 tabOnSelectedColor = new Color32(57, 136, 136, 255);


    public void Awake()
    {
        tabButtons = GetComponentsInChildren<TabButton>();

        GetActionPages();

        GetTabButtonImages();
    }


    public void Start()
    {
        foreach (GameObject actionPage in actionPages)
        {
            actionPage.SetActive(false);
        }

        // Activates first page when the game is started.
        OnTabSelected(tabButtons[selectedIndex]);
    }


    private void GetActionPages()
    {
        Transform pageAreaTransform = pageGroup.transform;

        foreach (Transform actionPage in pageAreaTransform)
        {
            actionPages.Add(actionPage.gameObject);
        }
    }

    private void GetTabButtonImages()
    {
        foreach (var tabButton in tabButtons)
        {
            tabImages.Add(tabButton.gameObject.GetComponent<Image>());
        }
    }


    /*
     * WARNING: This method assumes tabs are in the same
     *          order with pages in the editor!
     *
     * Algorithm:
     *      1- Close the currently open page.
     *      2- Set the color of previously selected tab to default.
     *      3- Open the new page.
     *      4- Set the color of selected tab.
     */
    public void OnTabSelected(TabButton tabButton)
    {
        actionPages[selectedIndex].SetActive(false);
        tabImages[selectedIndex].color = tabBaseColor;

        selectedIndex = tabButton.transform.GetSiblingIndex();

        actionPages[selectedIndex].SetActive(true);
        tabImages[selectedIndex].color = tabOnSelectedColor;
    }

   
}
