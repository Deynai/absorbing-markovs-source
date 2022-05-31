using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Deynai.Markov
{
    public class UIBezierCurve : Graphic
    {
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public Vector2 StartPos { get; set; }
        public Vector2 EndPos { get; set; }
        public Vector2 M1 { get; set; }
        public Vector2 M2 { get; set; }
        public float Radius { get; set; } = 10;
        public int Segments { get; set; } = 100;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = Color1;

            Vector2 segmentStart = BezierMaths.Cubic(StartPos, M1, M2, EndPos, 0);
            Vector2 normalStart = BezierMaths.CubicNormal(StartPos, M1, M2, EndPos, 0);
            vertex.position = segmentStart + normalStart * Radius;
            vh.AddVert(vertex);
            vertex.position = segmentStart - normalStart * Radius;
            vh.AddVert(vertex);

            for (int i = 0; i < Segments; i++)
            {
                float t = (i + 1) / (float)Segments;
                Vector2 segmentEnd = BezierMaths.Cubic(StartPos, M1, M2, EndPos, t);
                Vector2 normalEnd = BezierMaths.CubicNormal(StartPos, M1, M2, EndPos, t);
                vertex.color = Color.Lerp(Color1, Color2, t);

                vertex.position = segmentEnd + normalEnd * Radius;
                vh.AddVert(vertex);

                vertex.position = segmentEnd - normalEnd * Radius;
                vh.AddVert(vertex);

                int vi = i * 2;
                vh.AddTriangle(vi + 0, vi + 2, vi + 3);
                vh.AddTriangle(vi + 0, vi + 3, vi + 1);
            }
        }
    }
}
