using Godot;
using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinding
{
    public class Nav : TileMap
    {
        //this tilemap serves as a navigation tilemap.
        //all tiles are later appended to a dictionary where they can be referenced

        //Storing tile data on a dictionary empowers the data that can be efficiently
        //requested. EX: grid.has(position), grid[position], grid[postion][0], etc. 

        private List<int> walkableCells;
        // private int[,] grid;
        public PathfindingGrid Grid { get; private set; }
        private Vector2 currentTarget;
        // references
        Node2D player; //used as origin point 
        Draw drawNode;  // node that draws route and cursor
        Pathfinder pathFinder; // node that handles route generation

        CancellationTokenSource pathSearchCancellationTokenSource;

        public override void _Ready()
        {

            walkableCells = new List<int>() { 0 };
            //get used cells into an array (not real world pos)
            var _tiles = GetUsedCells();
            Grid = new PathfindingGrid(_tiles.Count);

            var tiles = new Vector2[_tiles.Count];
            for (int i = 0; i < _tiles.Count; i++)
            {
                tiles[i] = (Vector2)_tiles[i];
            }


            //GD.Print(tiles.Length);

            int minX = (int)tiles.Min(tl => tl.x);
            int maxX = (int)tiles.Max(tl => tl.x);
            int lengthX = maxX - minX;

            int minY = (int)tiles.Min(tl => tl.y);
            int maxY = (int)tiles.Max(tl => tl.y);
            int lengthY = maxY - minY;

            //get cell world pos, centralize it and append to grid array
            int gridIdx = 0;
            foreach (var item in tiles)
            {
                var tile = item;

                //get current cell index
                var idx = GetCell((int)tile.x, (int)tile.y);

                // if idx is not in walkable cell idx dictionary
                // skip to the next iteration
                if (!walkableCells.Contains(idx))
                    continue;

                Vector2 tgt = MapToWorld(tile, false);
                tgt = new Vector2(tgt.x, tgt.y + 15); //offset to centralize tile
                                                      // grid is dictionary, make data array for each cell
                Grid.SetCell(gridIdx, new GridData(tgt, GridStatusEnum.EMPTY, null));

                gridIdx++;
            }

            // define references with groups
            player = (Node2D)GetTree().GetNodesInGroup("player")[0];
            drawNode = (Draw)GetTree().GetNodesInGroup("draw")[0];
            pathFinder = (Pathfinder)GetTree().GetNodesInGroup("pathfinder")[0];


            // parse grid to be drawn
            drawNode.SetGrid(Grid);

            // cursor and player interactions
            SetProcess(true);

            // also cursor and player interactions
            SetProcessInput(true);
        }

        //  Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            // get map tile pos relative to mouse
            var targetCell = WorldToMap(GetGlobalMousePosition());


            // if targetCell is a valid cell (!= -1), sets it to currentTarget
            if (GetCell((int)targetCell.x, (int)targetCell.y) != -1)
            {
                // get world position and centralize offset tile 
                currentTarget = MapToWorld(targetCell) + new Vector2(0, 15);
            }
            else
            {
                // unable it
                currentTarget = new Vector2();
            }

            // parse cursor target to be drawn
            drawNode.SetCurrentTagert(currentTarget);
        }

        public override void _Input(InputEvent @event)
        {
            // teleport player
            if (@event.IsActionPressed("mouse_act_left"))
            {
                // if cursor cell is in the grid
                if (Grid.HasCell(currentTarget) && Grid.CellIsEmpty(currentTarget))
                {
                    // teleport the pawn and cleans drawn path
                    player.Position = currentTarget;
                    drawNode.ClearPath();
                }
            }

            // generate path
            if (@event.IsActionPressed("mouse_act_right"))
            {
                if (this.pathSearchCancellationTokenSource != null)
                    this.pathSearchCancellationTokenSource.Cancel();

                this.pathSearchCancellationTokenSource = new CancellationTokenSource();

                var cancellationToken = this.pathSearchCancellationTokenSource.Token;

                // if cursor cell is in the grid
                if (Grid.HasCell(currentTarget))
                {
                    Task.Run(async () =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            GD.Print("Task cancelled");
                            return;
                        }

                        var path = await pathFinder.SearchAsync(player.Position, currentTarget, cancellationToken);
                        await drawNode.SetPathAsync(path, cancellationToken);

                    }, cancellationToken);

                }
            }

            // TODO: Block / Unblock path
            //
        }
    }
}
