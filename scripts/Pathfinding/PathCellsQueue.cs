using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Pathfinding
{
    public class PathCellsQueue
    {
         public List<PathCellCost> cellsCosts;

    public PathCellsQueue()
    {
        cellsCosts = new List<PathCellCost>();
    }

    public void AddPathCellCost(Vector2 position, int priority)
    {
        var PathCellCost = new PathCellCost(position, priority);
        cellsCosts.Add(PathCellCost);
    }

    // gets highest priority (most efficient) position in queue
    public PathCellCost GetHighestPriorityCell()
    {
        var prioritaryCell = this.cellsCosts.OrderBy(x => x.cost).First();
        this.cellsCosts.Remove(prioritaryCell);

        return prioritaryCell;
    }

    public void ClearQueue()
    {
        this.cellsCosts = new List<PathCellCost>();
    }


    public Vector2[] GetNeighbourCells(PathCellCost cell, PathfindingGrid grid)
    {
        // array to hold all possible neighbours of current 'pos'
        var neighboringCells = new List<Vector2>();
        // minimum distance between cells
        var next = new Vector2(30, 15);


        // vector directions
        var up = new Vector2(1, -1);
        var down = new Vector2(-1, 1);
        var right = new Vector2(1, 1);
        var left = new Vector2(-1, -1);

        //	diagonal vector directions (activate it if you want it)
        // var w = new Vector2(0,2); 
        // var d = new Vector2(2,0); 
        // var s = new Vector2(0,-2);
        // var a = new Vector2(-2,0);

        // array of possible neighbors, yet to be validated
        // only horizontal movement
        var check = new Vector2[4] { up, down, right, left };

        //with diagonal movement (activate it if you want)
        //	var check = new Vector2[8]{up,down,right,left,w,a,s,d}

        // if neighbour exists in grid and is "empty", append
        foreach (var neighbor in check)
        {
            // (direction * minimum_distance) + pos = relative_neighbour
            var _neighbor = neighbor * next + cell.position;


            // skip if cell is blocked
            if (grid.HasCell(_neighbor))
            {

                // for diagonals:
                // skip condition, off by default
                //var skipTile = false;

                if (grid.CellIsEmpty(_neighbor) /*&& !skipTile*/)
                {
                    neighboringCells.Add(_neighbor);
                }
            }
        }

        return neighboringCells.ToArray();
    }

    public bool IsEmpty => this.cellsCosts == null || this.cellsCosts.Count == 0;
    }
}