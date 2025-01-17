﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class ExpParticle : MonoBehaviour, IPooledObject
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public string[] prefabsToSpawnOnDeath;


    [HideInInspector] public Transform t;
    SphereCollider c;
    Rigidbody rb;

    Transform joueurASuivre;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public Enums.TypeArme typeParticule;
    public bool donne5Pts;
    
    public float distanceDetection = 5f, duréeAvantActivation = .5f, moveSpeed = 20f, slowSpeed = .5f, forceOnSpawn = .5f;
    float timer;

    public AnimationCurve moveCurve, slowCurve;

    Coroutine co;

#if UNITY_EDITOR

    private void OnValidate()
    {
        GetScripts();
    }

    private void Reset()
    {
        GetScripts();
    }

#endif







    private void OnEnable()
    {
        joueurASuivre = null;
        GetScripts();

        if (co == null)
        co = StartCoroutine(ActiverDetectionAprèsDélai(true));

        SpawnPrefabsOnDeath();
    }


    // Start is called before the first frame update
    void Start()
    {
        GetScripts();

        if (co == null)
            co = StartCoroutine(ActiverDetectionAprèsDélai(true));
    }


    // Update is called once per frame
    void Update()
    {
        if (joueurASuivre)
        {
            if (!joueurASuivre.GetComponent<StatsSystem>().isDead)
            {
                if (co != null)
                {
                    StopCoroutine(co);
                    co = null;

                }

                MoveTowardsPlayer();
            }
        }
        else
        {
            if (co == null)
                co = StartCoroutine(ActiverDetectionAprèsDélai(true));
        }
    }









    //Ralentit la particule après son spawn, et active la détection de joueurs une fois stoppée
    private IEnumerator ActiverDetectionAprèsDélai(bool désactiverCollider)
    {
        c.enabled = !désactiverCollider;
        float timer = 0f;

        while (timer < duréeAvantActivation)
        {
            timer += Time.deltaTime;
            rb.velocity *= slowCurve.Evaluate(timer) * slowSpeed;
            yield return null;
        }

        c.enabled = true;
        co = null;
    }


    
    
    //Interface appelée par l'ObjectPooler
    public void OnObjectSpawn()
    {
        int alea = Random.Range(0, 2);

        float xForce = alea == 0 ? -forceOnSpawn : forceOnSpawn;
        float zForce = alea == 0 ? -forceOnSpawn : forceOnSpawn;

        Vector3 newForce = new Vector3(xForce, 0f, zForce);

        rb.AddForce(newForce, ForceMode.Impulse);

    }








    void GetScripts()
    {
        t = transform;
        c = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();


        c.radius = distanceDetection;
        c.isTrigger = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = false;
    }

    private void MoveTowardsPlayer()
    {
        int index = 0;

        switch (typeParticule)
        {
            case Enums.TypeArme.Bleu:
                index = 0;
                break;
            case Enums.TypeArme.Rouge:
                index = 1;
                break;
            case Enums.TypeArme.Jaune:
                index = 2;
                break;
            case Enums.TypeArme.Vert:
                index = 3;
                break;
        }

        Arme armeCorrespondante = joueurASuivre.GetChild(index).GetComponent<Arme>();

        if (!armeCorrespondante.isEvolved)
        {

            timer += Time.deltaTime;
            Vector3 moveDir = (joueurASuivre.position - t.position);
            Vector3 dir = (joueurASuivre.position - t.position).normalized;

            rb.MovePosition(t.position + dir * moveSpeed * moveCurve.Evaluate(timer));
            
            if (moveDir.magnitude < 1f)
            {
                armeCorrespondante.AddExp(donne5Pts ? 5 : 1);

                SpawnPrefabsOnDeath();

                if(co != null)
                {
                    StopCoroutine(co);
                    co = null;
                }

                gameObject.SetActive(false);
            }
        }
        else
        {
            joueurASuivre = null;

            if (co == null)
                co = StartCoroutine(ActiverDetectionAprèsDélai(false));

        }
    }




    private void SpawnPrefabsOnDeath()
    {
        for (int i = 0; i < prefabsToSpawnOnDeath.Length; i++)
        {
            ObjectPooler.instance.SpawnFromPool(prefabsToSpawnOnDeath[i], t.position, Quaternion.identity);
        }
    }




    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            int index = 0;

            switch (typeParticule)
            {
                case Enums.TypeArme.Bleu:
                    index = 0;
                    break;
                case Enums.TypeArme.Rouge:
                    index = 1;
                    break;
                case Enums.TypeArme.Jaune:
                    index = 2;
                    break;
                case Enums.TypeArme.Vert:
                    index = 3;
                    break;
            }

            Arme armeCorrespondante = col.transform.parent.GetChild(index).GetComponent<Arme>();

            if (!armeCorrespondante.isEvolved)
            {
                joueurASuivre = col.transform.parent;
                c.enabled = false;
            }
        }

    }


}
