using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVert : Projectile
{

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
