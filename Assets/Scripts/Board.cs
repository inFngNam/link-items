using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField] public GameObject boardGameObject;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite[] itemSprites;
    private GameObject[,] items;
    private GameObject hintItemOne;
    private GameObject hintItemTwo;
    private int level;
    static int totalColumns = 18;
    static int totalRows = 10;
    public static int totalItems = 8 * 16;
    private float startX = -9.2f;
    private float startY = 4.5f;
    private int[,] board;
    private List<int> values;
    public int GetTotalItems()
    {
        return totalItems;
    }

    private void Start()
    {
        level = PlayerPrefs.GetInt("level");
        itemSprites = Resources.LoadAll<Sprite>("Items");
        GetValues();
        SpawnTiles(ListValues());
        ClearHintItems();
    }


    private void GetValues()
    {
        values = new List<int>();

        for (int index = 0; index < totalItems / 2; index++)
        {
            int itemID = Random.Range(0, itemSprites.Length);
            values.Add(itemID);
        }
    }

    private List<int> ListValues()
    {
        List<int> listValues = new List<int>();

        for (int index = 0; index < values.Count; index++)
        {
            listValues.AddRange(new List<int> { values[index], values[index] });
        }

        for (int index = 0; index < listValues.Count; index++)
        {
            int temp = listValues[index];
            int randomIndex = Random.Range(index, listValues.Count);
            listValues[index] = listValues[randomIndex];
            listValues[randomIndex] = temp;
        }

        return listValues;
    }

    private void SpawnTiles(List<int> listValues)
    {
        items = new GameObject[totalRows, totalColumns];
        board = new int[totalRows, totalColumns];
        int index = 0;
        for (int row = 0; row < totalRows; row++)
        {
            for (int column = 0; column < totalColumns; column++)
            {
                if (column == 0 || column == totalColumns - 1 || row == 0 || row == totalRows - 1)
                {
                    items[row, column] = null;
                    board[row, column] = -1;
                }
                else
                {
                    int value = listValues[index];

                    Vector3 position = new Vector3(startX + column, startY - row, 0.0f);
                    GameObject item = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;
                    item.name = "Item[" + row + ", " + column + "]";

                    Item itemComponent = item.GetComponent<Item>();
                    itemComponent.row = row;
                    itemComponent.column = column;
                    itemComponent.value = value;

                    SpriteRenderer spriteRender = item.GetComponent<SpriteRenderer>();
                    spriteRender.sortingOrder = 2;
                    spriteRender.sprite = itemSprites[itemComponent.value];

                    item.transform.localScale = new Vector3(0.76f, 0.76f, 1f);
                    item.transform.SetParent(boardGameObject.transform);

                    items[row, column] = item;
                    board[row, column] = value;

                    index += 1;
                }
            }
        }
    }

    public void Clear(Position firstPosition, Position secondPosition)
    {
        int value = board[firstPosition.row, firstPosition.column];

        board[firstPosition.row, firstPosition.column] = -1;
        board[secondPosition.row, secondPosition.column] = -1;

        items[firstPosition.row, firstPosition.column] = null;
        items[secondPosition.row, secondPosition.column] = null;

        CheckHintAndClear(firstPosition, secondPosition);
        RestoreHintItemsColor();

        values.Remove(value);
    }

    private void CheckHintAndClear(Position firstPosition, Position secondPosition)
    {
        if (hintItemOne == null && hintItemTwo == null)
        {
            return;
        }

        Item hintItemOneComponent = hintItemOne.GetComponent<Item>();
        Item hintItemTwoComponent = hintItemTwo.GetComponent<Item>();

        int hintItemOneRow = hintItemOneComponent.row;
        int hintItemOneColumn = hintItemOneComponent.column;

        int hintItemTwoRow = hintItemTwoComponent.row;
        int hintItemTwoColumn = hintItemTwoComponent.column;

        int clearValue = board[firstPosition.row, firstPosition.column];
        int hintValue = board[hintItemOneRow, hintItemOneColumn];

        if (clearValue != hintValue)
        {
            return;
        }

        bool clearHintItems = false;

        if (firstPosition.row == hintItemOneRow && firstPosition.column == hintItemOneColumn)
        {
            clearHintItems = true;
        }

        if (secondPosition.row == hintItemOneRow && secondPosition.column == hintItemOneColumn)
        {
            clearHintItems = true;
        }

        if (firstPosition.row == hintItemTwoRow && firstPosition.column == hintItemTwoColumn)
        {
            clearHintItems = true;
        }

        if (secondPosition.row == hintItemTwoRow && secondPosition.column == hintItemTwoColumn)
        {
            clearHintItems = true;
        }

        if (clearHintItems)
        {
            ClearHintItems();
        }
    }

    public bool CheckConnection(Position startPosition, Position endPosition, bool drawPath)
    {
        if (IsEndPositionBlocked(startPosition, endPosition))
        {
            return false;
        }

        Queue<Node> nodes = new Queue<Node>();
        Node start = new Node(startPosition.row, startPosition.column);
        start.moves = "";
        start.positions = new List<Position>();
        start.positions.Add(startPosition);
        start.change = 0;
        start.isStartNode = true;
        nodes.Enqueue(start);

        while (nodes.Count != 0)
        {
            Node currentNode = nodes.Dequeue();

            if (currentNode.change > 2)
            {
                continue;
            }

            if (currentNode.row == endPosition.row && currentNode.column == endPosition.column)
            {
                List<Position> path = currentNode.positions;

                // string msg = "";
                // for (int index = 0; index < path.Count; index++)
                // {
                //     msg += "[" + path[index].row + ", " + path[index].column + "] ";
                // }

                if (drawPath)
                {
                    if (board[startPosition.row, startPosition.column] == board[endPosition.row, endPosition.column])
                    {
                        GameObject.Find("Path").GetComponent<Path>().DrawPath(path);
                    }
                }
                return true;
            }

            int[,] directions = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int index = 0; index < directions.GetLength(0); index++)
            {
                int row = currentNode.row + directions[index, 0];
                int column = currentNode.column + directions[index, 1];

                if (!IsValid(row, column))
                {
                    continue;
                }

                int currentNodeValue = board[currentNode.row, currentNode.column];

                if (currentNode.isStartNode || currentNodeValue == -1)
                {
                    bool skip = false;
                    List<Position> positions = new List<Position>();
                    foreach (Position position in currentNode.positions)
                    {
                        if (position.row == row && position.column == column)
                        {
                            skip = true;
                        }
                        positions.Add(position);
                    }

                    if (skip)
                    {
                        continue;
                    }

                    Node node = new Node(row, column);
                    node.change = currentNode.change;
                    node.moves = (string)currentNode.moves.Clone();
                    node.positions = positions;
                    node.AddPosition(new Position(row, column));
                    nodes.Enqueue(node);
                }
            }
        }

        return false;
    }

    private bool IsValid(int row, int column)
    {
        if (row < 0 || column < 0 || row >= totalRows || column >= totalColumns)
        {
            return false;
        }
        return true;
    }

    private bool IsEndPositionBlocked(Position start, Position end)
    {
        int[,] directions = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        for (int index = 0; index < directions.GetLength(0); index++)
        {
            int row = end.row + directions[index, 0];
            int column = end.column + directions[index, 1];

            if (row == start.row && column == start.column)
            {
                return false;
            }
            else if (board[row, column] == -1)
            {
                return false;
            }
        }
        return true;
    }


    public void Change()
    {
        List<int> listValues = ListValues();
        int valueIndex = 0;

        for (int row = 0; row < items.GetLength(0); row++)
        {
            for (int column = 0; column < items.GetLength(1); column++)
            {
                GameObject item = items[row, column];

                if (item == null)
                {
                    continue;
                }

                int value = listValues[valueIndex];

                Item itemComponent = item.GetComponent<Item>();
                itemComponent.value = value;

                SpriteRenderer spriteRender = item.GetComponent<SpriteRenderer>();
                spriteRender.sprite = itemSprites[itemComponent.value];

                board[row, column] = value;
                valueIndex++;
            }
        }

        ClearHintItems();
    }

    public bool GetHint()
    {
        if (hintItemOne != null && hintItemTwo != null)
        {
            ShowHintItemsColor();
            return true;
        }

        List<int> checkedValues = new List<int>();

        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
            {
                if (board[row, column] == -1 || checkedValues.Contains(board[row, column]))
                {
                    continue;
                }

                Position firstPosition = new Position(row, column);
                List<Position> positions = GetSameValuePositions(firstPosition, board[row, column]);

                checkedValues.Add(board[row, column]);

                foreach (Position position in positions)
                {
                    if (CheckConnection(firstPosition, position, false))
                    {
                        hintItemOne = items[row, column];
                        hintItemTwo = items[position.row, position.column];
                        ShowHintItemsColor();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private List<Position> GetSameValuePositions(Position currentPosition, int value)
    {
        List<Position> positions = new List<Position>();
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
            {
                if (currentPosition.row == row && currentPosition.column == column)
                {
                    continue;
                }

                if (board[row, column] == value)
                {
                    Position position = new Position(row, column);
                    positions.Add(position);
                }
            }
        }
        return positions;
    }

    private void RestoreHintItemsColor()
    {
        if (hintItemOne != null && hintItemTwo != null)
        {
            Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            hintItemOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
            hintItemTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
        }
    }

    private void ShowHintItemsColor()
    {
        if (hintItemOne != null && hintItemTwo != null)
        {
            Color color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
            hintItemOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
            hintItemTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void ClearHintItems()
    {
        if (hintItemOne != null && hintItemTwo != null)
        {
            Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            hintItemOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
            hintItemTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
        }
        hintItemOne = null;
        hintItemTwo = null;
    }

    public void UpdateBoardLevel2(Position firstPosition, Position secondPosition)
    {
        MoveAllItemsDown(firstPosition.column);
        MoveAllItemsDown(secondPosition.column);
    }

    private void MoveAllItemsDown(int column)
    {
        for (int rowI = totalRows - 2; rowI >= 1; rowI--)
        {
            for (int rowJ = totalRows - 2; rowJ >= 2; rowJ--)
            {
                if (board[rowJ, column] == -1 && board[rowJ - 1, column] != -1)
                {
                    int temp = board[rowJ, column];
                    board[rowJ, column] = board[rowJ - 1, column];
                    board[rowJ - 1, column] = temp;

                    GameObject tempItem = items[rowJ, column];
                    items[rowJ, column] = items[rowJ - 1, column];
                    items[rowJ - 1, column] = tempItem;
                }
            }
        }

        for (int row = 0; row < totalRows; row++)
        {
            GameObject item = items[row, column];
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

    public void UpdateBoardLevel3(Position firstPosition, Position secondPosition)
    {
        MoveAllItemUp(firstPosition.column);
        MoveAllItemUp(secondPosition.column);
    }

    private void MoveAllItemUp(int column)
    {
        for (int rowI = 1; rowI < (totalRows - 2); rowI++)
        {
            for (int rowJ = 1; rowJ < totalRows - rowI - 1; rowJ++)
            {
                if (board[rowJ, column] == -1 && board[rowJ + 1, column] != -1)
                {
                    int temp = board[rowJ, column];
                    board[rowJ, column] = board[rowJ + 1, column];
                    board[rowJ + 1, column] = temp;

                    GameObject tempItem = items[rowJ, column];
                    items[rowJ, column] = items[rowJ + 1, column];
                    items[rowJ + 1, column] = tempItem;
                }
            }
        }

        for (int row = 0; row < totalRows; row++)
        {
            GameObject item = items[row, column];
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
