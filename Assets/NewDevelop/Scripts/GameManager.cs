using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<DataPlayer> dataPlayers;

    public List<Button> listButtonChoose;

    public TextMeshProUGUI tmp;

    public Player player1;

    public Player player2;

    public ChunkSpawner chunkSpawner;

    public GameObject uiChoosePlayer;

    public Text countdownText;

    [Header("Hp")]

    public Slider sliderHp;

    public TextMeshProUGUI txtHp;

    public float hpDecreaseRate = 100;

    public const float MAX_HP = 1000;

    private float currentHp;

    [Header("WinLoseGameState")]

    public GameObject winGO;

    public GameObject loseGO;

    [Header("Audio")]

    public AudioSource audioSource;

    public AudioClip audioWin;
    public AudioClip audioLose;
    public AudioClip backgroundMusic;

    public TextMeshProUGUI txtCountReward;
    public GameObject gameOverCanvas;
    public GameObject pauseButton;
    public GameObject pauseGame;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        uiChoosePlayer.SetActive(true);
        PlayBackgroundMusic();

        listButtonChoose.ForEach(x =>
        {
            x.onClick.AddListener(() => OnChoose(x));
        });

        chunkSpawner.IsUpdateChunk = false;

        tmp.text = "Choose Player 1";
        currentHp = MAX_HP;
    }

    private int countChoose;  


    private void OnChoose(Button button)
    {
        countChoose++;

        if(countChoose == 1)
        {
            tmp.text = "Choose Player 2";

            int indexCharacter = int.Parse(button.name);

           
            player1.spriteCharacter.sprite = dataPlayers[indexCharacter].sprite;
            player1.animator.runtimeAnimatorController = dataPlayers[indexCharacter].animator;
        }
        else if(countChoose == 2)
        {
            int indexCharacter = int.Parse(button.name);

            player2.spriteCharacter.sprite = dataPlayers[indexCharacter].sprite;
            player2.animator.runtimeAnimatorController = dataPlayers[indexCharacter].animator;

            StartCoroutine(IEChooseCharacterCompleted());
        }

        Destroy(button.transform.parent.gameObject);
    }

    private IEnumerator IEChooseCharacterCompleted()
    {
        yield return new WaitForSeconds(0.1f);

        uiChoosePlayer.SetActive(false);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.gameObject.SetActive(false);

        player1.IsGameStarted = true;
        player2.IsGameStarted = true;
        chunkSpawner.IsUpdateChunk = true;
    }

    bool isFailed = false;
    public void RestartGame()
    {
        if (isFailed)
            return;

        isFailed = true;

        StartCoroutine(IEReStartGame());
    }

    private IEnumerator IEReStartGame()
    {

        player1.IsGameStarted = false;
        player2.IsGameStarted = false;
        chunkSpawner.IsUpdateChunk = false;

        loseGO.SetActive(true);

        PlayAudio(audioLose);
        

        yield return new WaitForSecondsRealtime(3f);

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            pauseButton.SetActive(false);
            pauseGame.SetActive(false);
        }
    }
    // ham tang mau cho nhan vat
    public void IncreaseHp()
    {
        currentHp += hpDecreaseRate;

        if (currentHp >= MAX_HP)
        {
            currentHp = MAX_HP;
        }

        txtHp.text = currentHp.ToString();

        sliderHp.value = currentHp / MAX_HP;
    }

    public void TriggerOstacle()
    {
        currentHp -= hpDecreaseRate;

        if(currentHp <= 0)
        {
            currentHp = 0;
            RestartGame();
        }

        txtHp.text = currentHp.ToString();

        sliderHp.value = currentHp / MAX_HP;
    }

    private int itemRewardCount;

    public int WIN_REWARD_COUNT = 10;

    public void RewardItem()
    {
        itemRewardCount++;

        txtCountReward.text = itemRewardCount.ToString() + "/" + WIN_REWARD_COUNT.ToString();

        if (itemRewardCount >= WIN_REWARD_COUNT)
        {
            WinGameState();
        }
    }

    private bool isWinState = false;
    public void WinGameState()
    {
        if (isWinState)
        {
            isWinState = true;
            return;
        }

        player1.IsGameStarted = false;
        player2.IsGameStarted = false;
        chunkSpawner.IsUpdateChunk = false;

        winGO.SetActive(true);
        pauseGame.SetActive(false);

        GameObject pfx = Resources.Load<GameObject>("Effect/EffectCookieJar");
        Instantiate(pfx).transform.position = Camera.main.transform.position;

        PlayAudio(audioWin);

        player1.Stop();
        player2.Stop();
    }

    public void PlayAudio(AudioClip audio)
    {
        audioSource.PlayOneShot(audio);
        
    }
    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Lặp lại nhạc nền
            audioSource.Play();
        }
    }
}


[Serializable]
public class DataPlayer
{
    public RuntimeAnimatorController animator;
    public Sprite sprite;
}
