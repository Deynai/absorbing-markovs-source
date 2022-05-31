

using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
    public static class BezierMaths
    {
        public static List<float> reusableList = new List<float>();

        public static Vector2 Cubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 q1 = (1 - t) * p0 + t * p1;
            Vector2 q2 = (1 - t) * p1 + t * p2;
            Vector2 q3 = (1 - t) * p2 + t * p3;

            Vector2 q4 = (1 - t) * q1 + t * q2;
            Vector2 q5 = (1 - t) * q2 + t * q3;

            Vector2 b = (1 - t) * q4 + t * q5;
            return b;
        }

        public static Vector2 CubicTangent(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 dbdt = (-3 + 6*t - 3 * t * t) * p0 + (3 - 12 * t + 9 * t * t) * p1 + (6 * t - 9 * t * t) * p2 + (3 * t * t) * p3;
            return dbdt.normalized;
        }

        public static Vector2 CubicNormal(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 tangent = CubicTangent(p0, p1, p2, p3, t);
            return new Vector2(-tangent.y, tangent.x);
        }

        public static Vector2 CubicDistance(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, int samples)
        {
            t = Mathf.Clamp01(t);
            if (t == 0) return Cubic(p0, p1, p2, p3, 0);
            if (t == 1) return Cubic(p0, p1, p2, p3, 1);

            reusableList.Clear();
            float diff = 1 / (float) samples;
            Vector2 v1 = Cubic(p0, p1, p2, p3, 0);
            float totalDistance = 0;

            for (int i = 1; i <= samples; i++)
            {
                Vector2 v2 = Cubic(p0, p1, p2, p3, i * diff);
                totalDistance += Vector2.Distance(v1, v2);
                reusableList.Add(totalDistance);
                v1 = v2;
            }

            float newT = 0;
            for (int i = 1; i < samples; i++)
            {
                if (reusableList[i] / totalDistance > t)
                {
                    float d1 = reusableList[i - 1] / totalDistance;
                    float d2 = reusableList[i] / totalDistance;
                    newT = ((i - 1) + (t - d1) / (d2 - d1))/samples;
                    break;
                }
            }

            return Cubic(p0, p1, p2, p3, newT);
        }

        public static Vector2 CubicDistanceCapped(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, int samples, float cap)
        {
            t = Mathf.Clamp01(t);
            if (t == 0) return Cubic(p0, p1, p2, p3, 0);
            if (t == 1) return Cubic(p0, p1, p2, p3, 1);

            reusableList.Clear();
            float diff = 1 / (float)samples;
            Vector2 v1 = Cubic(p0, p1, p2, p3, 0);
            float totalDistance = 0;

            for (int i = 1; i <= samples; i++)
            {
                Vector2 v2 = Cubic(p0, p1, p2, p3, i * diff);
                totalDistance += Vector2.Distance(v1, v2);
                reusableList.Add(totalDistance);
                v1 = v2;
            }

            float newT = 0;
            for (int i = 1; i < samples; i++)
            {
                //if (reusableList[i] / totalDistance > t)
                //{
                //    float d1 = reusableList[i - 1] / totalDistance;
                //    float d2 = reusableList[i] / totalDistance;
                //    newT = ((i - 1) + (t - d1) / (d2 - d1)) / samples;
                //    break;
                //}

                if (reusableList[i] > cap)
                {
                    float d1 = reusableList[i - 1];
                    float d2 = reusableList[i];
                    newT = (d1 + (cap - d1) / (d2 - d1))/totalDistance;
                    break;
                }
            }

            return Cubic(p0, p1, p2, p3, newT);
        }

        //public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        //{
        //    Vector2 q1 = (1 - t) * p0 + t * p1;
        //    Vector2 q2 = (1 - t) * p1 + t * p2;
        //    Vector2 b = (1 - t) * q1 + t * q2;
        //    return b;
        //}
        //public static Vector2 QuadraticBezierTangent(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        //{
        //    return (2 * t - 2) * p0 + (2 - 4 * t) * p1 + (2 * t) * p2;
        //}

        //public static Vector2 QuadraticBezierNormal(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        //{
        //    Vector2 tangent = QuadraticBezierTangent(p0, p1, p2, t);
        //    return new Vector2(-tangent.y, tangent.x).normalized;
        //}
    }
}
