using System.Collections.Generic;
using Godot;

namespace Pathfinding
{
    public struct PathfinderCost
    {
        public static int CostToNeighbourCell(Dictionary<Vector2, int> acumulatedCost, Vector2 currentCellPosition, Vector2 neighbour)
        {
            var newCost = acumulatedCost[currentCellPosition] + (int)currentCellPosition.DistanceTo(neighbour);

            return newCost;
        }
    }
}