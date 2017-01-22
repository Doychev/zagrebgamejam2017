using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance
    {
        get;
        private set;
    }

    public Text peopleText, timerText;
    public GameObject gameOverPanel;

    private bool gameStarted = false;

    private float timeLeft = 120.0f;


    private float currentScore = 0;

	void Awake () {
        ScoreManager.Instance = this;
    }

    public void startGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            StartCoroutine(UpdateScore());
            StartCoroutine(TsunamiManager.Instance.LaunchWave());
        }
    }

    void Update()
    {
        if (gameStarted)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = getTimerString(timeLeft);
            if (timeLeft < 0)
            {
                StartCoroutine(EndGame());
                gameStarted = false;
            }
        }
    }

    private string getTimerString(float time)
    {
        string result = "";
        if (time < 0)
        {
            result = "0:00";
        }
        else
        {
            int minutes = (int)time / 60;
            int seconds = (int)time - minutes * 60;
            if (seconds > 9)
            {
                result = minutes + ":" + seconds;
            }
            else
            {
                result = minutes + ":0" + seconds;
            }
        }
        return result;
    }

    public IEnumerator UpdateScore()
    {
        yield return new WaitForSeconds(1.0f);
        int livingPeople = CrowdManager.Instance.CountLivingHumans();
        while (livingPeople > 0)
        {
            livingPeople = CrowdManager.Instance.CountLivingHumans();
            currentScore += livingPeople;
            //scoreText.text = "Score: " + currentScore;
            peopleText.text = "Alive: " + livingPeople;

            if (livingPeople == 0)
            {
                StartCoroutine(EndGame());
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator EndGame()
    {
        int iterations = 10;
        for (int i = 0; i < iterations; i ++)
        {
            float timeScale = Time.timeScale;
            timeScale -= 0.99f / iterations;
            if (timeScale < 0)
            {
                timeScale = 0;
            }
            Time.timeScale = timeScale;
            yield return StartCoroutine(WaitForRealTime(0.75f / iterations));
        }
        Time.timeScale = 0.01f;
        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlaygroundSceneWithSound");
    }

    public static IEnumerator WaitForRealTime(float delay)
    {
        while (true)
        {
            float pauseEndTime = Time.realtimeSinceStartup + delay;
            while (Time.realtimeSinceStartup < pauseEndTime)
            {
                yield return 0;
            }
            break;
        }
    }
}
