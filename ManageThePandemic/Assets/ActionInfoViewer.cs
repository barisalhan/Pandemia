using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionInfoViewer : MonoBehaviour
{
    private ActionDataHolder actionDataHolder;
    private ActionData actionData;

    [SerializeField]
    public GameObject actionInfoPanel;

    public void Awake()
    {
        actionDataHolder = GetComponent<ActionDataHolder>();
        actionData = actionDataHolder.actionData;

        //TODO: It is dependent to the order of texts in the editor.
        actionInfoPanel.GetComponentInChildren<Text>().text = actionData.description;
    }


    public void OnClick()
    {
        actionInfoPanel.SetActive(true);       
    }
}
