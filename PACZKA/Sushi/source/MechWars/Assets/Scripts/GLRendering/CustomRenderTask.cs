using UnityEngine;

namespace MechWars.GLRendering
{
    public class CustomRenderTask : IRenderTask
    {
        System.Action task;

        public CustomRenderTask(System.Action task)
        {
            this.task = task;
        }

        public void Render()
        {
            task();
        }
    }
}