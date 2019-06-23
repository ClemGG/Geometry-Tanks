using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(IAStats), typeof(Rigidbody))]
public class IAMovement : MonoBehaviour
{
    
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [HideInInspector] public IAStats s;
    [HideInInspector] public Transform t, meshToRotate;
    Rigidbody rb;
    GameManager gameManager;

    PlayerMovement joueurASuivre;



    [Space(10)]
    [Header("Armes : ")]
    [Space(10)]

    public Enums.TypeArme typeDeCetteIA;
    [HideInInspector] public Arme arme;


    [Space(10)]
    [Header("Inputs : ")]
    [Space(10)]


    public float moveSpeed = 5f;
    public float rotSpeed = 4f;
    public float moveSmooth = .15f;

    [Space(10)]

    public float dstPoursuite = 10f;
    public float dstStop = 5f;
    public float dstFuite = 4f;
    public float dstAuSpawner = 15f; //Pour éviter que l'IA ne s'éloigne trop du spawner

    [Space(10)]

    Vector3 moveInput, rotInput, moveDir, distanceAuJoueurLePlusProche;
    [HideInInspector] public Vector3 spawnPos; //On récupère la position de l'enemySpawner propre à cet ennemi, pour éviter qu'il s'en éloigne de trop, comme ça, pas de calcul d'obstacle à faire

    float xVelocity, zVelocity;





    private void OnEnable()
    {
        InvokeRepeating("ChercherJoueurLePlusProche", 0f, .1f);
    }
    private void OnDisable()
    {
        CancelInvoke();
    }


    // Start is called before the first frame update
    void Start()
    {
        GetScripts();
    }

    // Update is called once per frame
    void Update()
    {
        if (s.partieTerminée)
            return;


        GetInput();

        if (distanceAuJoueurLePlusProche.sqrMagnitude < dstPoursuite * dstPoursuite)
        {
            Tirer();
        }        
    }



    private void FixedUpdate()
    {
        if (s.partieTerminée)
            return;

        
        Move();

        if (distanceAuJoueurLePlusProche.sqrMagnitude < dstPoursuite * dstPoursuite)
        {
            Rotate();
        }
    }





    private void GetScripts()
    {
        gameManager = GameManager.instance;
        
        s = GetComponent<IAStats>();
        t = transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        int i = 0;

        switch (typeDeCetteIA)
        {
            case Enums.TypeArme.Bleu:
                i = 0;
                break;
            case Enums.TypeArme.Rouge:
                i = 1;
                break;
            case Enums.TypeArme.Jaune:
                i = 2;
                break;
            case Enums.TypeArme.Vert:
                i = 3;
                break;
        }

        meshToRotate = t.GetChild(i);
        arme = meshToRotate.GetComponent<Arme>();

        for (int j = 0; j < t.childCount; j++)
        {
            t.GetChild(j).gameObject.SetActive(i == j);
        }
    }


