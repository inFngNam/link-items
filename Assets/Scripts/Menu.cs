using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    [SerializeField] public Button openLevelSelectPanelButton;
    [SerializeField] public Button startButton;
    [SerializeField] public GameObject levelSelectPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startGameSound;

    private void Start()
    {
        openLevelSelectPanelButton.onClick.AddListener(LevelSelect);
        startButton.onClick.AddListener(PlayGame);
    }

    private void LevelSelect()
    {
        levelSelectPanel.GetComponent<LevelSelectPanel>().Show();
    }

    private void PlayGame()
    {
        PlaySound();
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("level", 1);
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(startGameSound);
    }
}
