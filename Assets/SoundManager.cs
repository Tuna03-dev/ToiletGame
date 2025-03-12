using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volimeSlider;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }

    }

    public void ChangeVolume()
    {

        AudioListener.volume = volimeSlider.value;
        Save();
    }
    public void Load()
    {
        volimeSlider.value = PlayerPrefs.GetFloat("musicVolume", 5f);
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volimeSlider.value);
    }
}