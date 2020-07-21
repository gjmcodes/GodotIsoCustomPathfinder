using Godot;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinding
{
    public class Pathfinder : Node
    {


        // This node generates a path between origin and target based on parsed
        // grid, start_point and target_point.

        // It is really powerful as it can be changed to adapt to specific situations
        // and favor certain routes above other possible ones if 'COST' is calculated
        // differently, and so on.


        // get grid reference
        PathfindingGrid grid;
        PathCellsQueue pathCellsQueue;
        //var queue_list = { } #Queue used in the A* search algorithm
        List<Vector2> lockedPath;

        public override void _Ready()
        {
            pathCellsQueue = new PathCellsQueue();
            var navNode = GetTree().GetNodesInGroup("nav")[0];
            grid = (navNode as Nav).Grid;
        }


        void ClearLockedPath()
        {
            lockedPath = new List<Vector2>();
        }

        #region  A* Search
        public async Task<Vector2[]> SearchAsync(Vector2 startPosition, Vector2 targetPosition, CancellationToken cancellationToken)
        {

            if (cancellationToken.IsCancellationRequested)
            {
                GD.Print("Task cancelled");
                return new Vector2[0];
            }

            var searchTask = await Task.Run(() =>
            {
                // if start_pos == target_pos, return empty array
                if (startPosition == targetPosition)
                    return new Vector2[0];

                // resets path A* 
                ClearLockedPath();
                this.pathCellsQueue.ClearQueue();

                // set initial considerations
                this.pathCellsQueue.AddPathCellCost(startPosition, 0);

                PathCellCost current = new PathCellCost(new Vector2(0, 0), 0);
                var costSoFar = new Dictionary<Vector2, int>();
                costSoFar.Add(startPosition, 0);
                Dictionary<Vector2, PathCellCost> cameFromPositionStack = new Dictionary<Vector2, PathCellCost>();

                /*=============================   MAIN LOOP =============================  */

                if (cancellationToken.IsCancellationRequested)
                {
                    GD.Print("Task cancelled");
                    return new Vector2[0];
                }

                // while there are pos in queue list
                while (!this.pathCellsQueue.IsEmpty)
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        GD.Print("Task cancelled");
                        return new Vector2[0];
                    }


                    // current node is highest priority node in queue 
                    current = this.pathCellsQueue.GetHighestPriorityCell();

                    // has current node found it's target?
                    if (current.position == targetPosition)
                    {
                        GD.Print("Path found");
                        break;
                    }

                    var neighboringCells = this.pathCellsQueue.GetNeighbourCells(current, grid);


                    foreach (var neighbour in neighboringCells)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            GD.Print("Task cancelled");
                            return new Vector2[0];
                        }

                        // defines cost (cost acumulated + cost to neighbour cell)
                        var newCost = PathfinderCost.CostToNeighbourCell(costSoFar, current.position, neighbour);

                        // if pos hasn't been calculated before and cost more effective than before
                        if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                        {
                            costSoFar.Add(neighbour, newCost);

                            //  defines its priority
                            var priority = PathfinderPriority.CalculatePriority(newCost, targetPosition, neighbour);

                            //  put into queue
                            this.pathCellsQueue.AddPathCellCost(neighbour, priority);

                            //  define parent position
                            if (!cameFromPositionStack.ContainsKey(neighbour))
                                cameFromPositionStack.Add(neighbour, current);

                            cameFromPositionStack[neighbour] = current;
                        }
                    }
                }
                /*=============================   MAIN LOOP END =============================  */

                //  if array doesn't have targetPosition, failed, return empty array
                if (!cameFromPositionStack.ContainsKey(targetPosition))
                {
                    GD.Print("failed");
                    return new Vector2[0];
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    GD.Print("Task cancelled");
                    return new Vector2[0];
                }

                /* =============================  Retrace Route and Return Best Path =============================   */

                // path array is target position
                lockedPath.Add(current.position);

                // while current position isn't start position
                while (current.position != startPosition)
                {
                    //retrace last cell
                    current = cameFromPositionStack[current.position];

                    // insert at the start of the array
                    lockedPath.Insert(0, current.position);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        GD.Print("Task cancelled");
                        return new Vector2[0];
                    }
                }


                // removes start_pos from path
                lockedPath.RemoveAt(0);

                return lockedPath.ToArray();
            });

            return searchTask;
        }

        #endregion
    }
}
