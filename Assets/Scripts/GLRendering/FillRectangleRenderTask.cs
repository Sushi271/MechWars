using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.GLRendering
{
    public class FillRectangleRenderTask : IRenderTask
    {
        public Color Color { get; private set; }
        public Vector2 Location { get; private set; }
        public Vector2 Size { get; private set; }
        public float Distance { get; private set; }

        public FillRectangleRenderTask(Color color, Vector2 location, Vector2 size, float distance = 0.9f)
        {
            Location = location;
            Size = size;
            Color = color;
        }

        public void Render()
        {
            Vector2 v00 = Location;
            Vector2 v10 = Location + Size.VX();
            Vector2 v11 = Location + Size;
            Vector2 v01 = Location + Size.VY();

            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(Color);
            GL.Vertex3(v00.x, v00.y, Distance);
            GL.Vertex3(v10.x, v10.y, Distance);
            GL.Vertex3(v01.x, v01.y, Distance);
            GL.Vertex3(v11.x, v11.y, Distance);
            GL.End();
        }
    }
}