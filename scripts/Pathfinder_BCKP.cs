// using Godot;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

// public struct CellCost
// {
//     public Vector2 position;
//     public int cost;

//     public CellCost(Vector2 position, int cost)
//     {
//         this.position = position;
//         this.cost = cost;
//     }
// }

// public class CellCostQueue
// {
//     public List<CellCost> cellsCosts;

//     public CellCostQueue()
//     {
//         cellsCosts = new List<CellCost>();
//     }

//     public void AddCellCost(Vector2 position, int priority)
//     {
//         var cellCost = new CellCost(position, priority);
//         cellsCosts.Add(cellCost);
//     }

//     // gets highest priority (most efficient) position in queue
//     public CellCost GetHighestPriorityCell()
//     {
//         var prioritaryCell = this.cellsCosts.OrderBy(x => x.cost).First();
//         this.cellsCosts.Remove(prioritaryCell);

//         return prioritaryCell;
//     }

//     public void ClearQueue()
//     {
//         this.cellsCosts = new List<CellCost>();
//     }


//     public Vector2[] GetNeighbourCells(CellCost cell, Grid grid)
//     {
//         // array to hold all possible neighbours of current 'pos'
//         var neighboringCells = new List<Vector2>();
//         // minimum distance between cells
//         var next = new Vector2(30, 15);


//         // vector directions
//         var up = new Vector2(1, -1);
//         var down = new Vector2(-1, 1);
//         var right = new Vector2(1, 1);
//         var left = new Vector2(-1, -1);

//         //	diagonal vector directions (activate it if you want it)
//         // var w = new Vector2(0,2); 
//         // var d = new Vector2(2,0); 
//         // var s = new Vector2(0,-2);
//         // var a = new Vector2(-2,0);

//         // array of possible neighbors, yet to be validated
//         // only horizontal movement
//         var check = new Vector2[4] { up, down, right, left };

//         //with diagonal movement (activate it if you want)
//         //	var check = new Vector2[8]{up,down,right,left,w,a,s,d}

//         // if neighbour exists in grid and is "empty", append
//         foreach (var neighbor in check)
//         {
//             // (direction * minimum_distance) + pos = relative_neighbour
//             var _neighbor = neighbor * next + cell.position;


//             // skip if cell is blocked
//             if (grid.HasCell(_neighbor))
//             {

//                 // for diagonals:
//                 // skip condition, off by default
//                 //var skipTile = false;

//                 if (grid.CellIsEmpty(_neighbor) /*&& !skipTile*/)
//                 {
//                     neighboringCells.Add(_neighbor);
//                 }
//             }
//         }

//         return neighboringCells.ToArray();
//     }

//     public bool IsEmpty => this.cellsCosts == null || this.cellsCosts.Count == 0;
// }

// public class Pathfinder : Node
// {


//     // This node generates a path between origin and target based on parsed
//     // grid, start_point and target_point.

//     // It is really powerful as it can be changed to adapt to specific situations
//     // and favor certain routes above other possible ones if 'COST' is calculated
//     // differently, and so on.


//     // path finding

//     // get grid reference
//     Grid grid;
//     CellCostQueue queueCellsCosts;
//     //var queue_list = { } #Queue used in the A* search algorithm
//     List<Vector2> lockedPath;

//     public override void _Ready()
//     {
//         queueCellsCosts = new CellCostQueue();
//         var navNode = GetTree().GetNodesInGroup("nav")[0];
//         grid = (navNode as Nav).Grid;
//     }


//     void ClearLockedPath()
//     {
//         lockedPath = new List<Vector2>();
//     }

//     #region  A* Search
//     public Vector2[] Search(Vector2 startPosition, Vector2 targetPosition)
//     {

//         // if start_pos == target_pos, return empty array
//         if (startPosition == targetPosition)
//             return new Vector2[0];

//         // resets path A* 
//         ClearLockedPath();
//         this.queueCellsCosts.ClearQueue();

//         // set initial considerations
//         this.queueCellsCosts.AddCellCost(startPosition, 0);

//         CellCost current = new CellCost(new Vector2(0, 0), 0);
//         // var came_from = {} #dictionary for parent pos
//         // var cost_so_far = {} #dictionary with pos cost
//         // came_from[start_pos] = start_pos
//         // cost_so_far[start_pos] = 0
//         var costSoFar = new Dictionary<Vector2, int>();
//         costSoFar.Add(startPosition, 0);
//         Dictionary<Vector2, CellCost> cameFromPositionStack = new Dictionary<Vector2, CellCost>();

//         /*=============================   MAIN LOOP =============================  */

//         // while there are pos in queue list
//         while (!this.queueCellsCosts.IsEmpty)
//         {

//             // current node is highest priority node in queue 
//             current = this.queueCellsCosts.GetHighestPriorityCell();

//             // has current node found it's target?
//             if (current.position == targetPosition)
//             {
//                 GD.Print("Path found");
//                 break;
//             }

//             var neighboringCells = this.queueCellsCosts.GetNeighbourCells(current, grid);


//             foreach (var neighbour in neighboringCells)
//             {
//                 // defines cost (cost acumulated + cost to neighbour cell)
//                 var newCost = costSoFar[current.position] + (int)current.position.DistanceTo(neighbour);


//                 // if pos hasn't been calculated before and cost more effective than before
//                 if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
//                 {
//                     costSoFar.Add(neighbour, newCost);

//                     //  defines its priority
//                     var priority = newCost + (int)targetPosition.DistanceTo(neighbour);

//                     //  put into queue
//                     this.queueCellsCosts.AddCellCost(neighbour, priority);

//                     //  define parent position
//                     if (!cameFromPositionStack.ContainsKey(neighbour))
//                         cameFromPositionStack.Add(neighbour, current);

//                     cameFromPositionStack[neighbour] = current;
//                 }
//             }
//         }
//         /*=============================   MAIN LOOP END =============================  */

//         //  if array doesn't have targetPosition, failed, return empty array
//         if (!cameFromPositionStack.ContainsKey(targetPosition))
//         {
//             GD.Print("failed");
//             return new Vector2[0];
//         }

//         /* =============================  Retrace Route and Return Best Path =============================   */

//         // path array is target position
//         lockedPath.Add(current.position);

//         // while current position isn't start position
//         while (current.position != startPosition)
//         {
//             //retrace last cell
//             current = cameFromPositionStack[current.position];

//             // insert at the start of the array
//             lockedPath.Insert(0, current.position);
//         }


//         // removes start_pos from path
//         lockedPath.RemoveAt(0);

//         return lockedPath.ToArray();
//     }

//     #endregion
// }
