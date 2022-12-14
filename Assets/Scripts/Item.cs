using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private GameObject itemBackground;
    public int row, column, value;

    public void OnMouseDown()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if (gameController.IsPause() || gameController.IsGameOver())
        {
            return;
        }

        SpriteRenderer itemBackgroundSpriteRender = itemBackground.GetComponent<SpriteRenderer>();

        if (gameController.IsSelected())
        {
            if (gameController.GetFirstItem() != gameObject)
            {
                itemBackgroundSpriteRender.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                gameController.SelectSecondItem(gameObject);
            }
        }
        else
        {
            itemBackgroundSpriteRender.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
            gameController.SelectFirstItem(gameObject);
        }
    }
}