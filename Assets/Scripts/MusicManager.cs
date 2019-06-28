using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
  
    public AudioSource gameMusic;
    bool togglePlay = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) // if it doesnt exist
               instance = this; // set this as existing
            
        //If instance already exists and it's not this:
        else if (instance != this)
            Destroy(gameObject);    
            
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(transform.gameObject);
        gameMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameMusic.isPlaying && togglePlay)
        {
            gameMusic.Play();
        }
    }
    void OnEnable()
    {
        UIManager.OnToggleMusic += UIManager_OnToggleMusic;
    }
    void OnDisable()
    {
        UIManager.OnToggleMusic -= UIManager_OnToggleMusic;
    }
    void UIManager_OnToggleMusic()
    {
        if(gameMusic.isPlaying){
            gameMusic.Stop();
            togglePlay = false;
        }
        else{
            gameMusic.Play();
            togglePlay = false;
        }
    }
}
