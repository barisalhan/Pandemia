using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabGroup : MonoBehaviour
{

    private List<TabButton> tabButtons = new List<TabButton>();

    private TabButton selectedTab;

    private List<GameObject> actionPages = new List<GameObject>();

    [SerializeField]
    private GameObject pageArea;

    public void Awake()
    {
        Transform pageAreaTransform = pageArea.transform;

        foreach (Transform actionPage in pageAreaTransform)
        {
            actionPages.Add(actionPage.gameObject);
        }
    }
    public void AddToList(TabButton tabButton)
    {
        tabButtons.Add(tabButton);
    }

    public void OnTabSelected(TabButton tabButton)
    {
        int tabIndex = tabButton.transform.GetSiblingIndex();

        // This method assumes tabs are in the same order with pages

        for (int pageIndex = 0; pageIndex < actionPages.Count; pageIndex++)
        {
            if (pageIndex == tabIndex)
            {
                actionPages[pageIndex].SetActive(true);
            }
            else
            {
                actionPages[pageIndex].SetActive(false);
            }
        }
    }

   
}
