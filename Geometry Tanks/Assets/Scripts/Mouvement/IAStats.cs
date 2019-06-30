using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAStats : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]
    
    public string[] particleTags; //Le tag de la particule qui devra être libérée au moment de la mort du joueur. Dépend de la couleur du joueur
    [Space(10)]

    public string[] prefabsToSpawnOnDeath; //Le tag de la particule qui devra être libérée au moment de la mort du joueur. Dépend de la couleur du joueur

    [Space(10)]

    [HideInInspector] public IAMovement m;
    Transform t;

    [Space(10)]
    [Header("Général : ")]
    [Space(10)]
    
    public Vector2Int nbParticulesARelâcherMinEtMax = new Vector2Int(1, 3);   //Le nombre de particules à relâcher. Sera aléatoire entre ces 2 nombres
    int nbParticulesARelâcher;
    public int curHealth, maxHealth = 3;
    [HideInInspector] public bool partieTerminée;   //Appelée quand le match est fini pour que les joueurs ne bougent plus


    int timerAcide;
    Coroutine acideCoroutine;

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
            AudioManager.instance.Play("death");
            return;
        }

        AudioManager.instance.Play("hit");

    }


    //Mettre 0 par défaut pour les IAs (vu qu'elles n'ont pas d'ID)
    public void OnHit(int pts, Enums.TypeArme typeDeProjectile, bool isEvolved)
    {

        //Si le projectile et l'IA sont de la même couleur, alors on n'applique aucun dégât, et le projectile passe à travers le joueur
        if (typeDeProjectile == m.typeDeCetteIA)
            return;

        curHealth -= pts;



        if (curHealth < 0)
        {

            if (acideCoroutine != null)
            {
                StopCoroutine(acideCoroutine);
                acideCoroutine = null;
            }

            curHealth = 0;
            Die();
            AudioManager.instance.Play("death");

            return;
        }

        AudioManager.instance.Play("hit");


        if (typeDeProjectile == Enums.TypeArme.Vert)
        {

            if (acideCoroutine == null)
            {
                timerAcide = isEvolved ? 10 : 7;
                acideCoroutine = StartCoroutine(DégâtsAcide(typeDeProjectile, isEvolved));
            }
        }
    }



    private IEnumerator DégâtsAcide(Enums.TypeArme typeDeProjectile, bool isEvolved)
    {
        float timer = 0f;
        for (int i = 0; i < timerAcide; i++)
        {
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f;
            OnHit(1, Enums.TypeArme.Vert, isEvolved);
            yield return null;
        }
        
        acideCoroutine = null;
    }








    private void Die()
    {

        LooseAllExp(); //Videra tout l'exp d joueur et libérera des particules d'exp dans l'arène
        SpawnPrefabsOnDeath();
        gameObject.SetActive(false);
    }


    private void SpawnPrefabsOnDeath()
    {
        ObjectPooler.instance.SpawnFromPool(prefabsToSpawnOnDeath[(int)m.typeDeCetteIA], t.position, Quaternion.identity);
    }

    private void LooseAllExp()
    {
        for (int i = 0; i < nbParticulesARelâcher; i++)
        {
            ObjectPooler.instance.SpawnFromPool(particleTags[(int)m.typeDeCetteIA], t.position, Quaternion.identity);
        }
    }




    


    private void OnEnable()
    {
        curHealth = maxHealth;
    }

}
