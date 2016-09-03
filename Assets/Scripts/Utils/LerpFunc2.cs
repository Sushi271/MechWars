using System;
using System.IO;
using System.Linq;

namespace MechWars.Utils
{
    public class LerpFunc2
    {
        float[] dim0;
        float[] dim1;
        float[,] values;

        public string Dim0Name { get; private set; }
        public string Dim1Name { get; private set; }

        public bool Invalid { get; private set; }

        public float Min0 { get { return dim0.First(); } }
        public float Max0 { get { return dim0.Last(); } }
        public float Min1 { get { return dim1.First(); } }
        public float Max1 { get { return dim1.Last(); } }

        public float this[float dim0Val, float dim1Val]
        {
            get
            {
                if (dim0Val < Min0) dim0Val = Min0;
                if (dim0Val > Max0) dim0Val = Max0;
                if (dim1Val < Min1) dim1Val = Min1;
                if (dim1Val > Max1) dim1Val = Max1;

                int i0 = 0;
                bool equal0 = false;
                if (dim0[i0] == dim0Val)
                    equal0 = true;
                else for (i0 = 1; i0 < dim0.Length; i0++)
                        if (dim0[i0] >= dim0Val)
                        {
                            equal0 = dim0[i0] == dim0Val;
                            break;
                        }

                int i1 = 0;
                bool equal1 = false;
                if (dim1[i1] == dim1Val)
                    equal1 = true;
                else for (i1 = 1; i1 < dim1.Length; i1++)
                        if (dim1[i1] >= dim1Val)
                        {
                            equal1 = dim1[i1] == dim1Val;
                            break;
                        }

                if (equal0)
                {
                    if (equal1) return values[i0, i1];

                    var dimA = dim1[i1 - 1];
                    var dimB = dim1[i1];
                    var valA = values[i0, i1 - 1];
                    var valB = values[i0, i1];

                    var lerpRatio = (dim1Val - dimA) / (dimB - dimA);
                    var lerp = valA + lerpRatio * (valB - valA);
                    return lerp;
                }
                else if (equal1)
                {
                    var dimA = dim0[i0 - 1];
                    var dimB = dim0[i0];
                    var valA = values[i0 - 1, i1];
                    var valB = values[i0, i1];

                    var lerpRatio = (dim0Val - dimA) / (dimB - dimA);
                    var lerp = valA + lerpRatio * (valB - valA);
                    return lerp;
                }

                var dimA0 = dim0[i0 - 1];
                var dimB0 = dim0[i0];
                var valAA0 = values[i0 - 1, i1 - 1];
                var valBA0 = values[i0, i1 - 1];
                var valAB0 = values[i0 - 1, i1];
                var valBB0 = values[i0, i1];

                var lerpRatio0 = (dim0Val - dimA0) / (dimB0 - dimA0);
                var lerpA = valAA0 + lerpRatio0 * (valBA0 - valAA0);
                var lerpB = valAB0 + lerpRatio0 * (valBB0 - valAB0);

                var dimA1 = dim1[i1 - 1];
                var dimB1 = dim1[i1];

                var lerpRatio1 = (dim1Val - dimA1) / (dimB1 - dimB1);
                var lerp01 = lerpA + lerpRatio1 * (lerpB - lerpA);
                return lerp01;
            }
        }

        public LerpFunc2(byte[] rawData)
        {
            var ms = new MemoryStream(rawData);
            var sr = new StreamReader(ms);

            try
            {
                var dim0Str = sr.ReadLine().Split(' ');
                Dim0Name = dim0Str[0];
                dim0 = new float[dim0Str.Length - 1];
                for (int i = 1; i < dim0Str.Length; i++)
                    dim0[i - 1] = float.Parse(dim0Str[i]);
                if (dim0.Length < 2) throw new Exception();

                var dim1Str = sr.ReadLine().Split(' ');
                Dim1Name = dim1Str[0];
                dim1 = new float[dim1Str.Length - 1];
                for (int i = 1; i < dim1Str.Length; i++)
                    dim1[i - 1] = float.Parse(dim1Str[i]);
                if (dim1.Length < 2) throw new Exception();

                values = new float[dim0.Length, dim1.Length];
                for (int i0 = 0; i0 < dim0.Length; i0++)
                {
                    var valuesStr = sr.ReadLine().Split(' ');
                    for (int i1 = 0; i1 < dim1.Length; i1++)
                        values[i0, i1] = float.Parse(valuesStr[i1]);
                }
            }
            catch
            {
                Invalid = true;
            }
            finally
            {
                sr.Close();
            }
        }
    }
}