using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    private bool isGameOver;

    private bool isWin;

    [SerializeField]
    private TextMeshProUGUI resultText;

    public void Start()
    {
        isWin = false;
        isGameOver = false;
    }

    public void SetGameOver()
    {
        isGameOver = true;

        if (!isWin)
        {
            resultText.text = "Noob";
        }
        else
        {
            resultText.text = "Pro !!!";
        }
    }
}
