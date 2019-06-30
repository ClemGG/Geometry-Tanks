using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerProjectilsVertsV2 : MonoBehaviour
{
    Transform t;
    public string projectileVertTag;


    private void OnEnable()
    {
        t = transform;
        SpawnBullets();
        gameObject.SetActive(false);
    }

    private void SpawnBullets()
    {
        AudioManager.instance.Play("GreenExplosion");


        for (int i = 0; i < t.childCount; i++)
        {
            ObjectPooler.instance.SpawnFromPool(projectileVertTag, t.GetChild(i).position, t.GetChild(i).rotation);
        }
    }
}
