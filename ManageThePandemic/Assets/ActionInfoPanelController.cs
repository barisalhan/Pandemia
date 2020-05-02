using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ActionInfoPanelController : MonoBehaviour
{
    private RectTransform panelRectTransform;

    public void Awake()
    {
        panelRectTransform = gameObject.GetComponent<RectTransform>();
    }


    public void Update()
    {
        HideIfClickedOutside(gameObject);
    }


    private void HideIfClickedOutside(GameObject panel)
    {
        if (Input.GetMouseButton(0) && panel.activeSelf)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform,
                                                                   Input.mousePosition,
                                                                   Camera.main))
            {
                panel.SetActive(false);
            }
        }
    }
}
