using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBackground : MonoBehaviour
{
    private static SoundBackground instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Kiểm tra xem có đối tượng nào đã tồn tại chưa
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không bị hủy khi load scene mới
        }
        else
        {
            Destroy(gameObject); // Nếu đã có, hủy object mới để tránh trùng lặp nhạc
            return;
        }

        // Lấy AudioSource và phát nhạc
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
