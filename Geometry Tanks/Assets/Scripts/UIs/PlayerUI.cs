﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    public StatsSystem joueurCorrespondant;
    public TextMeshProUGUI joueurIDText;
    public Image barreDeVie;
    

    [Space(10)]

    public Color highHealthColor;
    public Color lowHealthColor;

    [Space(10)]

    public Color j1, j2, j3, j4;

    public void InitializeUI()
    {
        joueurIDText.text = "J" + joueurCorrespondant.p.joueurID;

        switch (joueurCorrespondant.p.joueurID)
        {
            case 1:
                joueurIDText.color = j1;
                break;
            case 2:
                joueurIDText.color = j2;
                break;
            case 3:
                joueurIDText.color = j3;
                break;
            case 4:
                joueurIDText.color = j4;
                break;
        }
    }


    private void OnEnable()
    {
        barreDeVie.fillAmount = 1f;
    }
    

    public void UpdateHealthUI()
    {
        barreDeVie.fillAmount = (float)joueurCorrespondant.curHealth / (float)joueurCorrespondant.maxHealth;
        barreDeVie.color = Color.Lerp(lowHealthColor, highHealthColor, barreDeVie.fillAmount);
    }

    public void InitializeUI(List<GameManager.Joystick> joysticks)
    {
        int nouvelIndex = 0;

        for (int i = 0; i < joysticks.Count; i++)
        {
            if (joueurCorrespondant.p.joueurID != joysticks[i].id)
                continue;

            nouvelIndex = i + 1;
            joueurIDText.text = "J" + (i+1).ToString();
        }


        switch (nouvelIndex)
        {
            case 1:
                joueurIDText.color = j1;
                break;
            case 2:
                joueurIDText.color = j2;
                break;
            case 3:
                joueurIDText.color = j3;
                break;
            case 4:
                joueurIDText.color = j4;
                break;
        }
    }
}
