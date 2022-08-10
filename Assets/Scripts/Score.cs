using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int score;

    public void Start()
    {
        score = 0;
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }

    public void AddScore(int combo)
    {
        int point = 100;
        score += point + (combo * 10);
    }

    public int GetScore()
    {
        return score;
    }
}