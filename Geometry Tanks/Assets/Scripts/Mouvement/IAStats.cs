using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAStats : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]
    
    public string particleTag; //Le tag de la particule qui devra être libérée au moment de la mort du joueur
    [HideInInspector] public IAMovement m;
    Transform t;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]
    
    [HideInInspector] public bool partieTerminée;   //Appelée quand le match est fini pour que les joueurs ne bougent plus

    public int curHealth, maxHealth = 3;

    public Vector2Int nbParticulesARelâcherMinEtMax = new Vector2Int(1, 3);   //Le nombre de particules à relâcher. Sera aléatoire entre ces 2 nombres
    int nbParticulesARelâcher;

    // Start is called before the first frame update
    void Start()
    {
        GetScripts();
    }


    private void Reset()
    {
        curHealth = maxHealth;
    }





    private void GetScripts()
    {
        m = GetComponent<IAMovement>();
        t = transform;

        curHealth = maxHealth;
        nbParticulesARelâcher = Random.Range(nbParticulesARelâcherMinEtMax.x, nbParticulesARelâcherMinEtMax.y+1);

    }








    //Mettre 0 par défaut pour les IAs (vu qu'elles n'ont pas d'ID)
    public void OnHit(int pts, Enums.TypeArme typeDeProjectile)
    {

        //Si le projectile et l'IA sont de la même couleur, alors on n'applique aucun dégât, et le projectile passe à travers le joueur
        if (typeDeProjectile == m.typeDeCetteIA)
            return;

        curHealth -= pts;
        

        if (curHealth < 0)
        {
            curHealth = 0;
            Die();
        }
    }

    private void Die()
    {

        LooseAllExp(); //Videra tout l'exp d joueur et libérera des particules d'exp dans l'arène
        gameObject.SetActive(false);
    }







    private void LooseAllExp()
    {
        for (int i = 0; i < nbParticulesARelâcher; i++)
        {
            ObjectPooler.instance.SpawnFromPool(particleTag, t.position, Quaternion.identity);
        }
    }




    


    private void OnEnable()
    {
        curHealth = maxHealth;
    }

}
