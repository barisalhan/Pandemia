using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    private TabGroup tabGroup;

    void Awake()
    {
        tabGroup = gameObject.GetComponentInParent<TabGroup>();
        tabGroup.AddToList(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
}
