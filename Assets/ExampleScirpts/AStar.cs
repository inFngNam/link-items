using System;
using System.Collections;
using System.Collections.Generic;

// A* Example
public class AStar
{
    public class Spot
    {
        public int f, g, h;
        public int row, column;
        public bool isBlock;
        public Spot parent;

        public Spot(int _row, int _column, bool _isBlock)
        {
            row = _row;
            column = _column;
            isBlock = _isBlock;

            f = 0;
            g = 0;
            h = 0;
        }


        public int ManhattanDistanceHeuristic(Spot target)
        {
            return Math.Abs(row - target.row) + Math.Abs(column - target.column);
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

        Spot[,] spots = new Spot[totalRows, totalColumns];

        for (int row = 0; row < totalRows; row++)
        {
            for (int column = 0; column < totalColumns; column++)
            {
                int value = board[row, column];
                spots[row, column] = new Spot(row, column, value == 0);
            }
        }

        List<Spot> openSet = new List<Spot>();
        List<Spot> closedSet = new List<Spot>();

        Spot start = spots[3, 3];
        Spot end = spots[3, 0];

        openSet.Add(start);

        while (openSet.Count != 0)
        {
            int lowestIndex = 0;
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].f < openSet[lowestIndex].f)
                {
                    lowestIndex = i;
                }
            }

            Spot currentSpot = openSet[lowestIndex];

            if (currentSpot == end)
            {
                List<string> path = new List<string>();
                Spot temp = currentSpot;
                while (temp.parent != null)
                {
                    path.Add("[" + temp.row + ", " + temp.column + "]");
                    temp = temp.parent;
                }

                path.Reverse();

                for (int i = 0; i < path.Count; i++)
                {
                    Console.WriteLine(path[i]);
                }

                return;
            }

            openSet.Remove(currentSpot);
            closedSet.Add(currentSpot);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == j || i == -j)
                    {
                        continue;
                    }

                    int nextRow = currentSpot.row + i;
                    if (nextRow == -1 || nextRow == totalRows)
                    {
                        continue;
                    }

                    int nextColumn = currentSpot.column + j;
                    if (nextColumn == -1 || nextColumn == totalColumns)
                    {
                        continue;
                    }

                    Spot neighborSpot = spots[nextRow, nextColumn];

                    if (closedSet.Contains(neighborSpot) || neighborSpot.isBlock)
                    {
                        continue;
                    }

                    int newG = currentSpot.g + 1;

                    if (openSet.Contains(neighborSpot))
                    {
                        if (newG < neighborSpot.g)
                        {
                            neighborSpot.g = newG;
                        }
                    }
                    else
                    {
                        neighborSpot.g = newG;
                        openSet.Add(neighborSpot);
                    }

                    neighborSpot.h = neighborSpot.ManhattanDistanceHeuristic(end);
                    neighborSpot.f = neighborSpot.g + neighborSpot.h;
                    neighborSpot.parent = currentSpot;
                }
            }
        }

        Console.WriteLine("No Solution");
    }
}

