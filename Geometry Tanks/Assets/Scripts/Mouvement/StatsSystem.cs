using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public string[] particleTags; //Les tags des particules qui devront être libérées au moment de la mort du joueur, les grosses comme les petites (donc 8 particules)
    [HideInInspector] public PlayerMovement p;
    Transform t;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool partieTerminée;   //Appelée quand le match est fini pour que les joueurs ne bougent plus
    [HideInInspector] public int kills, deaths;

    public int curHealth, maxHealth = 30;

    // Start is called before the first frame update
    void Start()
    {
        GetScripts();
        ScoreManager.instance.InitializePlayerUI(p.joueurID);
    }


    private void Reset()
    {
        curHealth = maxHealth;
    }





    private void GetScripts()
    {
        p = GetComponent<PlayerMovement>();
        t = transform;

        curHealth = maxHealth;
    }








    //Mettre 0 par défaut pour les IAs (vu qu'elles n'ont pas d'ID)
    public void OnHit(int pts, int enemyID)
    {
        curHealth -= pts;

        if(curHealth < 0)
        {
            curHealth = 0;
            Die(enemyID);
        }
    }

    private void Die(int enemyID)
    {
        isDead = true;
        deaths++;

        Camera.main.GetComponent<CameraShake>().Shake();


        ScoreManager.instance.UpdateHealthUI(p.joueurID);
        ScoreManager.instance.UpdateKdrUI(p.joueurID);

        LooseAllExp(); //Videra tout l'exp d joueur et libérera des particules d'exp dans l'arène

        for (int i = 0; i < 3; i++)
        {
            ScoreManager.instance.UpdateExpUI(p.joueurID, i);
        }

        StartCoroutine(GameManager.instance.RespawnPlayerOnDeath(p.joueurID));
    }







    private void LooseAllExp()
    {
        for (int i = 0; i < 3; i++)
        {
            int experience = p.tousLesMeshsDuJoueur[i].GetComponent<Arme>().curExp;

            int nbDeGrossesParticules = 0;

            if((float)experience / 5f > 1f)
            {
                nbDeGrossesParticules = Mathf.FloorToInt((float)experience / 5f); //Si on a plus de 5 points d'expérience dans une arme, on spawn les grosses particules, sinon les petites
            }
            

            for (int j = 0; j < nbDeGrossesParticules; j++)
            {
                ObjectPooler.instance.SpawnFromPool(particleTags[i + 4], t.position, Quaternion.identity);  //Penser à implémenter l'interface IPooledObject pour ajouter des forces aux particules
                experience -= 5;
            }
            for (int j = 0; j < experience; j++)
            {
                ObjectPooler.instance.SpawnFromPool(particleTags[i], t.position, Quaternion.identity);
            }


            p.tousLesMeshsDuJoueur[i].GetComponent<Arme>().curExp = 0;
            p.tousLesMeshsDuJoueur[i].GetComponent<Arme>().isEvolved = false;
        }
    }
    






    public float GetKDR()
    {
        if (deaths == 0)
        {
            return (float)kills;
        }
        else
        {
            if (deaths > kills)
            {
                if (kills == 0)
                {
                    return (float)-deaths;
                }
                else
                {
                    return (Mathf.Round(((float)kills / (float)deaths) * 100f)) / 100f;
                }
            }
            else if (deaths == kills)
            {
                return 0f;
            }
            else
            {
                return (Mathf.Round(((float)kills / (float)deaths) * 100f)) / 100f;
            }
        }
    }


    private void OnEnable()
    {
        ScoreManager.instance.InitializePlayerUI(p.joueurID);
        Camera.main.GetComponent<MultipleTargetCam>().targets.Add(transform);
    }
    private void OnDisable()
    {
        Camera.main.GetComponent<MultipleTargetCam>().targets.Remove(transform);
    }


}
