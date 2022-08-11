using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundCountdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roundCountdownText;

    [SerializeField]
    private int roundTime;

    private float roundTimeLeft;

    public void Start()
    {
        roundTimeLeft = (float) roundTime;
    }

    public void Update()
    {
        float currentTime = 0.0f;

        bool isGameOver = gameObject.GetComponent<GameController>().IsGameOver();

        if (roundTimeLeft > 0)
        {
            if (!isGameOver)
            {
                roundTimeLeft -= Time.deltaTime;
            }

            currentTime = roundTimeLeft > 0 ? roundTimeLeft : 0.0f;

            if (currentTime == 0.0f)
            {
                gameObject.GetComponent<GameController>().SetGameOver();
            }
        }

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        if (currentTime <= 30)
        {
            roundCountdownText.color = Color.red;
        }

        roundCountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}