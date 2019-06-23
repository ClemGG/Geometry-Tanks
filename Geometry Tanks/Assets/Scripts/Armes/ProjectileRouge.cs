using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRouge : Projectile
{
    private List<Transform> ennemis;
    

    protected override void OnTriggerEnter(Collider c)
    {
        ennemis.Add(c.transform);
        base.OnTriggerEnter(c);
    }


    protected override void SpawnPrefabsOnDeath()
    {
        base.SpawnPrefabsOnDeath();

        if (isEvolved)  //Si l'onde de choc rouge vient d'une arme évoluée, on fait spawner des ondes de choc plus faibles à l'endroit où les ennemis se trouvent
        {
            for (int i = 0; i < ennemis.Count; i++)
            {
                ProjectileRouge pr = ObjectPooler.instance.SpawnFromPool("projectileRougeRépliquat", ennemis[i].position, Quaternion.identity).GetComponent<ProjectileRouge>();
                pr.projectileID = projectileID; 
            }
        }
        ennemis.Clear();
    }
}
