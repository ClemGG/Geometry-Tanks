using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatsSystem), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public int joueurID = 1;
    public Transform[] tousLesMeshsDuJoueur;  // A remplir dans l'inspector


    [HideInInspector] public StatsSystem s;
    [HideInInspector] public Transform t, meshToRotate;
    [HideInInspector] public Rigidbody rb;




    [Space(10)]
    [Header("Armes : ")]
    [Space(10)]

    public Enums.TypeArme typeDuVaisseau;
    [HideInInspector] public Arme armeActuelle;
    int indexMeshActuel = 0;






    [Space(10)]
    [Header("Inputs : ")]
    [Space(10)]


    public float moveSpeed = 10f;
    public float rotSpeed = 7f;
    public float moveSmooth = .15f;


    float hInput, vInput;
    bool x, o, v, b;
    Vector3 moveInput, rotInput, moveDir;

    float xVelocity, zVelocity;





    // Appelée dans Awake car le script StatsSystem a besoin de l'arme pour mettre à jour le UI du joueur
    private void Awake()
    {
        GetScripts();

    }


    private void Update()
    {
        if (s.isDead || s.partieTerminée)
            return;
        
        GetInput();

        //print($"X : {x}, O : {o}, V : {v}, B: {b}");

        if (x)
        {
            if (typeDuVaisseau != Enums.TypeArme.Bleu)
                ChangerArmeEtVaisseau(Enums.TypeArme.Bleu);
            else
                Tirer();
        }
        if (o)
        {
            if (typeDuVaisseau != Enums.TypeArme.Rouge)
                ChangerArmeEtVaisseau(Enums.TypeArme.Rouge);
            else
                Tirer();
        }
        if (v)
        {
            if (typeDuVaisseau != Enums.TypeArme.Jaune)
                ChangerArmeEtVaisseau(Enums.TypeArme.Jaune);
            else
                Tirer();
        }
        if (b)
        {
            if (typeDuVaisseau != Enums.TypeArme.Vert)
                ChangerArmeEtVaisseau(Enums.TypeArme.Vert);
            else
                Tirer();
        }
    }


    private void FixedUpdate()
    {
        if (s.isDead || s.partieTerminée)
            return;

        Move();
        Rotate();
    }






    private void GetScripts()
    {
        s = GetComponent<StatsSystem>();
        t = transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        ChangerArmeEtVaisseau(typeDuVaisseau);
    }






    private void GetInput()
    {
        hInput = Input.GetAxis("Horizontal" + joueurID.ToString());
        vInput = Input.GetAxis("Vertical" + joueurID.ToString());

        moveInput.x = Mathf.SmoothDamp(moveInput.x, hInput, ref xVelocity, moveSmooth);
        moveInput.z = Mathf.SmoothDamp(moveInput.z, vInput, ref zVelocity, moveSmooth);

        rotInput = new Vector3(hInput, 0f, vInput);

        if (rotInput != Vector3.zero)
        moveDir = rotInput.normalized;

        x = Input.GetButtonDown("Croix" + joueurID.ToString());
        o = Input.GetButtonDown("Cercle" + joueurID.ToString());
        v = Input.GetButtonDown("Triangle" + joueurID.ToString());
        b = Input.GetButtonDown("Carré" + joueurID.ToString());
        
    }



    public void ChangerArmeEtVaisseau(Enums.TypeArme nouveauTypeArme)
    {
        if(ObjectPooler.instance)
            s.SpawnPrefabsOnDeath();

        if(AudioManager.instance)
            AudioManager.instance.Play("changementVaisseau");

        typeDuVaisseau = nouveauTypeArme;

        if (tousLesMeshsDuJoueur == null)
        {
            Debug.LogError("Erreur : La liste des meshs du joueur est nulle");
            return;
        }


        switch (nouveauTypeArme)
        {
            case Enums.TypeArme.Bleu:
                indexMeshActuel = 0;
                break;
            case Enums.TypeArme.Rouge:
                indexMeshActuel = 1;
                break;
            case Enums.TypeArme.Jaune:
                indexMeshActuel = 2;
                break;
            case Enums.TypeArme.Vert:
                indexMeshActuel = 3;
                break;
        }

        /* On regarde d'abord si l'arme sélectionnée a été évoluée. Si c'est le cas, on ajoute 4 à l'index pour qu'il cherche les armes suivantes.
         * Sinon, on ne change l'index que dans le switch.
         */
        if (tousLesMeshsDuJoueur[indexMeshActuel].GetComponent<Arme>().isEvolved)
            indexMeshActuel += 4;



        for (int i = 0; i < tousLesMeshsDuJoueur.Length; i++)
        {
            if (i == indexMeshActuel)
            {
                meshToRotate = tousLesMeshsDuJoueur[i];
                armeActuelle = tousLesMeshsDuJoueur[i].GetComponent<Arme>();
            }

            Collider[] cols = tousLesMeshsDuJoueur[i].GetComponents<Collider>();

            for (int k = 0; k < cols.Length; k++)
            {
                cols[k].enabled = i == indexMeshActuel;
            }
            /*  On désactive tous les colliders des vaisseaux qui ne sont pas affichés à l'écran
             */

            for (int j = 0; j < tousLesMeshsDuJoueur[i].childCount; j++)
            {
                tousLesMeshsDuJoueur[i].GetChild(j).gameObject.SetActive(i == indexMeshActuel);
            }
            /* Finalement, on récupère tous les enfants du mesh actuel et on les désactive. Comme ça, on désactive collisions, meshs et trails tout en gardant l'arme activée
             */

        }
    }







    // Si l'arme que l'on porte actuellement vient d'évoluer, on change l'apparence actuelle du vaisseau et on utilise son arme évoluée
    public void OnWeaponEvolved(Arme armeEvoluée)
    {
        if(armeEvoluée.typeArme == armeActuelle.typeArme)
        {
            ChangerArmeEtVaisseau(armeEvoluée.typeArme);
        }
    }








    private void Move()
    {
        //t.Translate(moveInput * moveSpeed * Time.deltaTime);
        rb.MovePosition(t.position + moveInput * moveSpeed * Time.deltaTime);
    }


    private void Rotate()
    {
        for (int i = 0; i < tousLesMeshsDuJoueur.Length; i++)
        {

            //On oriente le joueur en fonction de la normale de la surface sur laquelle il marche
            Quaternion targetRotation = Quaternion.FromToRotation(tousLesMeshsDuJoueur[i].forward, moveDir) * tousLesMeshsDuJoueur[i].rotation;
            tousLesMeshsDuJoueur[i].rotation = Quaternion.Slerp(tousLesMeshsDuJoueur[i].rotation, targetRotation, rotSpeed * Time.deltaTime);
            tousLesMeshsDuJoueur[i].eulerAngles = new Vector3(0f, tousLesMeshsDuJoueur[i].eulerAngles.y, 0f);
        }
    }



    private void Tirer()
    {
        if(armeActuelle.peutTirer)
        {
            armeActuelle.Tirer();

            StartCoroutine(ScoreManager.instance.UpdateCooldownUI(joueurID, indexMeshActuel));
            //On va le garder en tant que coroutine, comme ça la fonction n'aura pas à chercher les UIs à chaque frame
        }
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
