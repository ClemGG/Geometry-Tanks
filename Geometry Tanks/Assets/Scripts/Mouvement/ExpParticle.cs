using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class ExpParticle : MonoBehaviour, IPooledObject
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]
    
    [HideInInspector] public Transform t;
    SphereCollider c;
    Rigidbody rb;

    Transform joueurASuivre;



    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public Enums.TypeArme typeParticule;
    bool donne5Pts;
    
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
        c.enabled = true;
        joueurASuivre = null;
        timer = 0f;

        if(co == null)
        co = StartCoroutine(ActiverDetectionAprèsDélai());
    }


    // Start is called before the first frame update
    void Start()
    {
        GetScripts();

        if (co == null)
            co = StartCoroutine(ActiverDetectionAprèsDélai());
    }


    // Update is called once per frame
    void Update()
    {
        if (joueurASuivre)
        {
            MoveTowardsPlayer();
        }
    }










    private IEnumerator ActiverDetectionAprèsDélai()
    {
        c.enabled = false;

        float timer = 0f;

        while(timer < duréeAvantActivation)
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

        float xForce = Random.Range(-forceOnSpawn, forceOnSpawn);
        float zForce = Random.Range(-forceOnSpawn, forceOnSpawn);

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
        timer += Time.deltaTime;
        Vector3 moveDir = (joueurASuivre.position - t.position).normalized;
        Vector3 dir = (joueurASuivre.position - t.position).normalized;

        rb.MovePosition(t.position + dir * moveSpeed * moveCurve.Evaluate(timer));

        if(moveDir.sqrMagnitude < .1f * .1f)
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
            armeCorrespondante.AddExp(donne5Pts ? 5 : 1);
            gameObject.SetActive(false);
        }
    }










    



    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            joueurASuivre = col.transform;
        }

        c.enabled = false;
    }


}
