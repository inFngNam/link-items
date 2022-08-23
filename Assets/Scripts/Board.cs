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

    class Position
    {
        public int column, row;

        public Position(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }

    class Positions
    {
        private List<Position> listPositions;

        public Positions()
        {
            listPositions = new List<Position>();
        }

        public void Add(Position position)
        {
            listPositions.Add(position);
        }

        public bool Contains(Position checkPosition)
        {
            for (int i = 0; i < listPositions.Count; i++)
            {
                if (checkPosition.column == listPositions[i].column && listPositions[i].row == checkPosition.row)
                {
                    return true;
                }
            }
            return false;
        }

        public Position GetPosition(int index)
        {
            return listPositions[index];
        }
    }

    [SerializeField] public GameObject boardGameObject;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite[] itemSprites;
    private List<List<GameObject>> items;

    private GameObject hintItemOne;
    private GameObject hintItemTwo;

    private int level;
    static int totalColumns = 16;
    static int totalRows = 8;
    public static int totalItems = 8 * 16;
    private float startX = -8.9f;
    private float startY = 3.5f;
    private List<List<int>> board;
    private List<int> listItemIDs;
    private List<Positions> connectedPositions;

    public int GetTotalItems()
    {
        return totalItems;
    }

    private void Start()
    {
        level = PlayerPrefs.GetInt("level");
        items = new List<List<GameObject>>();
        itemSprites = Resources.LoadAll<Sprite>("Items");
        listItemIDs = new List<int>();
        board = new List<List<int>>();
        SpawnTiles(GetListItems());
        GetAllConnect();
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

        return listItems;
    }

    private void SpawnTiles(List<int> listItems)
    {
        int index = 0;
        for (int row = 0; row < totalRows; row++)
        {
            List<int> boardLine = new List<int>();
            List<GameObject> rowItems = new List<GameObject>();
            for (int column = 0; column < totalColumns; column++)
            {
                Vector3 position = new Vector3(startX + column, startY - row, 0.0f);
                GameObject item = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;
                item.name = "Item[" + row + ", " + column + "]";

                Item itemComponent = item.GetComponent<Item>();
                itemComponent.row = row;
                itemComponent.column = column;
                itemComponent.value = listItems[index];

                item.GetComponent<Transform>().localScale = new Vector3(0.76f, 0.76f, 1f);

                SpriteRenderer spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = 2;
                spriteRender.sprite = itemSprites[itemComponent.value];

                boardLine.Add(itemComponent.value);
                rowItems.Add(item);

                item.transform.SetParent(boardGameObject.transform);
                index += 1;
            }
            board.Add(boardLine);
            items.Add(rowItems);
        }
    }

    public void Clear(int firstItemRow, int firstItemColumn, int secondItemRow, int secondItemColumn)
    {
        int value = board[firstItemRow][firstItemColumn];
        board[firstItemRow][firstItemColumn] = -1;
        board[secondItemRow][secondItemColumn] = -1;

        Position firstPosition = new Position(firstItemRow, firstItemColumn);
        Position secondPosition = new Position(secondItemRow, secondItemColumn);

        for (int index = 0; index < connectedPositions.Count; index++)
        {
            Positions pair = connectedPositions[index];
            if (pair.Contains(firstPosition) || pair.Contains(secondPosition))
            {
                connectedPositions.RemoveAt(index);
            }
        }

        items[firstItemRow][firstItemColumn] = null;
        items[secondItemRow][secondItemColumn] = null;

        if (level == 2)
        {
            ChangeBoardLevel2(firstPosition, secondPosition);
        }
        else if (level == 3)
        {
            ChangeBoardLevel3(firstPosition, secondPosition);
        }

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
                    newNode.isHalfExpandable =
                        currentNode.IsBorder() && (currentNode.isHalfExpandable || currentNode.isFullExpandable);
                }
            }
        }
    }

    public void Change()
    {
        List<int> listItems = new List<int>();

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

        int valueIndex = 0;

        for (int row = 0; row < items.Count; row++)
        {
            for (int column = 0; column < items[0].Count; column++)
            {
                GameObject item = items[row][column];
                if (item == null)
                {
                    continue;
                }

                int value = listItems[valueIndex];

                Item itemComponent = item.GetComponent<Item>();
                itemComponent.value = value;

                SpriteRenderer spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = 2;
                spriteRender.sprite = itemSprites[itemComponent.value];

                board[itemComponent.row][itemComponent.column] = value;
                valueIndex++;
            }
        }

        if (hintItemOne != null && hintItemTwo != null)
        {
            hintItemOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            hintItemTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        GetAllConnect();
    }

    private void GetAllConnect()
    {
        connectedPositions = new List<Positions>();
        for (int row = 0; row < board.Count; row++)
        {
            for (int column = 0; column < board[0].Count; column++)
            {
                if (board[row][column] == -1)
                {
                    continue;
                }

                int firstValue = board[row][column];


                int[,] direction = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
                bool[,] visited = new bool[totalRows, totalColumns];

                Positions positions = new Positions();
                Position firstPosition = new Position(row, column);
                positions.Add(firstPosition);

                Queue<Node> queue = new Queue<Node>();
                Node firstNode = new Node(row, column, true);
                queue.Enqueue(firstNode);

                visited[row, column] = true;

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
                        int nextRow = currentRow + direction[directionIndex, 0];
                        int nextColumn = currentColumn + direction[directionIndex, 1];

                        Node nextNode = new Node(nextRow, nextColumn, false);

                        if (IsValid(visited, nextRow, nextColumn))
                        {
                            int nextValue = board[nextRow][nextColumn];
                            Position position = new Position(nextRow, nextColumn);

                            visited[nextRow, nextColumn] = GetNewNodeVisitable(node, nextNode);
                            GetNewNodeExpandable(node, nextNode);

                            if (visited[nextRow, nextColumn])
                            {
                                if (firstValue == nextValue)
                                {
                                    if (!positions.Contains(position))
                                    {
                                        bool shouldAdd = true;
                                        foreach (Positions pair in connectedPositions)
                                        {
                                            if (pair.Contains(firstPosition) && pair.Contains(position))
                                            {
                                                shouldAdd = false;
                                                break;
                                            }
                                        }

                                        if (shouldAdd)
                                        {
                                            Positions pair = new Positions();
                                            pair.Add(firstPosition);
                                            pair.Add(position);
                                            connectedPositions.Add(pair);
                                        }

                                        positions.Add(position);
                                    }
                                }
                                queue.Enqueue(nextNode);
                            }
                        }
                    }
                }
            }
        }
    }

    public bool GetHint()
    {
        bool result = false;
        if (connectedPositions.Count == 0)
        {
            GetAllConnect();
        }
        if (connectedPositions.Count != 0)
        {
            List<int> removeIndexes = new List<int>();
            for (int index = 0; index < connectedPositions.Count; index++)
            {
                Positions pair = connectedPositions[index];

                Position firstPosition = pair.GetPosition(0);
                Position secondPosition = pair.GetPosition(1);

                if (board[firstPosition.row][firstPosition.column] == -1 && board[secondPosition.row][secondPosition.column] == -1)
                {
                    removeIndexes.Add(index);
                    continue;
                }

                GameObject firstItem = items[firstPosition.row][firstPosition.column];
                GameObject secondItem = items[secondPosition.row][secondPosition.column];

                hintItemOne = firstItem;
                hintItemTwo = secondItem;

                if (firstItem != null && secondItem != null)
                {

                    Item firstItemComponent = firstItem.GetComponent<Item>();
                    Item secondItemComponent = firstItem.GetComponent<Item>();

                    if (firstItemComponent.value == secondItemComponent.value)
                    {
                        firstItem.transform.GetChild(0)
                            .GetComponent<SpriteRenderer>().color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
                        secondItem.transform.GetChild(0)
                            .GetComponent<SpriteRenderer>().color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
                        result = true;
                        break;
                    }
                    else
                    {
                        removeIndexes.Add(index);
                    }
                }
                else
                {
                    removeIndexes.Add(index);
                }
            }

            foreach (int index in removeIndexes)
            {
                if (connectedPositions.Count > removeIndexes[index])
                {
                    connectedPositions.RemoveAt(removeIndexes[index]);
                }
            }
        }

        return result;
    }

    private void RestoreColor()
    {

    }
    private void ChangeBoardLevel2(Position firstPosition, Position secondPosition)
    {
        MoveAllItemDown(firstPosition.column);
        MoveAllItemDown(secondPosition.column);
        GetAllConnect();
    }

    private void MoveAllItemDown(int column)
    {
        for (int rowI = totalRows - 1; rowI >= 0; rowI--)
        {
            for (int rowJ = totalRows - 1; rowJ >= 1; rowJ--)
            {
                if (board[rowJ][column] == -1 && board[rowJ - 1][column] != -1)
                {
                    int temp = board[rowJ][column];
                    board[rowJ][column] = board[rowJ - 1][column];
                    board[rowJ - 1][column] = temp;

                    GameObject tempItem = items[rowJ][column];
                    items[rowJ][column] = items[rowJ - 1][column];
                    items[rowJ - 1][column] = tempItem;
                }
            }
        }

        for (int row = 0; row < totalRows; row++)
        {
            GameObject item = items[row][column];
            if (item != null)
            {
                item.name = "Item[" + row + ", " + column + "]";

                Item itemComponent = item.GetComponent<Item>();
                itemComponent.row = row;

                SpriteRenderer spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sprite = itemSprites[itemComponent.value];

                Transform transform = item.GetComponent<Transform>();
                transform.position = new Vector3(startX + column, startY - row, 0.0f);
            }
        }
    }

    private void ChangeBoardLevel3(Position firstPosition, Position secondPosition)
    {
        MoveAllItemUp(firstPosition.column);
        MoveAllItemUp(secondPosition.column);
        GetAllConnect();
        for (int index = 0; index < connectedPositions.Count; index++)
        {
            string msg = "";
            Position _firstPosition = connectedPositions[index].GetPosition(0);
            Position _secondPosition = connectedPositions[index].GetPosition(1);
            msg += "[" + _firstPosition.row + ", " + _firstPosition.column + "] ";
            msg += "[" + _secondPosition.row + ", " + _secondPosition.column + "] ";
            Debug.Log(msg);
        }
    }

    private void MoveAllItemUp(int column)
    {
        for (int rowI = 0; rowI < totalRows; rowI++)
        {
            for (int rowJ = 0; rowJ < totalRows - rowI - 1; rowJ++)
            {
                if (board[rowJ][column] == -1 && board[rowJ + 1][column] != -1)
                {
                    int temp = board[rowJ][column];
                    board[rowJ][column] = board[rowJ + 1][column];
                    board[rowJ + 1][column] = temp;

                    GameObject tempItem = items[rowJ][column];
                    items[rowJ][column] = items[rowJ + 1][column];
                    items[rowJ + 1][column] = tempItem;
                }
            }
        }

        for (int row = 0; row < totalRows; row++)
        {
            GameObject item = items[row][column];
            if (item != null)
            {
                item.name = "Item[" + row + ", " + column + "]";

                Item itemComponent = item.GetComponent<Item>();
                itemComponent.row = row;

                SpriteRenderer spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sprite = itemSprites[itemComponent.value];

                Transform transform = item.GetComponent<Transform>();
                transform.position = new Vector3(startX + column, startY - row, 0.0f);
            }
        }
    }
}
