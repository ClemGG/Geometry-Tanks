using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AOE : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]
    
    [HideInInspector] public int projectileID;  //A remplir depuis le script Arme
    protected SphereCollider sc;




    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public bool isEvolved;
    public Enums.TypeArme typeAOE;
    public string[] collisionTags;   //Le type d'ennemi qu'il peut toucher (joueur ou IA, ou les deux)
    public int dégâts;

    [Space(10)]

    public float duréeDeVie = .2f, radius;
    float timer;
    


#if UNITY_EDITOR

    protected void OnValidate()
    {
        OnEnable();
    }

    protected void Reset()
    {
        OnEnable();
    }

#endif



    private void OnEnable()
    {
        if(!sc)
            sc = GetComponent<SphereCollider>();
        
        sc.isTrigger = true;
        sc.radius = radius;
    }

    private void Update()
    {
        if(timer < duréeDeVie)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerEnter(Collider c)
    {

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
                            s.OnHit(dégâts, projectileID, typeAOE, isEvolved); //Puisqu'on met le trigger sur chacun des meshs, on va chercher le "Player" donc le parent
                            
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
                            s.OnHit(dégâts, typeAOE, isEvolved); //Puisqu'on met le trigger sur chacun des meshs, on va chercher le "Player" donc le parent
                            
                        }
                    }
                }

            }
        }
    }
}
