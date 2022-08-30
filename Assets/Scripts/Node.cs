using System;
using System.Collections.Generic;

public class Node
{
    public int row, column, change;
    public string moves;
    public List<Position> positions;
    public bool isStartNode;

    public Node(int _row, int _column)
    {
        row = _row;
        column = _column;
    }

    public void AddPosition(Position position)
    {
        if (positions.Count == 0)
        {
            moves = "";
        }
        else
        {
            Position lastPosition = positions[positions.Count - 1];

            string move = position.GetMove(lastPosition);
            if (moves != "")
            {
                string lastMove = moves.Substring(moves.Length - 1);
                if (lastMove != move)
                {
                    change += 1;
                }
            }
            moves += move;
        }
        positions.Add(position);
    }
}
