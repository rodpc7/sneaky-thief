using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUi : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;  //subscreve ao evento de ser spotted
        RotatingGuard.OnRotGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel += ShowGameWinUI;//subscreve ao evento
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);//faz reload da scene
            }
        }
    }

    void ShowGameWinUI()
    {
            OnUI(gameWinUI);
    }

    void ShowGameLoseUI()
    {
            OnUI(gameLoseUI);
    }

    void OnUI(GameObject UI)
        {
            UI.SetActive(true);//Ativa a Ui
            gameIsOver=true;//Termina o jogo
            Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;//Desincreve do evento
            RotatingGuard.OnRotGuardHasSpottedPlayer -= ShowGameLoseUI;//Desincreve do evento
            FindObjectOfType<Player>().OnReachedEndOfLevel -= ShowGameWinUI;
    }
}
