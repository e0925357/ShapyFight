using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private bool isFailed;
    public bool IsFailed
    {
        get { return isFailed; }
        set
        {
            isFailed = value;
            if (value)
                UIController.instance.ShowFailedUI();
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
            if (boostValue >= 1)
                return;

            boostValue = value - 1;
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
    }
}
