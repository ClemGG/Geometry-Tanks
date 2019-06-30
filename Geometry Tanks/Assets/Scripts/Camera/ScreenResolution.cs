using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : MonoBehaviour
{



    private void Start()
    {
        Screen.SetResolution(1080, 1080, true);
    }

    private void Awake()
    {
        Screen.SetResolution(1080, 1080, true);
    }

    private void Update()
    {
        if(Screen.currentResolution.width != 1080)
        {
            Start();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Start();
    }
}
