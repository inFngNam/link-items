using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int column, row;

    public Position(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public string GetMove(Position previousPosition)
    {
        int deltaRow = row - previousPosition.row;
        int deltaColumn = column - previousPosition.column;

        if (deltaColumn == 0)
        {
            if (deltaRow == 1)
            {
                return "D";
            }
            else if (deltaRow == -1)
            {
                return "U";
            }
        }
        else if (deltaRow == 0)
        {
            if (deltaColumn == 1)
            {
                return "L";
            }
            else if (deltaColumn == -1)
            {
                return "R";
            }
        }
        return "";
    }
}
