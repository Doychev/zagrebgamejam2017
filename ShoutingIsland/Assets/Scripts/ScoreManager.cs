using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance
    {
        get;
        private set;
    }

    public Text scoreText, peopleText;

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

        TsunamiManager.Instance.IncreaseDifficulty();
    }
}
