using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.GLRendering
{
    public class RectangleRenderTask : ComplexRenderTask
    {
        public Vector2 Location { get; private set; }
        public Vector2 Size { get; private set; }
        public Color Color { get; private set; }

        public RectangleRenderTask(Vector2 location, Vector2 size, Color color)
            : base(
                new LineRenderTask(location, location + size.VX(), color),
                new LineRenderTask(location + size.VX(), location + size, color),
                new LineRenderTask(location + size, location + size.VY(), color),
                new LineRenderTask(location + size.VY(), location, color))
        {
            Location = location;
            Size = size;
            Color = color;
        }
    }
}