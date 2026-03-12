using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverText;
    public GameObject gameClearText;
    public Text scoreText;

    //SEの設定
    public AudioClip gameClearSE;
    public AudioClip gameOverSE;
    AudioSource audioSource;

    const int MAX_SCORE = 9999;
    int score = 0;


    private void Start()
    {
        scoreText.text = score.ToString();
        audioSource = GetComponent<AudioSource>();
    }
    public void AddScore(int val)
    {
        score += val;
        if (score >= MAX_SCORE)
        {
            score = MAX_SCORE;
        }
        scoreText.text = score.ToString();
    }


    public void GameOver()
    {
        gameOverText.SetActive(true);
        audioSource.PlayOneShot(gameOverSE);
        //Invoke;指定した時間に指定の関数を実行する
        //1.5秒後にRestartScene関数を実行する
        Invoke("RestartScene", 1.5f);
    }
    public void GameClear()
    {
        gameClearText.SetActive(true);
        //PlayOneShot:一度だけ鳴らす
        audioSource.PlayOneShot(gameClearSE);
        //Invoke;指定した時間に指定の関数を実行する
        //1.5秒後にRestartScene関数を実行する
        Invoke("NextScene", 1.5f);
    }

    public void RestartScene()
    {
        Scene thisscene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisscene.name);
    }

    public void NextScene()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.buildIndex + 1);
    }
}