    private void GetInput()
    {


        //Si on a pas de cible, pas la peine de faire tout ça, on se contente juste de ralentir le joueur
        if (!joueurASuivre)
        {
            moveInput.x = Mathf.SmoothDamp(moveInput.x, 0f, ref xVelocity, moveSmooth);
            moveInput.z = Mathf.SmoothDamp(moveInput.z, 0f, ref zVelocity, moveSmooth);
            return;
        }



        distanceAuJoueurLePlusProche = joueurASuivre.t.position - t.position;
        Vector3 dirNormalized = Vector3.ClampMagnitude(distanceAuJoueurLePlusProche, 1f);


        //C'est pas un déplacement parfait, vu que si on est trop près du point de spawn, l'IA peut nous rentrer dedans
        //Mais bon, c'est pas gravissime, au moins on peut toujours le ramener dans sa zone s'il est trop loin

        //Si le joueur est à portée et qu'on n'est pas trop loin, ou que le joueur est plus proche du spawner que nous, on avance
        if (distanceAuJoueurLePlusProche.sqrMagnitude < dstPoursuite * dstPoursuite && 
            distanceAuJoueurLePlusProche.sqrMagnitude > dstStop * dstStop || 
            (t.position - spawnPos).sqrMagnitude > (joueurASuivre.t.position - spawnPos).sqrMagnitude)
        {

            moveInput.x = Mathf.SmoothDamp(moveInput.x, dirNormalized.x, ref xVelocity, moveSmooth);
            moveInput.z = Mathf.SmoothDamp(moveInput.z, dirNormalized.z, ref zVelocity, moveSmooth);
        }

        //Si on est trop loin du joueur, du spawnPos ou qu'on est dans la limite d'approche du joueur, on s'arrête
        else if ((t.position - spawnPos).sqrMagnitude > dstAuSpawner * dstAuSpawner || distanceAuJoueurLePlusProche.sqrMagnitude > dstPoursuite * dstPoursuite ||
                distanceAuJoueurLePlusProche.sqrMagnitude < dstStop * dstStop && distanceAuJoueurLePlusProche.sqrMagnitude > dstFuite * dstFuite)
        {
            moveInput.x = Mathf.SmoothDamp(moveInput.x, 0f, ref xVelocity, moveSmooth);
            moveInput.z = Mathf.SmoothDamp(moveInput.z, 0f, ref zVelocity, moveSmooth);
        }

        //Si le joueur est trop près et qu'on n'est pas trop loin du spawner, ou que le joueur est plus loin du spawner que nous, on recule
        else if(distanceAuJoueurLePlusProche.sqrMagnitude < dstFuite * dstFuite && 
            ((t.position - spawnPos).sqrMagnitude < dstAuSpawner * dstAuSpawner || 
            (t.position - spawnPos).sqrMagnitude < (joueurASuivre.t.position - spawnPos).sqrMagnitude))
        {

            moveInput.x = Mathf.SmoothDamp(moveInput.x, -dirNormalized.x, ref xVelocity, moveSmooth);
            moveInput.z = Mathf.SmoothDamp(moveInput.z, -dirNormalized.z, ref zVelocity, moveSmooth);
        }



        rotInput = new Vector3(dirNormalized.x, 0f, dirNormalized.z);

        if (rotInput != Vector3.zero)
            moveDir = rotInput.normalized;

    }


    private void Move()
    {
        //t.Translate(moveInput * moveSpeed * Time.deltaTime);
        rb.MovePosition(t.position + moveInput * moveSpeed * Time.deltaTime);
    }


    private void Rotate()
    {
        //On oriente le joueur en fonction de la normale de la surface sur laquelle il marche
        Quaternion targetRotation = Quaternion.FromToRotation(meshToRotate.forward, moveDir) * meshToRotate.rotation;
        meshToRotate.rotation = Quaternion.Slerp(meshToRotate.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }


    private void Tirer()
    {


        if (arme.peutTirer)
        {
            arme.Tirer();
        }
    }



    private void ChercherJoueurLePlusProche()
    {
        float minDst = Mathf.Infinity;
        PlayerMovement joueurLePlusProche = null;

        for (int i = 0; i < gameManager.joueurs.Count; i++)
        {
            PlayerMovement p = gameManager.joueurs[i].p;

            float dst = (t.position - p.t.position).sqrMagnitude;
            if(dst < minDst)
            {
                if (p.typeDuVaisseau != typeDeCetteIA) //On filtre les joueurs de la même couleur que cette IA, comme ça elle ne va pas pourchasser des joueurs qu'elle ne peut pas toucher
                {
                    minDst = dst;
                    joueurLePlusProche = p;
                }
            }
        }

        joueurASuivre = joueurLePlusProche;

    }






#if UNITY_EDITOR

    private void Reset()
    {
        GetScripts();
    }
    private void OnValidate()
    {
        GetScripts();
    }

#endif
}
