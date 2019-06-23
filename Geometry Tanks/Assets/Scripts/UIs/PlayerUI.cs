using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public StatsSystem joueurCorrespondant;
    public TextMeshProUGUI joueurIDText;
    public Image barreDeVie;
    

    [Space(10)]

    public Color highHealthColor;
    public Color lowHealthColor;

    private void Awake()
    {
        joueurIDText.text = "J" + joueurCorrespondant.p.joueurID;
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

}
