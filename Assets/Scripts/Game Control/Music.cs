 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static Music instance;
    private AudioSource _auidioSource;
    private void Awake()
    {

        
        _auidioSource = GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayMusic()
    {
        if(_auidioSource.isPlaying) return;
        _auidioSource.Play();
    }
    public void StopMusci()
    {
        _auidioSource.Stop();
    }

    }
