using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Change : MonoBehaviour
{
    [SerializeField] private Button changeButton;
    private int totalChanges;

    public void Start()
    {
        totalChanges = 1;
        changeButton.onClick.AddListener(ChangeBoard);
    }

    private void ChangeBoard()
    {
        if (GetComponent<GameController>().IsGameOver() || GetComponent<GameController>().IsPause())
        {
            return;
        }

        GetComponent<Board>().Change();
        GetComponent<GameController>().MinusChangeScore(totalChanges);
        GetComponent<GameController>().PlayStartSound();
        totalChanges += 1;
    }
}
