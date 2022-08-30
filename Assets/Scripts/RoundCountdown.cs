using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundCountdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundCountdownText;

    [SerializeField] private int roundTime;

    private float roundTimeLeft;

    public void Start()
    {
        roundTimeLeft = (float)roundTime;
    }

    public void Update()
    {
        if (GetComponent<GameController>().IsPause() || GetComponent<GameController>().IsGameOver())
        {
            return;
        }

        float currentTime = 0.0f;

        if (roundTimeLeft > 0)
        {
            roundTimeLeft -= Time.deltaTime;

            currentTime = roundTimeLeft > 0 ? roundTimeLeft : 0.0f;

            if (currentTime == 0.0f)
            {
                gameObject.GetComponent<GameController>().SetGameOver();
            }
        }

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        if (currentTime <= 30f)
        {
            roundCountdownText.color = Color.red;
        }
        else if (currentTime > (float)roundTime * 0.6f)
        {
            roundCountdownText.color = new Color(47f / 255f, 154f / 255f, 8f / 255f, 1f);
        }
        else if (currentTime <= (float)roundTime * 0.6f)
        {
            roundCountdownText.color = new Color(217f / 255f, 152f / 255f, 0f, 1f);
        }

        roundCountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public float GetRemainTime()
    {
        return (float)roundTime - roundTimeLeft;
    }
}