using System;
using System.Collections;
using System.Collections.Generic;

// A* Example
public class AllPath
{
    public class Node
    {
        public int row, column;
        public List<Position> positions;

        public Node(int _row, int _column, List<Position> _positions)
        {
            row = _row;
            column = _column;
            positions = _positions;
        }

        public bool CheckContains(int _row, int _column)
        {
            foreach (Position position in positions)
            {
                if (position.row == row && position.column == column)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class Position
    {
        public int row, column;

        public Position(int _row, int _column)
        {
            row = _row;
            column = _column;
        }
    }

    static int totalColumns = 5;
    static int totalRows = 5;
    private int[,] board;

    public static void Main()
    {
        int[,] board = new int[,] {
            { 1, 0, 1, 1, 0 },
            { 1, 0, 1, 0, 0 },
            { 1, 0, 1, 1, 1 },
            { 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1 },
        };

        int[,] directions = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        Queue<Node> queue = new Queue<Node>();
        Node start = new Node(0, 0, new List<Position>());
        queue.Enqueue(start);

        while (queue.Count != 0)
        {
            Node currentNode = queue.Dequeue();

            if (currentNode.row == 4 && currentNode.column == 0)
            {
                List<Position> positions = currentNode.positions;

                string msg = "";
                for (int index = 0; index < positions.Count; index++)
                {
                    msg += "[" + positions[index].row + ", " + positions[index].column + "] ";
                }
                Console.WriteLine(msg);
                continue;
            }


            for (int index = 0; index < directions.GetLength(0); index++)
            {
                List<Position> positions = new List<Position>();

                int row = currentNode.row + directions[index, 0];
                int column = currentNode.column + directions[index, 1];

                if (!IsValid(row, column))
                {
                    continue;
                }

                int value = board[row, column];

                if (value == 0)
                {
                    continue;
                }

                bool skip = false;

                foreach (Position position in currentNode.positions)
                {
                    if (position.row == row && position.column == column)
                    {
                        skip = true;
                        break;
                    }
                    positions.Add(position);
                }

                if (skip)
                {
                    continue;
                }

                positions.Add(new Position(row, column));
                Node node = new Node(row, column, positions);
                queue.Enqueue(node);
            }
        }
    }

    static bool IsValid(int row, int column)
    {
        if (row < 0 || column < 0 || row >= totalRows || column >= totalColumns)
        {
            return false;
        }
        return true;
    }
}