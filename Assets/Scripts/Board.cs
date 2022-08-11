using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private Sprite[] itemSprites;
    private int totalItemSprites;

    private int totalColumns = 16;
    private int totalRows = 8;
    private int totalItems = 16 * 8;
    
    private int[,] grid;

    private float start_x = -7.5f;
    private float start_y = 3.5f;

    private List<int> items;

    public int GetTotalItems()
    {
        return totalItems;
    }

    public void Start()
    {
        itemSprites = Resources.LoadAll<Sprite>("Items");
        totalItemSprites = itemSprites.Length;

        GetListItems();
        SpawnTiles();
    }

    private void GetListItems()
    {
        items = new List<int>();
        for (int index = 0; index < totalItems / 2; index++)
        {
            int item_id = Random.Range(0, totalItemSprites); 
            items.Add(item_id);
            items.Add(item_id);
        }

        for (int index = 0; index < items.Count; index++)
        {
            int temp = items[index];
            int randomIndex = Random.Range(index, items.Count);
            items[index] = items[randomIndex];
            items[randomIndex] = temp;
        }
    }

    private void SpawnTiles()
    {
        int index = 0;
        grid = new int[totalRows, totalColumns];
        for (int row = 0; row < totalRows; row++)
        {
            for (int column = 0; column < totalColumns; column++)
            {
                Vector3 position = new Vector3(start_x + column, start_y - row, 0.0f);
                GameObject item = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;
                item.name = "("+row+", "+column+")";

                var itemComponent = item.GetComponent<Item>();
                itemComponent.row_index = row;
                itemComponent.column_index = column;
                itemComponent.value = items[index];

                var spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = 2;
                spriteRender.sprite = itemSprites[itemComponent.value];
                index += 1;
            }
        }
    }
}
