using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arme : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public string bulletTag; //Le tag de du projectile à spawner
    Transform pointDeTir;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public Enums.TypeArme typeArme;
    public bool isEvolved = false, peutTirer = true;

    public int curExp, maxExp = 10;

    public float cadenceDeTir;
    [HideInInspector] public float timer;






    // Appelé dans l'Awake pour être sûr que les UIs soient bien mis à jour
    protected void Awake()
    {
        timer = cadenceDeTir;
        peutTirer = true;
    }


    protected void Update()
    {
        if(timer < cadenceDeTir)
        {
            timer += Time.deltaTime;
        }

        peutTirer = timer >= cadenceDeTir;
    }


    public void Tirer()
    {
        timer = 0f;
        peutTirer = false;

        Projectile p = ObjectPooler.instance.SpawnFromPool(bulletTag, pointDeTir.position, pointDeTir.rotation).GetComponent<Projectile>();


        PlayerMovement parent = transform.parent.GetComponent<PlayerMovement>();

        if(parent.CompareTag("Player"))
            p.projectileID = parent.joueurID;
        else
            p.projectileID = 0;

        p.rb.velocity = parent.rb.velocity;

    }

    public void AddExp(int pts)
    {
        curExp += pts;
        curExp = Mathf.Clamp(curExp, 0, maxExp);

        if(curExp == maxExp)
        {
            isEvolved = true;
        }
    }



    //Quand l'arme est désactivée, le joueur est mort. On la remet donc à jour pour pouvoir la réutiliser après le respawn
    private void OnDisable()
    {
        timer = cadenceDeTir;
        peutTirer = true;
    }
}
