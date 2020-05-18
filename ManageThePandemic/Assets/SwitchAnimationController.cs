using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchAnimationController : MonoBehaviour
{
    public Animator anim;

    public ButtonController buttonController;

    public Canvas tutorialCanvas;

    public GameObject country;

    void Start()
    {
        buttonController.DisableButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("End"))
            {
                tutorialCanvas.gameObject.SetActive(false);
                buttonController.EnableButtons();
            }
            else
            {
                anim.SetTrigger("Switch");
            }
        }
    }       
}
