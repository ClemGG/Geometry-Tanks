using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arme : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public string bulletTag; //Le tag de du projectile à spawner
    public Transform pointDeTir;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]

    public Enums.TypeArme typeArme;
    public bool isEvolved = false, peutTirer = true;

    public int curExp, maxExp = 10;

    public float cadenceDeTir;
    [HideInInspector] public float timer;






    // Appelé dans l'Awake pour être sûr que les UIs soient bien mis à jour
    protected void Awake()
    {
        timer = 0f;
        peutTirer = true;
    }


    protected void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }

        peutTirer = timer <= 0f;
    }


    public void Tirer()
    {

        PlayTirerSound();
        


        timer = cadenceDeTir;
        peutTirer = false;

        Projectile p = ObjectPooler.instance.SpawnFromPool(bulletTag, pointDeTir.position, pointDeTir.rotation).GetComponent<Projectile>();


        Transform parent = transform.parent;
        PlayerMovement pm = parent.GetComponent<PlayerMovement>();
        IAMovement im = parent.GetComponent<IAMovement>();

        if (pm)
        {
            p.projectileID = pm.joueurID;
            p.rb.velocity = pm.rb.velocity;
        }
        else if(im)
        {
            p.projectileID = 0;
            p.rb.velocity = im.rb.velocity;
        }


        //Si c'est l'onde de choc rouge, on l'attache au joueur pour qu'elle le suive, puis on la détache quand elle est finie
        if (p.typeDeProjectile == Enums.TypeArme.Rouge)
        {
            p.transform.parent = parent;
        }

    }

    private void PlayTirerSound()
    {
        switch (typeArme)
        {
            case Enums.TypeArme.Bleu:
                AudioManager.instance.Play(isEvolved ? "BlueShot" : "sonTir");
                break;
            case Enums.TypeArme.Rouge:
                AudioManager.instance.Play(isEvolved ? "RedShot V2" : "RedShot");
                break;
            case Enums.TypeArme.Jaune:
                AudioManager.instance.Play("YellowShot");
                break;
            case Enums.TypeArme.Vert:
                AudioManager.instance.Play("GreenShot");
                break;
        }
    }





    public void AddExp(int pts)
    {
        curExp += pts;
        curExp = Mathf.Clamp(curExp, 0, maxExp);
        
        PlayerMovement parent = transform.parent.GetComponent<PlayerMovement>();

        if (curExp == maxExp)
        {
            AudioManager.instance.Play("changementVaisseau");
            isEvolved = true;
            parent.OnWeaponEvolved(this);

        }
        ScoreManager.instance.UpdateExpUI(parent.joueurID, (int)typeArme);
    }



    //Quand l'arme est désactivée, le joueur est mort. On la remet donc à jour pour pouvoir la réutiliser après le respawn
    private void OnDisable()
    {
        timer = 0f;
        peutTirer = true;
    }
}
