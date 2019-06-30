
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public GameObject playerPrefab; // On instancie les joueurs au début du jeu au lieu de les garder dans la scène (à tester)
    public Transform[] spawnPoints;
    public List<StatsSystem> joueurs;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public float respawnTime = 3f;


    public static GameManager instance;



    public struct Joystick
    {
        public string name;
        public int id;

        public Joystick(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
    public List<Joystick> joysticks;









    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
        SpawnJoueurs();

    }

    //private void Update()
    //{
    //    print(Input.GetJoystickNames().Length);
    //    for (int i = 0; i < Input.GetJoystickNames().Length; i++)
    //    {
    //        print($"Manette n°{i}, nom : \"{Input.GetJoystickNames()[i]}\".");
    //    }
    //}

    private void SpawnJoueurs()
    {
        joysticks = new List<Joystick>();
        int jIndex = 0;

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (jIndex <= 3 && Input.GetJoystickNames()[i] != string.Empty)
            {
                Joystick j = new Joystick(Input.GetJoystickNames()[i], i+1);
                joysticks.Add(j);

                StatsSystem s = Instantiate(playerPrefab, GetComponentRandomSpawnPoint(), Quaternion.identity).GetComponent<StatsSystem>();
                s.p.joueurID = j.id;
                joueurs.Add(s);

                s.playerUI.InitializeUI(joysticks);
                jIndex++; //On l'incrémente à la fin. Comme ça, si on a 5 manettes enregistrées mais seulement 4 de connectées, il prendra les 4 premières qui seront connectées
            }
        }
    }

    private Vector3 GetComponentRandomSpawnPoint()
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Collider[] c = Physics.OverlapSphere(spawnPoints[i].position, 5f, LayerMask.NameToLayer("Player"));
            if (c == null)  //Si on ne détecte aucun joueur dans la zone environnante, ce point de spawn est sûr et on peut y spawner le joueur
            {
                spawnPositions.Add(spawnPoints[i].position);
            }
        }

        // Vu qu'on a 4 joueurs et 6 points de spawn, ce n'est pas la peine de faire une vérification sur la taille de la liste pour voir si elle est nulle ou pas.
        // Mais juste pour la propreté du code (si jamais on a moins de pts de spawn), on met la boucle if quand même
        if (spawnPositions.Count == 0)
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;
        else
            return spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
    }



    public void RespawnPlayerOnDeath(int joueurID)
    {
        StartCoroutine(RespawnPlayer(joueurID));    //IMPORTANT : Ne pas appeler les Coroutines sur les objets qui seront désactivés par la Coroutine en question, sinon ça bloquera toute la fonction
    }

    public IEnumerator RespawnPlayer(int joueurID)
    {
        joueurs[joueurID - 1].gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        joueurs[joueurID - 1].gameObject.SetActive(true);
        joueurs[joueurID - 1].isDead = false;


        joueurs[joueurID - 1].p.t.position = GetComponentRandomSpawnPoint();
        joueurs[joueurID - 1].p.meshToRotate.rotation = Quaternion.identity;
        joueurs[joueurID - 1].p.ChangerArmeEtVaisseau(joueurs[joueurID - 1].p.typeDuVaisseau);

        joueurs[joueurID - 1].curHealth = joueurs[joueurID - 1].maxHealth;
        joueurs[joueurID - 1].playerUI.UpdateHealthUI();


        ScoreManager.instance.InitializePlayerUI(joueurID);
        //On remet à jour tous les UIs au moment de la mort et du respawn du joueur

    }





    public void StopAllPlayersMovement()
    {
        for (int i = 0; i < joueurs.Count; i++)
        {
            joueurs[i].partieTerminée = true;
        }

        IAStats[] ias = FindObjectsOfType<IAStats>();

        for (int i = 0; i < ias.Length; i++)
        {
            ias[i].partieTerminée = true;
        }
    }
}
