using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Gets the data from action data and 
 * shows it on action info panel.
 */
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
        Text[] texts = actionInfoPanel.GetComponentsInChildren<Text>();
        texts[0].text = actionData.description;
        texts[1].text = "<i>Hint:</i> " + actionData.hint;
        if (actionData.cost != 0)
        {
            texts[2].text = actionData.cost.ToString() + " M$";
        }
        else
        {
            texts[2].text = "FREE";
        }
    }


    public void OnClick()
    {
        actionInfoPanel.SetActive(true);
        gameObject.transform.SetAsLastSibling();
    }
}
