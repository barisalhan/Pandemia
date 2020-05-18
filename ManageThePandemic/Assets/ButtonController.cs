using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

    public List<Button> buttons = new List<Button>();

    public GameObject country;

    Button[] buttonsCountry;
    public void Awake()
    {
        buttonsCountry = country.GetComponentsInChildren<Button>();
    }

    public void EnableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        foreach (Button button in buttonsCountry)
        {
            button.interactable = true;
        }
    }

    public void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
        foreach (Button button in buttonsCountry)
        {
            button.interactable = false;
        }
    }
}
