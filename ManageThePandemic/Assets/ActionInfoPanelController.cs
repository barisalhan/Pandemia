using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


/*
 * It closes the info panel when it is clicked outside.
 */
public class ActionInfoPanelController : MonoBehaviour
{
    private RectTransform panelRectTransform;

    public void Awake()
    {
        panelRectTransform = gameObject.GetComponent<RectTransform>();
    }


    public void Update()
    {
        if (Input.GetMouseButton(0) && gameObject.activeSelf)
        {
            HideIfClickedOutside();
        }
    }


    private void HideIfClickedOutside()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform,
                                                               Input.mousePosition,
                                                               Camera.main))
        {
            gameObject.SetActive(false);
        }
    }
}
