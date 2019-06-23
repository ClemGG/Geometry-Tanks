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



        if (x)
        {
            if (typeDuVaisseau != armeActuelle.typeArme)
                ChangerArmeEtVaisseau(Enums.TypeArme.Bleu);
            else
                Tirer();
        }
        else if (o)
        {
            if (typeDuVaisseau != armeActuelle.typeArme)
                ChangerArmeEtVaisseau(Enums.TypeArme.Rouge);
            else
                Tirer();
        }
        else if (v)
        {
            if (typeDuVaisseau != armeActuelle.typeArme)
                ChangerArmeEtVaisseau(Enums.TypeArme.Jaune);
            else
                Tirer();
        }
        else if (b)
        {
            if (typeDuVaisseau != armeActuelle.typeArme)
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

    private void ChangerArmeEtVaisseau(Enums.TypeArme nouveauTypeArme)
    {

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

            MeshRenderer[] meshes = tousLesMeshsDuJoueur[i].GetComponentsInChildren<MeshRenderer>();

            for (int j = 0; j < meshes.Length; j++)
            {
                meshes[j].enabled = i == indexMeshActuel;
            }
            /* Au lieu de désactiver les meshs et leurs armes avec, on ne désactive que les meshRenderers et on garde les armes activées.
             * Comme ça elles pourront continuer de calculer leur cooldown seules sans crainte d'interruption, et elles ne seront pas appelées puisqu'on change d'arme en même temps que de mesh.
             */

        }
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

#endif


}
