using UnityEngine;

namespace MechWars.GLRendering
{
    public class ComplexRenderTask : IRenderTask
    {
        IRenderTask[] subTasks;

        public ComplexRenderTask(params IRenderTask[] subTasks)
        {
            this.subTasks = subTasks;
        }

        public void Render()
        {
            foreach (var subTask in subTasks)
            {
                subTask.Render();
            }
        }
    }
}