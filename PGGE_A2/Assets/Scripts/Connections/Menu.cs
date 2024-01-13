using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickSinglePlayer()
    {
        audioManager.source.PlayOneShot(audioManager.singleplayer);
        //Debug.Log("Loading singleplayer game");
        SceneManager.LoadScene("SinglePlayer");
    }

    public void OnClickMultiPlayer()
    {
        audioManager.source.PlayOneShot(audioManager.multiplayer);
        //Debug.Log("Loading multiplayer game");
        SceneManager.LoadScene("Multiplayer_Launcher");
    }

}
