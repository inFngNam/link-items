using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField] public Button hintButton;
    [SerializeField] public TextMeshProUGUI hintText;

    private int totalGetHintTime = 0;

    public void Start()
    {
        hintButton.onClick.AddListener(GetHint);
    }

    private void GetHint()
    {
        if (hintText.text == "HINT")
        {
            bool hasHint = GetComponent<GameController>().GetHint(totalGetHintTime);
            if (hasHint)
            {
                totalGetHintTime += 1;
            }
            else
            {
                hintText.text = "NO HINT";
            }
        }
    }

    public void ChangeText(string text)
    {
        hintText.text = text;
    }
}
