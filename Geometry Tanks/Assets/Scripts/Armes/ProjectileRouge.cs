using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRouge : Projectile
{
    private List<Transform> ennemis;

    [Space(10)]

    public float radiusSpeed; //La vitesse à laquelle le collider va s'élargir en même temps que les particules
    float timer;
    public AnimationCurve radiusCurve;


    protected override void OnEnable()
    {
        base.OnEnable();
        timer = 0f;
        ennemis = new List<Transform>();
    }

    protected override void Move()
    {
        //L'onde de choc ne bouge pas car elle est attachée au joueur
        //Pas la peine de le détacher dans OnDisable je pense, car elle est rattachée au joueur suivant dans la même frame que son spawn
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        timer += Time.deltaTime;
        sc.radius = radiusCurve.Evaluate(timer) * radiusSpeed;

        if(Mathf.Approximately(radiusCurve.Evaluate(timer), 1f))
        {
            gameObject.SetActive(false);
        }
    }

    protected override void OnTriggerEnter(Collider c)
    {
        PlayerMovement p = c.transform.parent.GetComponent<PlayerMovement>();

        if (p)
        { 
            if(p.joueurID != projectileID && p.typeDuVaisseau != typeDeProjectile)
                ennemis.Add(c.transform.parent); //Puisqu'on met le trigger sur chacun des meshs, on va chercher le "Player" donc le parent
        }
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
        ennemis = new List<Transform>();
    }
}
