using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVert : Projectile
{


    protected override void OnTriggerEnter(Collider c)
    {


        if (c.CompareTag("Wall"))
        {
            SpawnPrefabsOnDeath();
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < collisionTags.Length; i++)
        {
            if (c.CompareTag("Player"))
            {
                StatsSystem s = c.transform.transform.parent.GetComponent<StatsSystem>();

                if (s)
                {
                    if (s.p.joueurID != projectileID)
                    {

                        SpawnPrefabsOnDeath();
                        gameObject.SetActive(false);
                    }
                }
            }
            else if (c.CompareTag("IA"))
            {
                IAStats s = c.GetComponent<IAStats>();

                if (s)
                {
                    if (projectileID != 0)
                    {
                        SpawnPrefabsOnDeath();
                        gameObject.SetActive(false);
                    }
                }
            }
        }
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
