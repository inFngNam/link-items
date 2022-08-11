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

    private List<GameObject> items;

    static int totalColumns = 16;
    static int totalRows = 8;
    private int totalItems = 16 * 8;
    
    private int[,] grid;

    private float start_x = -7.5f;
    private float start_y = 3.5f;

    private List<List<int>> connectedItems;

    public int GetTotalItems()
    {
        return totalItems;
    }

    public void Start()
    {
        items = new List<GameObject>();
        itemSprites = Resources.LoadAll<Sprite>("Items");
        totalItemSprites = itemSprites.Length;
        SpawnTiles(GetListItems());
    }

    private List<int> GetListItems()
    {
        List<int> listItems = new List<int>();
        for (int index = 0; index < totalItems / 2; index++)
        {
            int item_id = Random.Range(0, totalItemSprites); 
            listItems.Add(item_id);
            listItems.Add(item_id);
        }

        for (int index = 0; index < listItems.Count; index++)
        {
            int temp = listItems[index];
            int randomIndex = Random.Range(index, listItems.Count);
            listItems[index] = listItems[randomIndex];
            listItems[randomIndex] = temp;
        }
        return listItems;
    }

    private void SpawnTiles(List<int> listItems)
    {
        int index = 0;
        grid = new int[totalRows, totalColumns];

        connectedItems = new List<List<int>>();

        for (int row = 0; row < totalRows; row++)
        {
            List<int> rowItems = new List<int>();
            for (int column = 0; column < totalColumns; column++)
            {
                Vector3 position = new Vector3(start_x + column, start_y - row, 0.0f);
                GameObject item = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;
                item.name = "("+row+", "+column+")";

                var itemComponent = item.GetComponent<Item>();
                itemComponent.row = row;
                itemComponent.column = column;
                itemComponent.value = listItems[index];

                var spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = 2;
                spriteRender.sprite = itemSprites[itemComponent.value];
                index += 1;

                rowItems.Add(itemComponent.value);

                items.Add(item);
            }
            connectedItems.Add(rowItems);
        }
    }

    public void Clear(int firstItemRow, int firstItemColumn, int secondItemRow, int secondItemColumn)
    {
        connectedItems[firstItemRow][firstItemColumn] = -1;
        connectedItems[secondItemRow][secondItemColumn] = -1;
    }

    class Node
    {
        public int column, row;
        public bool isFullExpandable, isHalfExpandable;
 
        public Node(int row, int column, bool isFullExpandable) 
        {
            this.row = row;
            this.column = column;
            this.isFullExpandable = isFullExpandable;
            this.isHalfExpandable = isFullExpandable;
        }

        public bool IsBorder()
        {
            if (this.row == 0 || this.row == totalRows - 1 || this.column == 0 || this.column == totalColumns - 1)
            {
                return true;
            }
            return false;
        }
    }

    public bool CheckConnection(int firstItemRow, int firstItemColumn, int secondItemRow, int secondItemColumn)
    {
        int[,] direction = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        bool[,] visited = new bool[totalRows, totalColumns];

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(new Node(firstItemRow, firstItemColumn, true));
        visited[firstItemRow, firstItemColumn] = true;

        while (queue.Count != 0)
        {
            Node node = queue.Peek();

            int currentRow = node.row;
            int currentColumn = node.column;
            bool currentNodeFullExpandable = node.isFullExpandable;
            bool currentNodeHalfExpandable = node.isHalfExpandable;

            queue.Dequeue();

            for(int directionIndex = 0; directionIndex < direction.GetLength(0); directionIndex++)
            {
                int row = currentRow + direction[directionIndex, 0];
                int column = currentColumn + direction[directionIndex, 1];

                Node newNode = new Node(row, column, false);

                if (IsValid(visited, row, column))
                {
                    visited[row, column] = GetNewNodeVisitable(node, newNode);
                    GetNewNodeExpandable(node, newNode);

                    if (visited[row, column])
                    {
                        queue.Enqueue(newNode);
                    }
                }
            }
        }
        return visited[secondItemRow, secondItemColumn];
    }

    private bool IsValid(bool[,] visited, int row, int column)
    {
        if (row < 0 || column < 0 || row >= totalRows || column >= totalColumns)
        {
            return false;
        }
        if (visited[row, column])
        {
            return false;
        }
        return true;
    }

    private bool GetNewNodeVisitable(Node currentNode, Node newNode)
    {
        if (currentNode.isFullExpandable)
        {
            return true;
        }
        else if (currentNode.isHalfExpandable)
        {
            return newNode.IsBorder();
        }
        else
        {
            if (connectedItems[newNode.row][newNode.column] == -1)
            {
                return currentNode.isHalfExpandable || currentNode.isFullExpandable;
            }
            else
            {
                return false;
            }
        }
    }

    private void GetNewNodeExpandable(Node currentNode, Node newNode)
    {
        if (connectedItems[newNode.row][newNode.column] == -1)
        {
            newNode.isFullExpandable = true;
        }

        if (newNode.IsBorder())
        {
            if (currentNode.IsBorder() && (currentNode.isHalfExpandable || currentNode.isFullExpandable))
            {
                newNode.isHalfExpandable = true;
            }
        }
    }

    public void Change()
    {
        List<int> remainItems = new List<int>();

        for (int row = 0; row < connectedItems.Count; row++)
        {
            for (int column = 0; column < connectedItems[0].Count; column++)
            {
                if (connectedItems[row][column] != -1)
                {
                    remainItems.Add(connectedItems[row][column]);
                }
            }
        }

        for (int index = 0; index < remainItems.Count; index++)
        {
            int temp = remainItems[index];
            int randomIndex = Random.Range(index, remainItems.Count);
            remainItems[index] = remainItems[randomIndex];
            remainItems[randomIndex] = temp;
        }

        items = items.Where(item => item != null).ToList();

        for (int index = 0; index < remainItems.Count; index++)
        {
            int value = remainItems[index];

            GameObject item = items[index];

            Item itemComponent = item.GetComponent<Item>();
            itemComponent.value = value;

            var spriteRender = item.GetComponent<SpriteRenderer>();
            spriteRender.sortingOrder = 2;
            spriteRender.sprite = itemSprites[itemComponent.value];
        }
    }
}
