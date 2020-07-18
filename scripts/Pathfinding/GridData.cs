using Godot;

namespace Pathfinding
{

    public struct GridData
    {
        public Vector2 position;
        public GridStatusEnum status;
        public Sprite sprite;

        public GridData(Vector2 position, GridStatusEnum status, Sprite sprite)
        {
            this.position = position;
            this.status = status;
            this.sprite = sprite;
        }
    }
}
