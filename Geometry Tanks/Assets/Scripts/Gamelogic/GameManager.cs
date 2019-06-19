
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

    private void SpawnJoueurs()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            StatsSystem s = Instantiate(playerPrefab, GetComponentRandomSpawnPoint(), Quaternion.identity).GetComponent<StatsSystem>();
            s.p.joueurID = i + 1;
            joueurs.Add(s);
        }
    }

    private Vector3 GetComponentRandomSpawnPoint()
    {
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;
    }





    public IEnumerator RespawnPlayerOnDeath(int joueurID)
    {
        joueurs[joueurID - 1].gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        joueurs[joueurID - 1].gameObject.SetActive(true);
        joueurs[joueurID - 1].isDead = false;

        joueurs[joueurID - 1].p.t.position = GetComponentRandomSpawnPoint();
        joueurs[joueurID - 1].p.meshToRotate.rotation = Quaternion.identity;
    }





    public void StopAllPlayersMovement()
    {
        for (int i = 0; i < joueurs.Count; i++)
        {
            joueurs[i].partieTerminée = true;
        }
    }
}
