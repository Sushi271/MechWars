using System.Collections.Generic;
using UnityEngine;

namespace MechWars.GLRendering
{
    public class GLRenderer : MonoBehaviour
    {
        Material glMaterial;

        Queue<IRenderTask> tasks = new Queue<IRenderTask>();

        void Awake()
        {
            glMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
            glMaterial.hideFlags = HideFlags.HideAndDontSave;
            glMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }

        public void Schedule(IRenderTask task)
        {
            tasks.Enqueue(task);
        }

        void OnPostRender()
        {
            glMaterial.SetPass(0);

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            while (tasks.Count > 0)
                tasks.Dequeue().Render();

            GL.PopMatrix();
        }
    }
}