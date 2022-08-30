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


// 
// int[,] direction = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
// bool[,] visited = new bool[totalRows, totalColumns];
// Queue<Node> queue = new Queue<Node>();
// Node targetNode = null;
// 
// Node startNode = new Node(start.row, target.column, "", 0);
// queue.Enqueue(startNode);
// visited[startNode.row, startNode.column] = true;
// 
// while (queue.Count != 0)
// {
// Node currentNode = queue.Peek();
// int currentRow = currentNode.row;
// int currentColumn = currentNode.column;
// string path = currentNode.path;
// int change = currentNode.change;
// queue.Dequeue();
// 
// if (currentRow == target.row && currentColumn == target.column)
// {
// targetNode = currentNode;
// break;
// }
// 
// for (int index = 0; index < direction.GetLength(0); index++)
// {
// int nextRow = currentRow + direction[index, 0];
// int nextColumn = currentColumn + direction[index, 1];
// Node nextNode = new Node(nextRow, nextColumn, path, change);
// 
// switch (index)
// {
// case 0:
// nextNode.AddMove("R");
// break;
// case 1:
// nextNode.AddMove("L");
// break;
// case 2:
// nextNode.AddMove("D");
// break;
// default:
// nextNode.AddMove("U");
// break;
// }
// 
// if (IsValid(visited, nextRow, nextColumn))
// {
// if (currentColumn == start.column && currentRow == start.row)
// {
// visited[nextRow, nextColumn] = true;
// }
// else
// {
// visited[nextRow, nextColumn] = CheckVisitable(currentNode);
// }
// 
// if (visited[nextRow, nextColumn])
// {
// queue.Enqueue(nextNode);
// }
// }
// }
// }
// 
// if (targetNode != null)
// {
// 
// }
// return false;