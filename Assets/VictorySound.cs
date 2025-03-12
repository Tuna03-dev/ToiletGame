using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySound : MonoBehaviour
{
    private AudioSource audioSource; // AudioSource để phát âm thanh chiến thắng
    public AudioClip victoryClip; // Âm thanh chiến thắng

    private CommonPlayer commonPlayer; // Tham chiếu đến CommonPlayer

    void Start()
    {
        // Lấy AudioSource từ object này
        audioSource = GetComponent<AudioSource>();

        // Kiểm tra nếu có AudioSource và victoryClip được gán
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found!");
        }

        if (victoryClip == null)
        {
            Debug.LogError("Victory sound clip not assigned!");
        }

        // Lấy tham chiếu đến CommonPlayer
        commonPlayer = FindObjectOfType<CommonPlayer>();
        if (commonPlayer == null)
        {
            Debug.LogError("CommonPlayer not found!");
        }
    }

    void Update()
    {
        // Kiểm tra nếu người chơi đã thắng
        if (commonPlayer != null && commonPlayer.CheckVictory())
        {
            PlayVictorySound(); 
        }
    }

    // Phương thức để phát âm thanh chiến thắng
    public void PlayVictorySound()
    {
        if (audioSource != null && victoryClip != null)
        {
            audioSource.PlayOneShot(victoryClip); // Phát âm thanh chiến thắng
        }
    }
}
