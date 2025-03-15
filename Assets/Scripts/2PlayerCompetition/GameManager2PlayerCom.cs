using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager2PlayerCom : MonoBehaviour
{
    public List<DataPlayer> dataPlayers;

    public List<Button> listButtonChoose;

    public TextMeshProUGUI tmp;
    public Text countdownText;
    public Controller2Player player1;

    public Controller2Player player2;

    public MapSpawner mapSpawner;

    public GameObject uiChoosePlayer;

    private Sprite player1Sprite;
    private RuntimeAnimatorController player1Animator;
    private Sprite player2Sprite;
    private RuntimeAnimatorController player2Animator;

    private int player1Wins = 0;
    private int player2Wins = 0;
    [HideInInspector]
    public int currentRound = 1;
    public int totalRounds = 5; // Mặc định là 5 round

    public Text winnerText;
    public Text resultText;
    public GameObject endScreen; // Panel hiển thị kết quả cuối cùng
    public GameObject menuScreen;

    public static GameManager2PlayerCom Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        menuScreen.SetActive(true);


        listButtonChoose.ForEach(x =>
        {
            x.onClick.AddListener(() => OnChoose(x));
        });
        mapSpawner.IsUpdateChunk = false;

        tmp.text = "Choose Character 1";
    }

    private int countChoose;


    private void OnChoose(Button button)
    {
        countChoose++;
        
        if (countChoose == 1)
        {
            tmp.text = "Choose Character 2";

            int indexCharacter = int.Parse(button.name);


            player1.spriteCharacter.sprite = dataPlayers[indexCharacter].sprite;
            player1.animator.runtimeAnimatorController = dataPlayers[indexCharacter].animator;
            player1Sprite = dataPlayers[indexCharacter].sprite;
            player1Animator = dataPlayers[indexCharacter].animator;
        }
        else if (countChoose == 2)
        {
            int indexCharacter = int.Parse(button.name);

            player2.spriteCharacter.sprite = dataPlayers[indexCharacter].sprite;
            player2.animator.runtimeAnimatorController = dataPlayers[indexCharacter].animator;
            player2Sprite = dataPlayers[indexCharacter].sprite;
            player2Animator = dataPlayers[indexCharacter].animator;
            StartCoroutine(IEChooseCharacterCompleted());
        }

        Destroy(button.transform.parent.gameObject);
    }

    private IEnumerator IEChooseCharacterCompleted()
    {
        yield return new WaitForSeconds(0.1f);
        uiChoosePlayer.SetActive(false);
        countdownText.text = "Round " + currentRound +" GO!";
        countdownText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        countdownText.gameObject.SetActive(false);
        player1.isGameStarted = true;
        player2.isGameStarted = true;
        mapSpawner.IsUpdateChunk = true;
    }

    // Hàm này gọi khi người chơi chọn số round
    public void SetTotalRounds(int rounds)
    {
        totalRounds = rounds;
        menuScreen.SetActive(false);
        uiChoosePlayer.SetActive(true);
    }

    public void PlayerWin(int playerNumber)
    {
        if (playerNumber == 1)
            player1Wins++;
        else if (playerNumber == 2)
            player2Wins++;

        currentRound++;

        if (currentRound > totalRounds)
        {
            EndGame();
        }
        else
        {
            RestartRound();
        }
    }

    void RestartRound()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    

    // Khi Scene mới được load xong, khôi phục lại nhân vật
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        mapSpawner = FindObjectOfType<MapSpawner>();
        uiChoosePlayer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "UIChoosePlayer");
        menuScreen = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "CanvasRound");
        endScreen = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "Final");

        countdownText = GameObject.Find("CanvaCountDown").GetComponent<Canvas>().transform.Find("Countdown").GetComponent<Text>();
        winnerText = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "Final").transform.Find("PlayerWin").GetComponent<Text>();
        resultText = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "Final").transform.Find("Result").GetComponent<Text>();
        tmp = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "UIChoosePlayer").transform.Find("ChoosePlayer").GetComponent<TextMeshProUGUI>();
        // Tìm lại player trong Scene mới
        Controller2Player[] players = FindObjectsOfType<Controller2Player>();
        foreach (var p in players)
        {
            if (p.isPlayer1)
            {
                player1 = p;
                player1.spriteCharacter.sprite = player1Sprite;
                player1.animator.runtimeAnimatorController = player1Animator;
            }
            else
            {
                player2 = p;
                player2.spriteCharacter.sprite = player2Sprite;
                player2.animator.runtimeAnimatorController = player2Animator;
            }
        }

        StartCoroutine(IEChooseCharacterCompleted());
    }
    void EndGame()
    {
        // Hiển thị màn hình kết quả
        endScreen.SetActive(true);
        resultText.text = $"Player 1-Player 2: {player1Wins}-{player2Wins}";
        if (player1Wins > player2Wins)
            winnerText.text = "Player 1 Win!";
        else if (player2Wins > player1Wins)
            winnerText.text = "Player 2 Win!";  
    }

}



