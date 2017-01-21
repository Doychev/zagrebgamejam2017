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

    public Text scoreText, peopleText;
    public GameObject gameOverPanel;

    private int currentScore = 0;

	void Awake () {
        ScoreManager.Instance = this;
    }
	
    public void UpdateScore()
    {
        int livingPeople = CrowdManager.Instance.CountLivingHumans();
        currentScore += livingPeople;
        scoreText.text = "Score: " + currentScore;
        peopleText.text = "Alive: " + livingPeople;

        if (livingPeople > 0)
        {
            TsunamiManager.Instance.IncreaseDifficulty();
        } else
        {
            StartCoroutine(EndGame());
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
        SceneManager.LoadScene("PlaygroundScene");
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
