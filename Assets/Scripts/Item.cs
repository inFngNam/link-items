using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField]
    private GameObject itemBackground;

    public int row;
    public int column;
    public int value;

    public void OnMouseDown()
    {
        var gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if (gameController.IsSelected())
        {
            var firstItem = gameController.GetFirstItem();

            if (firstItem != gameObject)
            {
                gameController.SelectSecondItem(gameObject);
            }
        }
        else
        {
            var itemBackgroundSpriteRender = itemBackground.GetComponent<SpriteRenderer>();
            itemBackgroundSpriteRender.color = new Color(121f/255f, 185f/255f, 183f/255f, 72f/255f);
            gameController.SelectFirstItem(gameObject);
        }
    }
}