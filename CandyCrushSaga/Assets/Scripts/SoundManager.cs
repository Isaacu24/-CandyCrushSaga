using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    AudioSource background;
    AudioSource effectSound;

    [SerializeField]
    private AudioClip[] clips = new AudioClip[10];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(gameObject.transform.GetChild(0));
        }

        else
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
    }

    private void Start()
    {
        background = GetComponent<AudioSource>();
        effectSound = transform.GetChild(0).GetComponent<AudioSource>();

        PlayTitleSound();
    }

    void Update()
    {
        
    }

    public void PlayTitleSound()
    {
        background.clip = clips[0];
        background.Play();
    }

    public void CilckButton()
    {
        effectSound.clip = clips[9];
        effectSound.Play();

        background.Stop();
    }

    public void DestroyCandy()
    {
        effectSound.Stop();
        effectSound.clip = clips[2];
        effectSound.Play();
    }

    public void FallCandy()
    {
        effectSound.Stop();
        effectSound.clip = clips[3];
        effectSound.Play();
    }

    public void LandCandy()
    {
        effectSound.Stop();
        effectSound.clip = clips[4];
        effectSound.Play();
    }
}
