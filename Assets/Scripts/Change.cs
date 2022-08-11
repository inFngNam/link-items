using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Change : MonoBehaviour
{
    [SerializeField]
    private Button changeButton;

    public void Start()
    {
        changeButton.onClick.AddListener(ChangeBoard);
    }

    private void ChangeBoard()
    {
        GetComponent<Board>().Change();
    }
}
