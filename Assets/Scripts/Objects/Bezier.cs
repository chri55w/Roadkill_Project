using System;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public class Bezier
    {
        public Vector2 PointA;
        public Vector2 PointB;
        public Vector2 PointC;
        public Vector2 PointD;

        public Bezier(Vector2 p_PointA, Vector2 p_PointB, Vector2 p_PointC, Vector2 p_PointD)
        {
            PointA = p_PointA;
            PointB = p_PointB;
            PointC = p_PointC;
            PointD = p_PointD;
        }

        public Vector2 GetPoint(float p_CurvePosition)
        {
            float l_A1 = Mathf.Pow(1 - p_CurvePosition, 3) * PointA.y;
            float l_B1 = 3 * Mathf.Pow(1 - p_CurvePosition, 2) * p_CurvePosition * PointB.y;
            float l_C1 = 3 * (1 - p_CurvePosition) * Mathf.Pow(p_CurvePosition, 2) * PointC.y;
            float l_D1 = Mathf.Pow(p_CurvePosition, 3) * PointD.y;

            float l_A2 = Mathf.Pow(1 - p_CurvePosition, 3) * PointA.y;
            float l_B2 = 3 * Mathf.Pow(1 - p_CurvePosition, 2) * p_CurvePosition * PointB.y;
            float l_C2 = 3 * (1 - p_CurvePosition) * Mathf.Pow(p_CurvePosition, 2) * PointC.y;
            float l_D2 = Mathf.Pow(p_CurvePosition, 3) * PointD.y;

            return new Vector2((l_A1 + l_B1 + l_C1 + l_D1), (l_A2 + l_B2 + l_C2 + l_D2 ));
        }
    }
}