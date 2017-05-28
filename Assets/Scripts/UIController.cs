using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CanvasGroupExtension;
using UnityEngine.UI;
using System.Text;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    private CanvasGroup failedScreenCanvasGroup;
    [SerializeField]
    private CanvasGroup HUDCanvasGroup;
    [SerializeField]
    private CanvasGroup pauseUICanvasGroup;
    [SerializeField]
    private RectTransform boostBarFillerRect;
    [SerializeField]
    private Text scoreText;
    //[SerializeField]
    //private Animator scoreAnim;
    [SerializeField]
    private Text failedScreenText;

    public Transform scoreAdditionGOTran;

    private StringBuilder sb;

    private void Awake()
    {
        instance = this;
        sb = new StringBuilder(13);
    }

    private void Start()
    {
        UpdateBoostBar();
    }

    public void UpdateBoostBar()
    {
        boostBarFillerRect.localScale = new Vector3(GameController.instance.BoostValue - 1, 1, 1);
    }

    public void UpdateFailedText()
    {
        if (GameController.instance.Score < 100)
            failedScreenText.text = string.Format("You are not skilled enough!\nYour score is just {0}!", GameController.instance.Score);
        else if (GameController.instance.Score < 300)
            failedScreenText.text = string.Format("It is a little bit better but you are still not skilled enough!\nYour score is just {0}!", GameController.instance.Score);
        else
            failedScreenText.text = string.Format("You are quite skilled bro!!\nYour score is {0}!", GameController.instance.Score);

    }

    public void UpdateScoreText()
    {
        sb.Append(string.Format("Score: {0}", GameController.instance.Score));
        scoreText.text = sb.ToString();
        sb.Remove(0, sb.Length);
    }

    //public void ScaleText()
    //{
    //    if (scoreAnim.GetCurrentAnimatorStateInfo(0).IsName("ScoreTextNormal"))
    //        scoreAnim.SetTrigger("Scale");
    //    else
    //    {
    //        scoreAnim.SetTrigger("Repeat");
    //        scoreAnim.SetTrigger("Scale");

    //    }
    //}

    public void ShowFailedUI()
    {
        failedScreenCanvasGroup.Enable();
        HUDCanvasGroup.Disable();
        pauseUICanvasGroup.Disable();
    }

    public void ShowHUDUI()
    {
        failedScreenCanvasGroup.Disable();
        HUDCanvasGroup.Enable();
        pauseUICanvasGroup.Disable();
    }

    public void ShowPauseUI()
    {
        pauseUICanvasGroup.Enable();
        HUDCanvasGroup.Disable();
        failedScreenCanvasGroup.Disable();
    }
}
