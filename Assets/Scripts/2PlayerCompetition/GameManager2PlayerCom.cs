using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager2PlayerCom : MonoBehaviour
{
    public List<DataPlayer> dataPlayers;

    public List<Button> listButtonChoose;

    public TextMeshProUGUI tmp;

    public Controller2Player player1;

    public Controller2Player player2;

    public MapSpawner mapSpawner;

    public GameObject uiChoosePlayer;

    
    public static GameManager2PlayerCom Instance { get; private set; }

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
        }
        else if (countChoose == 2)
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

        player1.isGameStarted = true;
        player2.isGameStarted = true;
        mapSpawner.IsUpdateChunk = true;
    }
}


[Serializable]
public class ChoosePlayer
{
    public RuntimeAnimatorController animator;
    public Sprite sprite;
}
