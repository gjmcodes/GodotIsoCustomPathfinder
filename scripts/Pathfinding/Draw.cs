using Godot;
using Pathfinding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinding
{

    public class Draw : Node2D
    {

        [Export]
        int drawPathTime;


        // drawing process that illustrates the Path and cursor
        private Vector2 currentTarget;
        private PathfindingGrid grid;
        public List<Vector2> Path { get; private set; }

        public void SetCurrentTagert(Vector2 target)
        {
            this.currentTarget = target;
        }

        public void SetGrid(PathfindingGrid grid)
        {
            this.grid = grid;
        }

        public async Task SetPathAsync(Vector2[] path, CancellationToken cancellationToken)
        {
            ClearPath();

            foreach (var node in path)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    GD.Print("Task cancelled");
                    return;
                }

                await Task.Delay(drawPathTime);
                this.Path.Add(node);
            }
        }

        public void ClearPath()
        {
            if (this.Path != null)
                this.Path.Clear();
            else
                this.Path = new List<Vector2>();
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            // create a instruction label
            var label = new Label();
            label.Text = "Mouse Middle Btn: Block/Unblock Cell \nMouse R Btn: Generate Path \nMouse L Btn: Teleport";

            label.RectPosition = new Vector2(-5, -45);

            // camera zoom is doubled, correct scale 
            label.RectScale = new Vector2(.5f, .5f);
            GetViewport().CallDeferred("add_child", label);

            SetPhysicsProcess(true);
        }

        public override void _PhysicsProcess(float delta)
        {

            // draw each physics frame
            Update();
        }

        public override void _Draw()
        {
            var color = new Color(.5f, 0, .5f);
            var line = 6;

            //cell size
            var p = new Vector2(30, 15);

            // drawn cursor cell selector if avaliable
            if (grid.HasCell(currentTarget))
            {
                // circle square
                DrawLine(currentTarget + new Vector2(0, -p.y), currentTarget + new Vector2(-p.x, 0), color, line);
                DrawLine(currentTarget + new Vector2(0, -p.y), currentTarget + new Vector2(p.x, 0), color, line);
                DrawLine(currentTarget + new Vector2(0, p.y), currentTarget + new Vector2(-p.x, 0), color, line);
                DrawLine(currentTarget + new Vector2(0, p.y), currentTarget + new Vector2(p.x, 0), color, line);
            }



            // draw route
            if (Path?.Count > 0)
            {
                foreach (var cell in Path)
                {
                    DrawCircle(cell, line, color);
                }
            }
        }
    }
}

