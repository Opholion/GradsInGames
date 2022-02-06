using UnityEngine;
using TMPro;
using System.Collections;

public class UI : MonoBehaviour
{
    [SerializeField] private CanvasGroup Menu;
    [SerializeField] private CanvasGroup Game;
    [SerializeField] private CanvasGroup Result;
    [SerializeField] private CanvasGroup Help;

    [SerializeField] private CanvasGroup AttackMenu;

    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text CollectibleText;

    [SerializeField] private TMP_Text ResultText;


    private static readonly string[] ResultTexts = { "Game Over!", "You Win!!" };
    private static readonly float AnimationTime = 0.5f;
    bool IsHelpShowing = false;

    public void ShowMenu()
    {
        StartCoroutine(ShowCanvas(Menu, 1.0f, true));
    }

    public void ShowGame()
    {
        //Why would this block raycasts? From what I can see, it's only a timer. 
        StartCoroutine(ShowCanvas(Game, 1.0f, false));
    }

    public void ShowResult(bool success)
    {
        if (ResultText != null)
        {
            ResultText.text = ResultTexts[success ? 1 : 0];
        }

        StartCoroutine(ShowCanvas(Result, 1.0f, true));
    }

    public void HideMenu()
    {
        StartCoroutine(ShowCanvas(Menu, 0.0f, false));
        HideHelp();
    }

    public void HideGame()
    {
        StartCoroutine(ShowCanvas(Game, 0.0f, false));
    }

    public void HideResult()
    {
        StartCoroutine(ShowCanvas(Result, 0.0f, false));
    }



    //Custom UI
    public void ShowAbilities()
    {
        StartCoroutine(ShowCanvas(AttackMenu, 1.0f, true));
    }
    public void HideAbilities()
    {
        StartCoroutine(ShowCanvas(AttackMenu, 0.0f, false));
    }


    public void ToggleHelp ()
    {
        StartCoroutine(ShowCanvas(Help, !IsHelpShowing ? 1.0f : 0.0f, !IsHelpShowing ? true : false));

        IsHelpShowing = !IsHelpShowing;
    }
    public void HideHelp()
    {
        if (IsHelpShowing)
        {
            StartCoroutine(ShowCanvas(Help, 0.0f, false));
            IsHelpShowing = !IsHelpShowing;
        }
    }
    //End custom UI

    public void UpdateTimer(double gameTime)
    {
        if (TimerText != null)
        {
            TimerText.text = FormatTime(gameTime);
        }
    }
    public void ShowCollectibles(int current, int max)
    {
        if (CollectibleText != null)
        {
            CollectibleText.text = "Target: " + current + "/" + max;
        }
    }

    private void Awake()
    {
        if (Menu != null)
        {
            Menu.alpha = 0.0f;
            Menu.interactable = false;
            Menu.blocksRaycasts = false;
        }

        if (Game != null)
        {
            Game.alpha = 0.0f;
            Game.interactable = false;
            Game.blocksRaycasts = false;
        }

        if (Result != null)
        {
            Result.alpha = 0.0f;
            Result.interactable = false;
            Result.blocksRaycasts = false;
        }
    }

    private static string FormatTime(double seconds)
    {
        float m = Mathf.Floor((int)seconds / 60);
        float s = (float)seconds - (m * 60);
        string mStr = m.ToString("00");
        string sStr = s.ToString("00.000");
        return string.Format("{0}:{1}", mStr, sStr);
    }

    private IEnumerator ShowCanvas(CanvasGroup group, float target, bool isBlockRaycast)
    {
        if (group != null)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = target >= 1.0f;
            group.blocksRaycasts = isBlockRaycast;

            while (t < AnimationTime)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0.0f, AnimationTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / AnimationTime);
                yield return null;
            }
        }
    }
}
