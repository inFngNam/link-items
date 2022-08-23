using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] public Button closePanelButton;
    [SerializeField] public GameObject panel;
    [SerializeField] public Button levelOneSelectButton;
    [SerializeField] public Button levelTwoSelectButton;
    [SerializeField] public Button levelThreeSelectButton;

    private void Start()
    {
        closePanelButton.onClick.AddListener(Hide);

        levelOneSelectButton.onClick.AddListener(SelectLevelOne);
        levelTwoSelectButton.onClick.AddListener(SelectLevelTwo);
        levelThreeSelectButton.onClick.AddListener(SelectLevelThree);
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    private void Hide()
    {
        panel.SetActive(false);
    }

    private void SelectLevelOne()
    {
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("level", 1);
    }

    private void SelectLevelTwo()
    {
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("level", 2);
    }

    private void SelectLevelThree()
    {
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("level", 3);
    }
}
