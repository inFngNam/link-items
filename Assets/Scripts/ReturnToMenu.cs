using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] public Button returnToMenuButton;

    public void Start()
    {
        returnToMenuButton.onClick.AddListener(GoToMenu);
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
