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

    [Space(10)]

    public string[] particlesToSpawnOnDeath; //Le tag de la particule qui devra être libérée au moment de la mort du joueur. Dépend de la couleur du joueur

    [Space(10)]

    [HideInInspector] public PlayerMovement p;
    Transform t;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool partieTerminée;   //Appelée quand le match est fini pour que les joueurs ne bougent plus
    [HideInInspector] public int kills, deaths;

    public int curHealth, maxHealth = 30;

    int timerAcide;
    Coroutine acideCoroutine;

    // Start is called before the first frame update
    void Start()
    {

        if (MultipleTargetCam.instance)
            MultipleTargetCam.instance.targets.Add(transform);

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

        if (ScoreManager.instance)
        {
            if (p.joueurID != 0)
            {
                ScoreManager.instance.InitializePlayerUI(p.joueurID);
                //On remet à jour tous les UIs au moment de la mort et du respawn du joueur
            }
        }
    }








    //Mettre 0 par défaut pour les IAs (vu qu'elles n'ont pas d'ID)
    public void OnHit(int pts, int enemyID, Enums.TypeArme typeDeProjectile)
    {
        //Si le projectile et le vaisseau sont de la même couleur ou si le joueur est touché par son propre projectile, alors on n'applique aucun dégât, et le projectile passe à travers le joueur
        if (typeDeProjectile == p.typeDuVaisseau || enemyID == p.joueurID)
            return;

        if (typeDeProjectile == Enums.TypeArme.Vert)

            curHealth -= pts;

        playerUI.UpdateHealthUI();

        if (curHealth < 0)
        {
            curHealth = 0;
            Die(enemyID);
        }
    }

    //Mettre 0 par défaut pour les IAs (vu qu'elles n'ont pas d'ID)
    public void OnHit(int pts, int enemyID, Enums.TypeArme typeDeProjectile, bool isEvolved)
    {
        //Si le projectile et le vaisseau sont de la même couleur ou si le joueur est touché par son propre projectile, alors on n'applique aucun dégât, et le projectile passe à travers le joueur
        if (typeDeProjectile == p.typeDuVaisseau || enemyID == p.joueurID)
            return;


        curHealth -= pts;
        playerUI.UpdateHealthUI();



        if (curHealth < 0)
        {

            if (acideCoroutine != null)
            {
                StopCoroutine(acideCoroutine);
                acideCoroutine = null;
            }

            curHealth = 0;
            Die(enemyID);
            
            return;
        }

        if (typeDeProjectile == Enums.TypeArme.Vert)
        {

            if(acideCoroutine == null)
            {
                timerAcide = isEvolved ? 10 : 7;
                acideCoroutine = StartCoroutine(DégâtsAcide(enemyID, typeDeProjectile, isEvolved));
            }
        }
    }

    private IEnumerator DégâtsAcide(int enemyID, Enums.TypeArme typeDeProjectile, bool isEvolved)
    {
        float timer = 0f;
        for (int i = 0; i < timerAcide; i++)
        {
            while(timer < 1f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f;
            OnHit(1, enemyID, Enums.TypeArme.Vert, isEvolved);
            yield return null;
        }
        
        acideCoroutine = null;
    }







    private void Die(int enemyID)
    {
        isDead = true;
        deaths++;

        //Camera.main.GetComponent<CameraShake>().Shake();
        
        if(enemyID > 0) //Si l'ID est égal à 0, l'ennemi qui a tué le joueur est une IA, donc on ne change que le UI du joueur mort
            ScoreManager.instance.UpdateKdrUI(enemyID);

        LooseAllExp(); //Videra tout l'exp d joueur et libérera des particules d'exp dans l'arène

        SpawnPrefabsOnDeath();
        StartCoroutine(GameManager.instance.RespawnPlayerOnDeath(p.joueurID));
    }







    private void SpawnPrefabsOnDeath()
    {
        ObjectPooler.instance.SpawnFromPool(particlesToSpawnOnDeath[(int)p.typeDuVaisseau], t.position, Quaternion.identity);
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
        p = GetComponent<PlayerMovement>();

        if (ScoreManager.instance)
        {
            if(p.joueurID != 0)
            {
                ScoreManager.instance.InitializePlayerUI(p.joueurID);
                //On remet à jour tous les UIs au moment de la mort et du respawn du joueur
            }
        }

        if(MultipleTargetCam.instance)
            MultipleTargetCam.instance.targets.Add(transform);
    }
    private void OnDisable()
    {
        if (ScoreManager.instance)
        {
            if (p.joueurID != 0)
            {
                ScoreManager.instance.InitializePlayerUI(p.joueurID);
                //On remet à jour tous les UIs au moment de la mort et du respawn du joueur
            }
        }

        if (MultipleTargetCam.instance)
            MultipleTargetCam.instance.targets.Remove(transform);
    }


}
