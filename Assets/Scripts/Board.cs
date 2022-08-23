using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
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
            return this.row == 0 || this.row == totalRows - 1 || this.column == 0 || this.column == totalColumns - 1;
        }

        public bool IsCorner()
        {
            return (this.row == 0 || this.row == totalRows - 1) && (this.column == 0 || this.column == totalColumns - 1);
        }
    }

    [SerializeField]
    public GameObject boardGameObject;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private Sprite[] itemSprites;

    private List<GameObject> items;

    static int totalColumns = 16;
    static int totalRows = 8;
    static int totalItems = 8 * 16;
    private float start_x = -8.9f;
    private float start_y = 3.5f;

    private List<List<int>> board;

    private List<int> listItemIDs;

    public int GetTotalItems()
    {
        return totalItems;
    }

    private void Start()
    {
        items = new List<GameObject>();
        itemSprites = Resources.LoadAll<Sprite>("Items");

        listItemIDs = new List<int>();
        board = new List<List<int>>();

        SpawnTiles(GetListItems());
    }

    private void Update()
    {
        AutoChangeBoard();
    }

    private List<int> GetListItems()
    {
        List<int> listItems = new List<int>();

        for (int index = 0; index < totalItems / 2; index++)
        {
            int itemID = Random.Range(0, itemSprites.Length);
            listItems.Add(itemID);
            listItems.Add(itemID);
            listItemIDs.Add(itemID);
        }

        for (int index = 0; index < listItems.Count; index++)
        {
            int temp = listItems[index];
            int randomIndex = Random.Range(index, listItems.Count);
            listItems[index] = listItems[randomIndex];
            listItems[randomIndex] = temp;
        }

        DebugListItemIDs();

        return listItems;
    }

    private void SpawnTiles(List<int> listItems)
    {
        int index = 0;

        for (int row = 0; row < totalRows; row++)
        {
            List<int> rowItems = new List<int>();
            for (int column = 0; column < totalColumns; column++)
            {
                Vector3 position = new Vector3(start_x + column, start_y - row, 0.0f);
                GameObject item = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;
                item.name = "Item[" + row + ", " + column + "]";

                var itemComponent = item.GetComponent<Item>();
                itemComponent.row = row;
                itemComponent.column = column;
                itemComponent.value = listItems[index];

                item.GetComponent<Transform>().localScale = new Vector3(0.83f, 0.83f, 1f);

                var spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = 2;
                spriteRender.sprite = itemSprites[itemComponent.value];
                index += 1;

                rowItems.Add(itemComponent.value);
                items.Add(item);

                item.transform.SetParent(boardGameObject.transform);
            }
            board.Add(rowItems);
        }
    }

    public void Clear(int firstItemRow, int firstItemColumn, int secondItemRow, int secondItemColumn)
    {
        int value = board[firstItemRow][firstItemColumn];
        board[firstItemRow][firstItemColumn] = -1;
        board[secondItemRow][secondItemColumn] = -1;
        listItemIDs.Remove(value);
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

            for (int directionIndex = 0; directionIndex < direction.GetLength(0); directionIndex++)
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

        DebugBoard(visited);
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
            if (currentNode.IsCorner())
            {
                return false;
            }
            else
            {
                return newNode.IsBorder();
            }
        }
        return false;
    }

    private void GetNewNodeExpandable(Node currentNode, Node newNode)
    {
        if (board[newNode.row][newNode.column] == -1)
        {
            newNode.isFullExpandable = true;
        }
        else
        {
            if (newNode.IsBorder())
            {
                if (newNode.IsCorner())
                {
                    newNode.isHalfExpandable = false;
                }
                else
                {
                    newNode.isHalfExpandable = currentNode.IsBorder() && (currentNode.isHalfExpandable || currentNode.isFullExpandable);
                }
            }
        }
    }

    public void Change()
    {
        List<int> listItems = new List<int>();

        DebugListItemIDs();

        for (int index = 0; index < listItemIDs.Count; index++)
        {
            int value = listItemIDs[index];
            listItems.Add(value);
            listItems.Add(value);
        }

        for (int index = 0; index < listItems.Count; index++)
        {
            int temp = listItems[index];
            int randomIndex = Random.Range(index, listItems.Count);
            listItems[index] = listItems[randomIndex];
            listItems[randomIndex] = temp;
        }

        var msg = "";

        for (int index = 0; index < listItems.Count; index++)
        {
            msg += " " + listItems[index];
        }

        Debug.Log(msg);


        items = items.Where(item => item != null).ToList();

        for (int index = 0; index < items.Count; index++)
        {
            int value = listItems[index];

            GameObject item = items[index];

            Item itemComponent = item.GetComponent<Item>();
            itemComponent.value = value;

            var spriteRender = item.GetComponent<SpriteRenderer>();
            spriteRender.sortingOrder = 2;
            spriteRender.sprite = itemSprites[itemComponent.value];

            board[itemComponent.row][itemComponent.column] = value;
        }
    }

    private void DebugBoard()
    {
        var msg = "";

        for (int row = 0; row < board.Count; row++)
        {
            for (int column = 0; column < board[0].Count; column++)
            {
                msg += " " + board[row][column];
            }
            msg += "\n";
        }

        Debug.Log(msg);
    }

    private void DebugBoard(bool[,] array)
    {
        var msg = "";

        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int column = 0; column < array.GetLength(1); column++)
            {
                msg += " " + array[row, column];
            }
            msg += "\n";
        }

        Debug.Log(msg);
    }

    private void DebugListItemIDs()
    {
        var msg = "";

        for (int index = 0; index < listItemIDs.Count; index++)
        {
            msg += " " + listItemIDs[index];
        }

        Debug.Log(msg);
    }

    private void AutoChangeBoard()
    {
        bool change = false;

        if (change)
        {
            Change();
        }
    }
}
