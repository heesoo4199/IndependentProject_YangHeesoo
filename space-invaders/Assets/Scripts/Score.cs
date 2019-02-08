using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Score : MonoBehaviour {

    public Text scoreText;
    public Text highScoreText;
    public Text livesLeft;

    public int scoreCount;
    public int highScoreCount;
    public int lives;
    // Use this for initialization
    void Start ()
    {
        lives = 3;
        scoreCount = 0;
        highScoreCount = PlayerPrefs.GetInt("highscore", 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        scoreText.text = "Score : " + scoreCount;
        if (scoreText.text.Equals("Score : 175"))
        {
            Application.LoadLevel(3);
        }
        StoreHighscore(scoreCount);
        highScoreText.text = "High Score : " + highScoreCount;
        if (lives != 0)
        livesLeft.text = "Lives : " + lives;
        else
        {
            //livesLeft.text = "GAME OVER BITCH";
            Application.LoadLevel(2);
        }
    }

    void StoreHighscore(int newHighscore)
    {
        int oldHighscore = PlayerPrefs.GetInt("highscore", 0);
        if (newHighscore > oldHighscore)
        {
            PlayerPrefs.SetInt("highscore", newHighscore);
            highScoreCount = newHighscore;
        }
    }

}
