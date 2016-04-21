using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.GLRendering
{
    public class RectangleRenderTask : ComplexRenderTask
    {
        public Color Color { get; private set; }
        public Vector2 Location { get; private set; }
        public Vector2 Size { get; private set; }

        public RectangleRenderTask(Color color, Vector2 location, Vector2 size, float distance = 0)
            : base(
                new LineRenderTask(color, location, location + size.VX(), distance),
                new LineRenderTask(color, location + size.VX(), location + size, distance),
                new LineRenderTask(color, location + size, location + size.VY(), distance),
                new LineRenderTask(color, location + size.VY(), location, distance))
        {
            Location = location;
            Size = size;
            Color = color;
        }
    }
}