using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] public TextMeshProUGUI resultScoreText;
    [SerializeField] public TextMeshProUGUI resultTimePlayedText;
    [SerializeField] public TextMeshProUGUI resultHighestCombosText;
    [SerializeField] public GameObject stars;
    [SerializeField] public GameObject gameOverImage;
    [SerializeField] public Button playAgainButton;
    [SerializeField] public Button resultMenuButton;

    private void Start()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        resultMenuButton.onClick.AddListener(ReturnToMenu);
    }

    public void Show(bool winStatus, int score, float maxCombo, float playTime)
    {
        resultPanel.SetActive(true);

        stars.SetActive(winStatus);
        gameOverImage.SetActive(!winStatus);

        resultScoreText.text = "Score: " + score;

        resultHighestCombosText.text = "Combo: " + (int)maxCombo;

        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);
        resultTimePlayedText.text = "Time: " + timeStr;
    }

    private void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
