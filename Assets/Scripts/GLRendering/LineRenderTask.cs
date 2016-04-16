using UnityEngine;

namespace MechWars.GLRendering
{
    public class LineRenderTask : IRenderTask
    {
        public Color Color { get; private set; }
        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }
        public float Distance { get; private set; }

        public LineRenderTask(Color color, Vector2 start, Vector2 end, float distance = 0.9f)
        {
            Color = color;
            Start = start;
            End = end;
            Distance = distance;
        }

        public void Render()
        {
            GL.Begin(GL.LINES);
            GL.Color(Color);
            GL.Vertex3(Start.x, Start.y, Distance);
            GL.Vertex3(End.x, End.y, Distance);
            GL.End();
        }
    }
}