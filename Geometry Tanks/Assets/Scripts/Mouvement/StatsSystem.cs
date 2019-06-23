using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public PlayerUI playerUI; //L'UI perso du joueur sur le terrain. Doit être placé tout en bas de la hiérarchie pour ne pas gêner la recherche des armes
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

        playerUI = t.GetChild(t.childCount - 1).GetComponent<PlayerUI>();
        playerUI.UpdateHealthUI();

        ScoreManager.instance.InitializePlayerUI(p.joueurID);
    }








    //Mettre 0 par défaut pour les IAs (vu qu'elles n'ont pas d'ID)
    public void OnHit(int pts, int enemyID, Enums.TypeArme typeDeProjectile)
    {
        //Si le projectile et le vaisseau sont de la même couleur ou si le joueur est touché par son propre projectile, alors on n'applique aucun dégât, et le projectile passe à travers le joueur
        if (typeDeProjectile == p.typeDuVaisseau || enemyID == p.joueurID)
            return;

        curHealth -= pts;

        playerUI.UpdateHealthUI();

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
        
        if(enemyID > 0) //Si l'ID est égal à 0, l'ennemi qui a tué le joueur est une IA, donc on ne change que le UI du joueur mort
            ScoreManager.instance.UpdateKdrUI(enemyID);

        LooseAllExp(); //Videra tout l'exp d joueur et libérera des particules d'exp dans l'arène
        
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
        //On remet à jour tous les UIs au moment de la mort et du respawn du joueur

        Camera.main.GetComponent<MultipleTargetCam>().targets.Add(transform);
    }
    private void OnDisable()
    {
        ScoreManager.instance.InitializePlayerUI(p.joueurID);
        //On remet à jour tous les UIs au moment de la mort et du respawn du joueur
        
        Camera.main.GetComponent<MultipleTargetCam>().targets.Remove(transform);
    }


}
