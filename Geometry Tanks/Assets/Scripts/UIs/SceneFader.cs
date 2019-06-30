using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour {
    
    public Image fadeImg;

    [Tooltip("Plus la valeur est haute, plus le fondu sera rapide, et inversement plus la valeur est basse.")]
    public float fadeAlphaSpeed = 1f;
    [Tooltip("Plus la valeur est haute, plus le fondu sera rapide, et inversement plus la valeur est basse.")]
    public float fadeColorSpeed = 1f;
    public float waitDuration = 1.5f;

    public Color fadeColorToWhite = Color.white;
    public Color fadeColorToBlack = Color.black;
    public AnimationCurve fadeCurve;


    public static SceneFader instance;




    private void Awake()
    {
        if (instance != null)
        {
            print("More than one SceneFader in scene !");
            return;
        }

        instance = this;
        
    }





    private void Start()
    {
        if (fadeImg.gameObject.activeSelf == false)
        {
            fadeImg.gameObject.SetActive(true);
        }

        StartCoroutine(FadeIn());
    }









    /// <summary>
    /// Permet de réaliser un fondu entre les scènes
    /// </summary>
    public void FadeToScene(int sceneIndex)
    {
        if(AudioManager.instance)
            AudioManager.instance.Play("TimesupOuverture");

        StartCoroutine(FadeOut(sceneIndex));
    }



    /// <summary>
    /// Permet de réaliser un fondu avant de quitter le jeu.
    /// </summary>
    public void FadeToQuitScene()
    {
        if(AudioManager.instance)
            AudioManager.instance.Play("TimesupOuverture");

        StartCoroutine(FadeQuit());
    }

























    /// <summary>
    /// Diminue l'alpha du fondu pour faire apparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeIn()
    {
        Time.timeScale = 0f;
        float t = 0f;
        fadeImg.color = fadeColorToBlack;


        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeColorSpeed;
            fadeImg.color = Color.Lerp(fadeColorToBlack, fadeColorToWhite, t);
            yield return 0;
        }


        yield return new WaitForSecondsRealtime(waitDuration);
        Time.timeScale = 1f;
        t = 1f;


        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        fadeImg.gameObject.SetActive(false);

        

    }








    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOut(int sceneIndex)
    {
        fadeImg.gameObject.SetActive(true);
        Time.timeScale = 1f;
        float t = 0f;



        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(waitDuration);
        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeColorSpeed;
            fadeImg.color = Color.Lerp(fadeColorToWhite, fadeColorToBlack, t);
            yield return 0;
        }


        SceneManager.LoadScene(sceneIndex);
    }







    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène et quitter le jeu.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeQuit()
    {
        fadeImg.gameObject.SetActive(true);
        float t = 0f;



        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        //yield return new WaitForSecondsRealtime(waitDuration);
        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeColorSpeed;
            fadeImg.color = Color.Lerp(fadeColorToWhite, fadeColorToBlack, t);
            yield return 0;
        }


        Application.Quit();
    }
}
