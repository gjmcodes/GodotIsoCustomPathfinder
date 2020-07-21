using Godot;

namespace Pathfinding
{
    public struct PathfinderPriority
    {
        public static int CalculatePriority(int costCurrentToNeighbour, Vector2 targetPosition, Vector2 neighbour)
        {
            var priority = costCurrentToNeighbour + (int)targetPosition.DistanceTo(neighbour);

            return priority;
        }
    }
}