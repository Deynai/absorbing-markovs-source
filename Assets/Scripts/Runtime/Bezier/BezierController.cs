using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
    public class BezierController : MonoBehaviour
    {
        [SerializeField] private UIBezierCurve bezier;
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;

        private Vector2 m1offset = new Vector2(400, 0);
        private Vector2 m2offset = new Vector2(-400, 0);

        private void Awake()
        {
            bezier.Color1 = color1;
            bezier.Color2 = color2;
        }

        public void UpdateBezier(Vector2 start, Vector2 end)
        {
            bezier.StartPos = start;
            bezier.EndPos = end;
            bezier.M1 = start + m1offset;
            bezier.M2 = end + m2offset; 
            bezier.SetVerticesDirty();
        }

        public void SetM1Offset(Vector2 offset)
        {
            m1offset = offset;
        }

        public void SetM2Offset(Vector2 offset)
        {
            m2offset = offset;
        }

        public void SetThickness(float thickness)
        {
            bezier.Radius = thickness;
        }

        public Vector2 EvaluateBezier(float t)
        {
            return BezierMaths.Cubic(bezier.StartPos, bezier.M1, bezier.M2, bezier.EndPos, t);
        }

        public Vector2 EvaluateBezierDistance(float t)
        {
            return BezierMaths.CubicDistance(bezier.StartPos, bezier.M1, bezier.M2, bezier.EndPos, t, 20);
        }
    }
}
