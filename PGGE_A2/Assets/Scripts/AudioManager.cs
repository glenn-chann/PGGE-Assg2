using PGGE.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    //variable containing the audio source
    [HideInInspector] public AudioSource source;

    //audio clips
    public AudioClip multiplayer;
    public AudioClip singleplayer;
    public AudioClip back;
    public AudioClip join;

    //setting the audio source variable 
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
}

