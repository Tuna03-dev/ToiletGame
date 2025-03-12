using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Thêm namespace để sử dụng SceneManager

public class SoundBackground : MonoBehaviour
{
    private static SoundBackground instance;
    private AudioSource audioSource;
    private CommonPlayer commonPlayer;

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

        // Lấy tham chiếu đến CommonPlayer
        commonPlayer = FindObjectOfType<CommonPlayer>();

        // Đăng ký sự kiện khi scene thay đổi
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded; // Đăng ký sự kiện khi scene bị thoát
    }

    // Hủy đăng ký sự kiện khi object bị hủy
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded; // Hủy đăng ký sự kiện scene bị thoát
    }

    // Hàm này sẽ được gọi mỗi khi một scene mới được load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra nếu nhạc đang tắt thì bật lại
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Hàm này sẽ được gọi mỗi khi một scene bị thoát
    private void OnSceneUnloaded(Scene scene)
    {
        // Dừng nhạc khi scene bị thoát
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Kiểm tra nếu người chơi thắng và dừng nhạc
    void Update()
    {
        if (commonPlayer != null && commonPlayer.CheckVictory())
        {
            OnPlayerWin();
        }
    }

    // Gọi hàm này khi người chơi thắng
    private void OnPlayerWin()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Dừng nhạc khi người chơi thắng
        }
    }
}
