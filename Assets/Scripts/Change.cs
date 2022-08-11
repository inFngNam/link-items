using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Change : MonoBehaviour
{
    [SerializeField]
    private Button changeButton;
    private int totalChanges;

    public void Start()
    {
        totalChanges = 1;
        changeButton.onClick.AddListener(ChangeBoard);
    }

    private void ChangeBoard()
    {
        GetComponent<Board>().Change();
        GetComponent<GameController>().MinusChangeScore(totalChanges);
        totalChanges += 1;
    }
}
