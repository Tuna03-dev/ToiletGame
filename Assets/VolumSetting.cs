using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumSetting : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicslider;
    public void SetVolume()
    {
        float volume = musicslider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
    }
}
