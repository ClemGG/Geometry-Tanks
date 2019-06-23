using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {

    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private StandaloneInputModule inputModule;

    private SceneFader sf;

    private bool showRules = false;
    private bool showControls = false;


    


    // Use this for initialization
    void Start () {

        if(controlsPanel)
        controlsPanel.SetActive(false);

        if(rulesPanel)
        rulesPanel.SetActive(false);

        sf = FindObjectOfType<SceneFader>();
    }


    private void Update()
    {
        inputModule.verticalAxis = (showControls || showRules) ? "Vertical" : "Vertical1";
    }



    public void Play()
    {
        //GetComponent<AudioSource>().Play();
        sf.FadeToScene(1);
    }






    public void Rules()
    {
        if (showControls)
            return;

        showRules = !showRules;

        if (showRules)
            rulesPanel.SetActive(true);
        else
            rulesPanel.GetComponent<Animator>().SetTrigger("hideUI");
    }





    public void Controls()
    {

        if (showRules)
            return;

        showControls = !showControls;

        if (showControls)
            controlsPanel.SetActive(true);
        else
            controlsPanel.GetComponent<Animator>().SetTrigger("hideUI");

    }


    
    public void Quit()
    {
        sf.FadeToQuitScene();
    }
}
