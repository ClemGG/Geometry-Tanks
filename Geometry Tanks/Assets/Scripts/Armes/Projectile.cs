using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public string[] prefabsToSpawnOnDeath;

    [HideInInspector] public int projectileID;  //A remplir depuis le script Arme
    protected Transform t;
    [HideInInspector] public Rigidbody rb;
    protected SphereCollider sc;




    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public bool isEvolved;  //Nous permettra de partager le code entre les projectiles normaux et leurs évolutions
    public Enums.TypeArme typeDeProjectile;
    public string[] collisionTags;   //Le type d'ennemi qu'il peut toucher (joueur ou IA, ou les deux)

    [Space(10)]

    public int dégâts;

    [Space(10)]

    public AnimationCurve moveCurve; //Si la vitesse doit être constante, mettre une curve uniforme
    public float duréeDeVie, moveSpeed;
    protected float moveTimer, duréeTimer; //Utilisé pour évaluer moveCurve;


    protected Vector3 moveDir;




#if UNITY_EDITOR

    protected void OnValidate()
    {
        GetScripts();
    }

    protected void Reset()
    {
        GetScripts();
    }

#endif





    protected virtual void OnEnable()
    {
        moveTimer = 0f;
        duréeTimer = 0f;
    }



    // Start is called before the first frame update
    protected virtual void Start()
    {
        GetScripts();
    }


    protected void GetScripts()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();

        sc.isTrigger = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;

    }







    protected virtual void Update()
    {
        if(duréeTimer < duréeDeVie)
        {
            duréeTimer += Time.deltaTime;
        }
        else
        {
            SpawnPrefabsOnDeath();
            gameObject.SetActive(false);
        }
    }



    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Move();
    }



    protected virtual void Move()
    {
        moveTimer += Time.deltaTime;
        rb.MovePosition(t.position + t.forward * moveCurve.Evaluate(moveTimer) * moveSpeed /* * Time.deltaTime*/);
    }

    protected virtual void OnTriggerEnter(Collider c)
    {


        if (c.CompareTag("Wall"))
        {
            SpawnPrefabsOnDeath();
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < collisionTags.Length; i++)
        {
            if (c.CompareTag(collisionTags[i]))
            {
                if (c.CompareTag("Player"))
                {
                    StatsSystem s = c.transform.transform.parent.GetComponent<StatsSystem>();

                    if (s)
                    {
                        if (s.p.joueurID != projectileID)
                        {
                            s.OnHit(dégâts, projectileID, typeDeProjectile); //Puisqu'on met le trigger sur chacun des meshs, on va chercher le "Player" donc le parent

                            if (s.p.typeDuVaisseau != typeDeProjectile)
                            {
                                SpawnPrefabsOnDeath();
                                gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else if(c.CompareTag("IA"))
                {
                    IAStats s = c.GetComponent<IAStats>();

                    if (s)
                    {
                        if (projectileID != 0)
                        {
                            s.OnHit(dégâts, typeDeProjectile); //Puisqu'on met le trigger sur chacun des meshs, on va chercher le "Player" donc le parent


                            if (s.m.typeDeCetteIA != typeDeProjectile)
                            {
                                SpawnPrefabsOnDeath();
                                gameObject.SetActive(false);
                            }
                        }
                    }
                }

            }
        }
    }
    

    protected virtual void SpawnPrefabsOnDeath()
    {
        for (int i = 0; i < prefabsToSpawnOnDeath.Length; i++)
        {
            ObjectPooler.instance.SpawnFromPool(prefabsToSpawnOnDeath[i], t.position, t.rotation);
        }
    }
}
