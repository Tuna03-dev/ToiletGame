using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerMainMenu : MonoBehaviour
{
    [Header("--------Audio Sources-------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------Audio Clip------")]
    public AudioClip background;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    public void PlaySound()
    {
        if (musicSource != null && buttonClick != null)
        {
            musicSource.PlayOneShot(buttonClick);
        }
    }
}
