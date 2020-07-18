using Godot;

namespace Pathfinding
{
    public struct PathCellCost
    {
        public Vector2 position;
        public int cost;

        public PathCellCost(Vector2 position, int cost)
        {
            this.position = position;
            this.cost = cost;
        }
    }
}