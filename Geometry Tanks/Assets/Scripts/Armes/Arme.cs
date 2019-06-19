using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Arme : MonoBehaviour
{
    public Enums.TypeArme typeArme;
    public bool isEvolved = false;

    public int curExp, maxExp = 10;

    public float cadenceDeTir;
    [HideInInspector] public float timer;   //Diminué dans ScoreManager



    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }


    public abstract void Tirer();
}
