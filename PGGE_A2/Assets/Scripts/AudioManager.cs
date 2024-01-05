using PGGE.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [HideInInspector] public AudioSource source;

    public AudioClip multiplayer;
    public AudioClip singleplayer;
    public AudioClip back;
    public AudioClip join;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
}

