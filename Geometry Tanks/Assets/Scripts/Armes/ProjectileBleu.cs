using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBleu : Projectile
{

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isEvolved)
        {
            RayCast();  //Si le projectile bleu vient d'une arme évoluée, on le fait rebondir sur les murs de l'arène
        }
    }



    private void RayCast()
    {
        if(Physics.Raycast(t.position, t.forward, out RaycastHit hit, .5f, LayerMask.NameToLayer("Wall")))
        {
            rb.velocity = Vector3.Reflect(rb.velocity, hit.normal);
        }
    }
}
