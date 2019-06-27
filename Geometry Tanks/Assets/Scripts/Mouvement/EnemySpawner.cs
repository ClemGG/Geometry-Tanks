using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class EnemySpawner : MonoBehaviour
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public string enemyTagToSpawn;
    public string[] particleOnSpawnTag;
    IAMovement[] ennemisSpawnés; //Les ennemis spawnés vont ête stockés temporairement pour le calcul de allDead, puis seront remplacés par les nouveaux ennemis au prochain spawn
    [HideInInspector] public Transform t;


    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public int nbEnnemisASpawner = 3;
    public float radius = 5f;

    public float cooldownTimer = 20f;
    float timer;

    public bool allDead, cooldownDone;






#if UNITY_EDITOR

    private void OnValidate()
    {
        GetComponent<SphereCollider>().radius = radius*2f;  //On instanciera les IAs dans le radius, et la détection du joueur sera dans le double de ce radius, pour ne pas surprendre le joueur
        timer = cooldownTimer / 2f;
    }

    private void Reset()
    {
        GetComponent<SphereCollider>().radius = radius*2f;
        timer = cooldownTimer / 2f;
    }


#endif


    private void Start()
    {
        t = transform;
        ennemisSpawnés = new IAMovement[0];

        SendReadyMessage();
    }


    // Update is called once per frame
    void Update()
    {
        if(timer < cooldownTimer)
        {
            timer += Time.deltaTime;
        }
        else
        {
            cooldownDone = true;
        }
    }

    public void SendReadyMessage()  //Appelé par les IAs à leur mort. Si elles sont toutes mortes, alors le spawner est prêt à les respawner
    {
        for (int i = 0; i < ennemisSpawnés.Length; i++)
        {
            if (ennemisSpawnés[i].gameObject.activeSelf)
            {
                allDead = false;
                return;
            }
        }

        allDead = true;
    }


    private void SpawnEnemies()
    {
        int alea = Random.Range(0, 4);
        ennemisSpawnés = new IAMovement[nbEnnemisASpawner];
        Enums.TypeArme typeAlea = Enums.TypeArme.Bleu;

        switch (alea)
        {
            case 0:
                typeAlea = Enums.TypeArme.Bleu;
                break;
            case 1:
                typeAlea = Enums.TypeArme.Rouge;
                break;
            case 2:
                typeAlea = Enums.TypeArme.Jaune;
                break;
            case 3:
                typeAlea = Enums.TypeArme.Vert;
                break;
        }


    


        for (int i = 0; i < nbEnnemisASpawner; i++)
        {
            Vector3 v = t.position + Random.insideUnitSphere * radius;

            ObjectPooler.instance.SpawnFromPool(particleOnSpawnTag[alea], new Vector3(v.x, 0f, v.z), Quaternion.identity);

            ennemisSpawnés[i] = ObjectPooler.instance.SpawnFromPool(enemyTagToSpawn, new Vector3(v.x, 0f, v.z), Quaternion.identity).GetComponent<IAMovement>();
            ennemisSpawnés[i].spawner = this;

            ennemisSpawnés[i].typeDeCetteIA = typeAlea;
            ennemisSpawnés[i].GetScripts();

        }
    }

    //On Trigger Enter est appelée quand CET objet entre dans un trigger, peu importe si notre collider est trigger ou non
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            if (cooldownDone && allDead)
            {
                SpawnEnemies();

                cooldownDone = allDead = false;
                timer = 0f;
            }
        }
    }
}
