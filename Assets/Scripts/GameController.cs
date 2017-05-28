using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public bool IsSecondChance { get; set; }

    private bool isPaused;
    public bool IsPaused
    {
        get { return isPaused; }
        private set
        {
            isPaused = value;
            if (value)
                UIController.instance.ShowPauseUI();
            else
                UIController.instance.ShowHUDUI();
        }
    }

    private bool isFailed;
    public bool IsFailed
    {
        get { return isFailed; }
        set
        {
            isFailed = value;
            if (value)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerState = PlayerState.Dead;
                isPaused = true;
                Time.timeScale = 0;
                UIController.instance.UpdateFailedText();
                UIController.instance.ShowFailedUI();

	            bool newHighscore = false;
	            int highscore;

	            if (PlayerPrefs.HasKey("Highscore"))
	            {
		            highscore = PlayerPrefs.GetInt("Highscore");

		            if (highscore < Score)
		            {
			            highscore = Score;
			            newHighscore = true;
		            }
	            }
	            else
	            {
					highscore = Score;
		            newHighscore = true;
				}

	            if (newHighscore)
	            {
		            PlayerPrefs.SetInt("Highscore", highscore);
					PlayerPrefs.Save();
	            }

	            UIController.instance.UpdateHighscoreText(newHighscore, highscore);
			}
            else
                UIController.instance.ShowHUDUI();
        }
    }

    private float boostValue;
    public float BoostValue
    {
        get { return boostValue + 1; }
        set
        {
            boostValue = Mathf.Clamp01(value - 1);
            UIController.instance.UpdateBoostBar();
        }
    }

    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (player.PlayerState == PlayerState.Ghost && score <= 0)
            {
                IsFailed = true;
                score = 0;
            }
            else
                score = value;

            UIController.instance.UpdateScoreText();
        }
    }

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void RageQuit()
    {
#if UNITY_EDITOR
        Debug.Log("Quitted");
#endif
        Application.Quit();
    }

    public void TogglePausing()
    {
        IsPaused = (isPaused) ? false : true;

        if (isPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
