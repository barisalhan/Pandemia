﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Canvas countryCanvas;

    [SerializeField]
    private Canvas midCanvas;

    [SerializeField]
    private Canvas frontCanvas;

    public GameObject mainGameObject;

    [HideInInspector]
    public GameController gameController;

    [SerializeField]
    private GameObject InfoPanel;

    [SerializeField]
    private GameObject ActionTreePanel;

    private BarController barController;

    private UpperPanelController upperPanelController;

    public void Awake()
    {
        gameController = mainGameObject.GetComponent<GameController>();
        barController = GetComponent<BarController>();
        upperPanelController = GetComponent<UpperPanelController>();

        SubscribeBarToSocietyModel();
        SubscribeUpperPanelToBudget();
    }


    public void Start()
    {
        frontCanvas.gameObject.SetActive(false);
    }

    private void SubscribeBarToSocietyModel()
    {
        gameController.countryController.societyModel.HappinessChanged += barController.OnHappinesChanged;
    }

    private void SubscribeUpperPanelToBudget()
    {
      //TODO: implement here.
    }


    public void OnClickOpenActionTreePanel()
    {
        countryCanvas.gameObject.SetActive(false);
        midCanvas.gameObject.SetActive(false);
        frontCanvas.gameObject.SetActive(true);
    }

    public void OnClickCloseActionTreePanel()
    {
        countryCanvas.gameObject.SetActive(true);
        midCanvas.gameObject.SetActive(true);
        frontCanvas.gameObject.SetActive(false);
    }

}
