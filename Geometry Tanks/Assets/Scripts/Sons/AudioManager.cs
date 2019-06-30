using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Son[] sons;


    public static AudioManager instance;

#if UNITY_EDITOR

    private void OnValidate()
    {
        for (int i = 0; i < sons.Length; i++)
        {
            sons[i].tag = sons[i].clip.name;
        }
    }

#endif




    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sons.Length; i++)
        {
            if(sons[i].source == null)
            {
                sons[i].source = gameObject.AddComponent<AudioSource>();
                sons[i].source.clip = sons[i].clip;
                sons[i].source.volume = sons[i].volume;
                sons[i].source.loop = sons[i].loop;
                sons[i].source.playOnAwake = sons[i].playOnAwake;

                if (sons[i].playOnAwake)
                {
                    Play(sons[i].clip.name);
                }
            }
        }
    }


    public void Play(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
            s.source.Play();
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
    public void Stop(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if(s != null)
            s.source.Stop();
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
}
