using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField]
    private GameObject itemBackground;

    public int row_index;
    public int column_index;
    public int value;

    public void OnMouseDown()
    {
        var gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if (gameController.IsSelected())
        {
            gameController.SelectSecondItem(gameObject);
        }
        else
        {
            var itemBackgroundSpriteRender = itemBackground.GetComponent<SpriteRenderer>();
            itemBackgroundSpriteRender.color = new Color(253.0f/255.0f, 225.0f/255.0f, 225.0f/255.0f, 38.0f/255.0f);
            gameController.SelectFirstItem(gameObject);
        }
    }
}