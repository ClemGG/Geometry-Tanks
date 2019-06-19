using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public Transform[] panelsJoueurs; // A remplir dans l'inspector 
    public Transform[] scoreFinauxJoueurs; // A remplir dans l'inspector 

    [Space(10)]

    public GameObject matchCanvas;
    public GameObject scoreCanvas;


    [Space(10)]
    [Header("Couleurs : ")]
    [Space(10)]

    public Color normalColor;
    public Color evolvedColor;
    public Color highHealthColor;
    public Color lowHealthColor;
    public Color lowCountdownColor;


    [Space(10)]
    [Header("Countdown Match : ")]
    [Space(10)]

    public float countdown = 180f;
    public float redCountdown = 30f;
    private float countdownTimer;
    public TextMeshProUGUI countdownText;

    [HideInInspector] public bool partieTerminée = false;




    public static ScoreManager instance;
    GameManager gameManager;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;

    }


    private void Start()
    {
        gameManager = GameManager.instance;

        for (int i = 0; i < panelsJoueurs.Length; i++)
        {
            panelsJoueurs[i].GetChild(1).gameObject.SetActive(i <= gameManager.joueurs.Count);
            scoreFinauxJoueurs[i].gameObject.SetActive(i <= gameManager.joueurs.Count);
        }

        countdownTimer = countdown;
        
        //On cache le time's up et le tableau des scores s'ils ne le sont pas déjà
        matchCanvas.transform.GetChild(1).gameObject.SetActive(false);  
        matchCanvas.transform.GetChild(2).gameObject.SetActive(false);


    }





    private void Update()
    {
        MatchCountdown();
    }



    private void MatchCountdown()
    {
        //Tant que le timer n'a pas atteint 0, le match continue
        if (countdownTimer > 0f)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer < 0f)
            {
                countdownTimer = 0f;
            }

            ConvertTime(countdownTimer);

        }
        //Sinon, on arrête le match
        else
        {
            if (!partieTerminée)
                EndGame();
        }
    }








    private void ConvertTime(float timer)
    {
        //Utilisé pour convertir le temps en minutes et secondes avant de l'afficher sur l'UI
        int min = (int)Mathf.Floor(timer / 60);
        int sec = (int)(timer % 60);

        if (sec == 60)
        {
            sec = 0;
            min++;
        }

        string minutes = min.ToString("0");
        string seconds = sec.ToString("00");

        countdownText.text = string.Format("{0}:{1}", minutes, seconds);

        if(timer < redCountdown)
        {
            countdownText.color = lowCountdownColor;
        }
    }




    public void EndGame()
    {
        partieTerminée = true;

        StartCoroutine(CheckWinner());
    }


    private IEnumerator CheckWinner()
    {
        gameManager.StopAllPlayersMovement();


        //On active juste le texte Time's Up! pour indiquer au joueur que la partie est finie
        //Et on désactive le countdown de la partie
        matchCanvas.transform.GetChild(0).gameObject.SetActive(false); //Countdown
        scoreCanvas.SetActive(false);

        matchCanvas.transform.GetChild(1).gameObject.SetActive(true); //Time's up
        yield return new WaitForSeconds(4f);
        matchCanvas.transform.GetChild(1).gameObject.SetActive(false);





        //We check which player has won the match, and if there were ex-aequo winners
        if (GetWinner().Count > 1)
        {
            string str = GetWinner()[0].p.joueurID.ToString();

            for (int i = 1; i < GetWinner().Count - 1; i++)
            {
                str = string.Format("{0}, {1}", str, GetWinner()[i].p.joueurID.ToString());

            }

            str = string.Format("{0} and {1}", str, GetWinner()[GetWinner().Count - 1].p.joueurID.ToString());


            matchCanvas.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Victoire des joueurs " + str + " !";
        }
        else if (GetWinner().Count == 1)
        {
            matchCanvas.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Victoire du joueur " + GetWinner()[0].p.joueurID.ToString() + " !";
        }


        UpdateLeaderboardUI();


        //Puis on active le tableau des scores
        matchCanvas.transform.GetChild(2).gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    private List<StatsSystem> GetWinner()
    {
        StatsSystem winner = gameManager.joueurs[0];
        List<StatsSystem> exAequoWinners = new List<StatsSystem>();

        bool hasExAequo = false;

        for (int i = 0; i < gameManager.joueurs.Count; i++)
        {
            matchCanvas.transform.GetChild(1).GetChild(0).GetChild(i + 1).GetChild(1).GetComponent<TextMeshProUGUI>().text = gameManager.joueurs[i].kills + "/" + gameManager.joueurs[i].deaths;
            matchCanvas.transform.GetChild(1).GetChild(0).GetChild(i + 1).GetChild(2).GetComponent<TextMeshProUGUI>().text = gameManager.joueurs[i].GetKDR().ToString();
        }

        for (int i = 0; i < gameManager.joueurs.Count; i++)
        {


            if (gameManager.joueurs[i] != winner && gameManager.joueurs[i].GetKDR() > winner.GetKDR())
            {
                winner = gameManager.joueurs[i];
                exAequoWinners.Clear();
            }
            else if (gameManager.joueurs[i] != winner && gameManager.joueurs[i].GetKDR() == winner.GetKDR())
            {
                hasExAequo = true;

                if (!exAequoWinners.Contains(winner))
                {
                    exAequoWinners.Add(winner);
                }

                exAequoWinners.Add(gameManager.joueurs[i]);
            }

        }

        if (!hasExAequo)
        {

            exAequoWinners.Clear();
            exAequoWinners.Add(winner);
        }

        return exAequoWinners;
    }



    private void UpdateLeaderboardUI()
    {
        for (int i = 0; i < gameManager.joueurs.Count; i++)
        {
            StatsSystem s = gameManager.joueurs[i];

                                    //ScoreJ1             //fond      //kdr
            TextMeshProUGUI kills = scoreFinauxJoueurs[i].GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI deaths = scoreFinauxJoueurs[i].GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ratio = scoreFinauxJoueurs[i].GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();

            kills.text = s.kills.ToString();
            deaths.text = s.deaths.ToString();
            ratio.text = s.GetKDR().ToString();
        }
    }









    //Appelée dans le Start de chaque StatsSystem en jeu. Initialise les UIs
    public void InitializePlayerUI(int joueurID)
    {
        for (int i = 0; i < 4; i++)
        {
            UpdateExpUI(joueurID, i);



            //Panel                    //content   //Boutons   //Croix             //logo et décompte
            Image armeLogo = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(i).GetChild(2).GetComponent<Image>();
            Image décompteImg = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(i).GetChild(3).GetComponent<Image>();
            TextMeshProUGUI décompte = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
            
            //On désactive juste le compteur et on active le logo.
            décompteImg.fillAmount = 0f;
            décompte.gameObject.SetActive(false);
            armeLogo.gameObject.SetActive(true);
        }


        //On récupère toutes les armes du joueur et on remet leur timer à 0 pour que le joueur puisse les utiliser à nouveau
        //Ca forcera aussi l'arrêt de la coroutine qui gère le cooldown
        Arme[] armesDuJoueur = gameManager.joueurs[joueurID - 1].p.transform.GetComponentsInChildren<Arme>();

        for (int j = 0; j < armesDuJoueur.Length; j++)
        {
            armesDuJoueur[j].timer = 0f;
        }

        UpdateHealthUI(joueurID);
        UpdateKdrUI(joueurID);
    }









    /* Le script pour gagner de l'exp sera sur les particules
     * Quand une particule touche le joueur, elle donne des points à l'arme correspondante si elle n'est pas déjà pleine
     * Comme les armes standards deviendront évoluées et ne pourront plus gagner de points, on peut se contenter de récupérer les 4 premières armes au lieu de les trier
     * Le particleIndex ira donc de 0 à 3
     */
    public void UpdateExpUI(int joueurID, int particleIndex)    
    {

       
            //Panel                    //content   //Boutons
            for (int i = 0; i < panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).childCount; i++)
            {
                //Panel                    //content   //Boutons   //Croix     //contour
                Image contour = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(particleIndex).GetChild(0).GetComponent<Image>();
                Arme armeCorrespondante = gameManager.joueurs[joueurID - 1].p.tousLesMeshsDuJoueur[particleIndex].GetComponent<Arme>();

                contour.fillAmount = armeCorrespondante.curExp / armeCorrespondante.maxExp;
            


                //Panel                    //content   //Boutons   //Croix             //logo
                Image armeLogo = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(particleIndex).GetChild(2).GetComponent<Image>();
                armeLogo.color = armeCorrespondante.isEvolved ? evolvedColor : normalColor;
            }
    }







    //Appelée depuis la fonction Tirer() du script PlayerMovement
    public IEnumerator UpdateCooldownUI(int joueurID, int meshIndex)
    {
        int uiToUpdate = meshIndex;

        if (meshIndex >= 4)
            uiToUpdate -= 4;




        //Panel                    //content   //Boutons   //Croix             //compteurImg
        Image décompteImg = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(uiToUpdate).GetChild(3).GetComponent<Image>();
        Image armeLogo = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(uiToUpdate).GetChild(2).GetComponent<Image>();
        TextMeshProUGUI décompte = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(0).GetChild(uiToUpdate).GetChild(4).GetComponent<TextMeshProUGUI>();
        Arme armeActuelle = gameManager.joueurs[joueurID - 1].p.armeActuelle;


        décompte.gameObject.SetActive(true);
        armeLogo.gameObject.SetActive(false);



        armeActuelle.timer = armeActuelle.cadenceDeTir;
        décompteImg.fillAmount = 1f;

        while (décompteImg.fillAmount > 0f)
        {
            armeActuelle.timer -= Time.deltaTime;
            décompteImg.fillAmount = armeActuelle.timer / armeActuelle.cadenceDeTir;
            décompte.text = Mathf.RoundToInt(armeActuelle.timer).ToString();

            yield return null;
        }

        armeActuelle.timer = 0f;
        décompteImg.fillAmount = 0f;

        décompte.gameObject.SetActive(false);
        armeLogo.gameObject.SetActive(true);
    }









    public void UpdateHealthUI(int joueurID)
    {
                            //Panel                    //content   //barre de vie
        Image barreDeVie = panelsJoueurs[joueurID - 1].GetChild(1).GetChild(2).GetComponent<Image>();

        barreDeVie.fillAmount = gameManager.joueurs[joueurID - 1].curHealth / gameManager.joueurs[joueurID - 1].maxHealth;
        barreDeVie.color = Color.Lerp(lowHealthColor, highHealthColor, barreDeVie.fillAmount);
    }

    public void UpdateKdrUI(int joueurID)
    {
        //Panel                    //content   //KDR       //kills et deaths
        panelsJoueurs[joueurID - 1].GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = gameManager.joueurs[joueurID - 1].kills.ToString();
        panelsJoueurs[joueurID - 1].GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = gameManager.joueurs[joueurID - 1].deaths.ToString();
    }
}
