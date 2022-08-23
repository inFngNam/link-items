// private void DebugConnectionNodes()
// {
//     for (int index = 0; index < connectedPositions.Count; index++)
//     {
//         string msg = "";
//         Position firstPosition = connectedPositions[index].GetPosition(0);
//         Position secondPosition = connectedPositions[index].GetPosition(1);
//         msg += "[" + firstPosition.row + ", " + firstPosition.column + "] ";
//         msg += "[" + secondPosition.row + ", " + secondPosition.column + "] ";
//         Debug.Log(msg);
//     }
// }

// private void DebugBoard()
// {
//     var msg = "";

//     for (int row = 0; row < board.Count; row++)
//     {
//         for (int column = 0; column < board[0].Count; column++)
//         {
//             msg += " " + board[row][column];
//         }
//         msg += "\n";
//     }

//     Debug.Log(msg);
// }

// private void DebugBoard(bool[,] array)
// {
//     var msg = "";

//     for (int row = 0; row < array.GetLength(0); row++)
//     {
//         for (int column = 0; column < array.GetLength(1); column++)
//         {
//             msg += " " + array[row, column];
//         }
//         msg += "\n";
//     }

//     Debug.Log(msg);
// }

// private void DebugListItemIDs()
// {
//     var msg = "";

//     for (int index = 0; index < listItemIDs.Count; index++)
//     {
//         msg += " " + listItemIDs[index];
//     }

//     Debug.Log(msg);
// }