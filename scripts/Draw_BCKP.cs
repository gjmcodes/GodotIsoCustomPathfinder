// using Godot;
// using System;

// public class Draw : Node2D
// {
//     // drawing process that illustrates the path and cursor
//     private Vector2 currentTarget;
//     private Grid grid;
//     private Vector2[] path;

//     public void SetCurrentTagert(Vector2 target)
//     {
//         this.currentTarget = target;
//     }

//     public void SetGrid(Grid grid)
//     {
//         this.grid = grid;
//     }

//     public void SetPath(Vector2[] path)
//     {
//         this.path = path;
//     }

//     public void ClearPath()
//     {
//         if (this.path != null)
//             Array.Clear(this.path, 0, this.path.Length);
//     }

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         // create a instruction label
//         var label = new Label();
//         label.Text = "Mouse Middle Btn: Block/Unblock Cell \nMouse R Btn: Generate Path \nMouse L Btn: Teleport";

//         label.RectPosition = new Vector2(-5, -45);

//         // camera zoom is doubled, correct scale 
//         label.RectScale = new Vector2(.5f, .5f);
//         GetViewport().CallDeferred("add_child", label);

//         SetPhysicsProcess(true);
//     }

//     public override void _PhysicsProcess(float delta)
//     {

//         // draw each physics frame
//         Update();
//     }

//     public override void _Draw()
//     {
//         var color = new Color(.5f, 0, .5f);
//         var line = 6;

//         //cell size
//         var p = new Vector2(30, 15);

//         // drawn cursor cell selector if avaliable
//         if (grid.HasCell(currentTarget))
//         {
//             // circle square
//             DrawLine(currentTarget + new Vector2(0, -p.y), currentTarget + new Vector2(-p.x, 0), color, line);
//             DrawLine(currentTarget + new Vector2(0, -p.y), currentTarget + new Vector2(p.x, 0), color, line);
//             DrawLine(currentTarget + new Vector2(0, p.y), currentTarget + new Vector2(-p.x, 0), color, line);
//             DrawLine(currentTarget + new Vector2(0, p.y), currentTarget + new Vector2(p.x, 0), color, line);
//         }



//         // draw route
//         if (path?.Length > 0)
//         {
//             foreach (var cell in path)
//             {
//                 DrawCircle(cell, line, color);
//             }
//         }
//     }
// }
