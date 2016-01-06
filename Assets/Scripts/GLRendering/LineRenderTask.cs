using UnityEngine;

namespace MechWars.GLRendering
{
    public class LineRenderTask : IRenderTask
    {
        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }
        public Color Color { get; private set; }

        public LineRenderTask(Vector2 start, Vector2 end, Color color)
        {
            Start = start;
            End = end;
            Color = color;
        }

        public void Render()
        {
            GL.Begin(GL.LINES);
            GL.Color(Color);
            GL.Vertex(Start);
            GL.Vertex(End);
            GL.End();
        }
    }
}