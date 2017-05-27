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
    private RectTransform boostBarFillerRect;
    [SerializeField]
    private Text scoreText;

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

    public void UpdateScoreText()
    {
        sb.Append(string.Format("Score: {0}", GameController.instance.Score));
        scoreText.text = sb.ToString();
        sb.Remove(0, sb.Length);
    }

    public void ShowFailedUI()
    {
        failedScreenCanvasGroup.Enable();
        HUDCanvasGroup.Disable();
    }

    public void ShowHUDUI()
    {
        failedScreenCanvasGroup.Disable();
        HUDCanvasGroup.Enable();
    }
}
