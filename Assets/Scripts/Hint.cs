using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField] public Button hintButton;
    private int totalGetHintTime = 0;

    public void Start()
    {
        hintButton.onClick.AddListener(GetHint);
    }

    private void GetHint()
    {
        totalGetHintTime += 1;
        GetComponent<GameController>().GetHint(totalGetHintTime);
    }
}
