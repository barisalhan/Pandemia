using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    private PageAreaController pageAreaController;

    public void Awake()
    {
        pageAreaController = GetComponentInParent<PageAreaController>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        pageAreaController.OnTabSelected(this);
    }
}
