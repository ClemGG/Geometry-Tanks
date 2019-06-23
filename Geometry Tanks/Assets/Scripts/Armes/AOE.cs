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
                    c.GetComponent<StatsSystem>().OnHit(dégâts, projectileID, typeAOE);
                }
                else if (c.CompareTag("IA"))
                {
                    c.GetComponent<IAStats>().OnHit(dégâts, typeAOE);
                }
                
            }
        }
    }
}
