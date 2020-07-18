using Godot;
using System;
using System.Linq;

namespace Pathfinding
{
    public class PathfindingGrid
    {
        public GridData[] data;

        public PathfindingGrid(int length)
        {
            data = new GridData[length];
        }

        public void SetCell(int index, GridData data)
        {
            this.data[index] = data;
        }

        public bool HasCell(Vector2 cell)
        {
            return data.Select(x => x.position).Contains(cell);
        }

        public bool CellIsEmpty(Vector2 cell)
        {
            var gridCell = data.First(x => x.position == cell);
            return gridCell.status == GridStatusEnum.EMPTY;
        }
    }
}
