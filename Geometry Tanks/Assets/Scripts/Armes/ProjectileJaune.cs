using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileJaune : Projectile
{
    Transform joueurASuivre, meshToRotate;

    public float rotSpeed = 20f;



    protected override void Start()
    {
        base.Start();
        meshToRotate = t.GetChild(0); //Pour faire tourner le missile. Les meshs / effets de particule des projectiles devront tous être enfants à la Transform portant le script Projectile

    }


    protected override void FixedUpdate()
    {
        joueurASuivre = ChercherJoueurLePlusProche();
        base.FixedUpdate();
        Rotate();
    }


    protected override void Rotate()
    {
        Vector3 dir = (joueurASuivre.position - t.position).normalized;

        //On oriente le mesh du missile en fonction de sa direction par rapport au joueur
        Quaternion targetRotation = Quaternion.FromToRotation(meshToRotate.forward, moveDir) * meshToRotate.rotation;
        meshToRotate.rotation = Quaternion.Slerp(meshToRotate.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }


    private Transform ChercherJoueurLePlusProche()
    {
        GameObject[] ias = GameObject.FindGameObjectsWithTag("IA");
        GameObject[] joueurs = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] entités = ias.Concat(joueurs).ToArray();

        float minDst = Mathf.Infinity;
        Transform joueurLePlusProche = null;

        for (int i = 0; i < entités.Length; i++)
        {
            float dst = (t.position - entités[i].transform.position).sqrMagnitude;

            if (dst < minDst)
            {

                if (entités[i].GetComponent<IAMovement>())
                {
                    IAMovement m = entités[i].GetComponent<IAMovement>();

                    //On filtre les entités de la même couleur que le missile, comme ça il ne va pas pourchasser celles qu'il ne peut pas toucher
                    //Il ne va pas chasser non plus d'autres IAs s'il a été tiré par une IA
                    if (m.typeDeCetteIA != Enums.TypeArme.Jaune && projectileID != 0) 
                    {
                        minDst = dst;
                        joueurLePlusProche = m.t;
                    }
                }
                else
                {
                    PlayerMovement p = entités[i].GetComponent<PlayerMovement>();

                    //On filtre les entités de la même couleur que le missile, comme ça il ne va pas pourchasser celles qu'il ne peut pas toucher
                    //Il ne va pas chasser non plus le joueur qui l'a tiré
                    if (p.typeDuVaisseau != Enums.TypeArme.Jaune && projectileID != p.joueurID)
                    {
                        minDst = dst;
                        joueurLePlusProche = p.t;
                    }
                }
            }
            
        }

        return joueurLePlusProche;
    }







    protected override void SpawnPrefabsOnDeath()
    {
        for (int i = 0; i < prefabsToSpawnOnDeath.Length; i++)
        {
            AOE aoe = ObjectPooler.instance.SpawnFromPool(prefabsToSpawnOnDeath[i], t.position, t.rotation).GetComponent<AOE>();

            if (aoe)
            {
                aoe.projectileID = projectileID;
                aoe.typeAOE = typeDeProjectile;
            }
        }
    }
}
