using System;
using System.Collections.Generic;
using UnityEngine;

using StaticObjects;

namespace Objects
{
    public class BezierSpline : MonoBehaviour
    {
        [SerializeField]
        private List<Vector3> Points = new List<Vector3>();
        [SerializeField]
        private List<BezierControlPointMode> Modes = new List<BezierControlPointMode>();
        [SerializeField]
        private List<float> Widths = new List<float>();
        [SerializeField]
        private List<float> TiltAngles = new List<float>();
        [SerializeField]
        private bool m_Loop;
        [SerializeField]
        private bool m_Locked;
        [SerializeField]
        public List<Vector2> MeshVertLayout = new List<Vector2>();
        [SerializeField]
        private int m_MeshDetailLevel = 150;
        [SerializeField]
        private float m_DefaultTrackWidth = 5f;
        [SerializeField]
        private bool m_DebugVertices = false;

        public bool Loop
        {
            get { return m_Loop; }
            set
            {
                m_Loop = value;
                if (value == true)
                {
                    Modes[Modes.Count - 1] = Modes[0];
                    Widths[Widths.Count - 1] = Widths[0];
                    TiltAngles[TiltAngles.Count - 1] = TiltAngles[0];
                    SetControlPoint(0, Points[0]);

                    DrawCurve();
                }
            }
        }

        public void Start()
        {
            Debug.Log(Points.Count);
        }

        public bool Locked
        {
            get { return m_Locked; }
            set
            {
                m_Locked = value;
                if (value == true)
                    DrawCurve();
            }
        }

        public int MeshDetailLevel
        {
            get { return m_MeshDetailLevel; }
            set
            {
                m_MeshDetailLevel = value;

                DrawCurve();
            }
        }

        public int CurveCount
        {
            get
            {
                return (Points.Count - 1) / 3;
            }
        }
        public int ControlPointCount
        {
            get
            {
                return Points.Count;
            }
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return Modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            Modes[modeIndex] = mode;

            if (Loop)
            {
                if (modeIndex == 0)
                    Modes[Modes.Count - 1] = mode;
                else if (modeIndex == Modes.Count - 1)
                    Modes[0] = mode;
            }

            EnforceMode(index);
            DrawCurve();
        }

        public float GetControlPointTrackWidth(int index)
        {
            return Widths[(index + 1) / 3];
        }

        public void SetControlPointTrackWidth(int index, float TrackWidth)
        {
            int trackWidthIndex = (index + 1) / 3;
            Widths[trackWidthIndex] = TrackWidth;

            if (Loop)
            {
                if (trackWidthIndex == 0)
                    Widths[Widths.Count - 1] = TrackWidth;
                else if (trackWidthIndex == Widths.Count - 1)
                    Widths[0] = TrackWidth;
            }

            EnforceMode(index);
            DrawCurve();
        }

        public float GetControlPointTrackTiltAngle(int index)
        {
            return TiltAngles[(index + 1) / 3];
        }

        public void SetControlPointTrackTiltAngle(int index, float TrackTiltAngle)
        {
            int TrackTiltAnglesIndex = (index + 1) / 3;
            TiltAngles[TrackTiltAnglesIndex] = TrackTiltAngle;

            if (Loop)
            {
                if (TrackTiltAnglesIndex == 0)
                    TiltAngles[TiltAngles.Count - 1] = TrackTiltAngle;
                else if (TrackTiltAnglesIndex == TiltAngles.Count - 1)
                    TiltAngles[0] = TrackTiltAngle;
            }

            EnforceMode(index);
            DrawCurve();
        }

        public Vector3 GetControlPoint(int index)
        {
            return Points[index];
        }

        internal void RecalculateTrackUVs()
        {
            DrawCurve();

            MeshFilter meshfilter = GetComponent<MeshFilter>();

            Mesh mesh = meshfilter.sharedMesh;

            List<Vector2> UVs = new List<Vector2>();

            float currentRoadUTexValue = 0.0f;
            for (int num = 0; num < mesh.vertexCount; num++)
            {
                Vector2 UV = new Vector2(currentRoadUTexValue, 1);

                UVs.Add(UV);

                // Uniform calculation of the texture coordinates for the roadway,
                // so it doesn't matter if there is a gap of 2 or 200 m
                currentRoadUTexValue += 200 *
                    (mesh.vertices[(num + 1) % mesh.vertices.Length] -
                    mesh.vertices[num % mesh.vertices.Length]).magnitude;
            }

            meshfilter.sharedMesh.SetUVs(0, UVs);

            /*

            List<Vector2> UVs = new List<Vector2>(UnityEditor.Unwrapping.GeneratePerTriangleUV(GetComponent<MeshFilter>().sharedMesh));

            Debug.Log(mesh.sharedMesh.vertexCount);
            Debug.Log(UVs.Count);

            GetComponent<MeshFilter>().sharedMesh.SetUVs(0, UVs);

    */
        }

        internal void RecalculateTrackNormals()
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

            mesh.RecalculateNormals();

            DrawCurve();
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - Points[index];
                if (Loop)
                {
                    if (index == 0)
                    {
                        Points[1] += delta;
                        Points[Points.Count - 2] += delta;
                        Points[Points.Count - 1] = point;
                    }
                    else if (index == Points.Count - 1)
                    {
                        Points[0] = point;
                        Points[1] += delta;
                        Points[index - 1] += delta;
                    }
                    else
                    {
                        Points[index - 1] += delta;
                        Points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                        Points[index - 1] += delta;

                    if (index + 1 < Points.Count)
                        Points[index + 1] += delta;

                }
            }
            Points[index] = point;
            EnforceMode(index);
            DrawCurve();
        }

        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = Points.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(StaticObjects.Bezier.GetPoint(Points[i], Points[i + 1], Points[i + 2], Points[i + 3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = Points.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(StaticObjects.Bezier.GetFirstDerivative(Points[i], Points[i + 1], Points[i + 2], Points[i + 3], t)) - transform.position;
        }

        public Vector3 GetVelocityAtControlPoint(int index)
        {
            int l_Index = (index + 1) / 3;
            float t = ((float)l_Index / (float)CurveCount);

            return GetVelocity(t);
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public Vector3 GetDirectionAtControlPoint(int index)
        {
            int l_Index = (index + 1) / 3;
            float t = ((float)l_Index / (float)CurveCount);

            return GetDirection(t);
        }

        public Quaternion GetRotation(float t)
        {
            Vector3 direction = GetDirection(t);

            t = Mathf.Clamp01(t) * CurveCount;

            int TrackTiltAngleIndex = (int)Mathf.Floor(t);

            TrackTiltAngleIndex = Mathf.Clamp(TrackTiltAngleIndex, 0, CurveCount);

            t = t % 1;

            float TiltAngle = 0f;

            if (TrackTiltAngleIndex + 1 <= TiltAngles.Count - 1)
                TiltAngle = Mathf.LerpAngle(TiltAngles[TrackTiltAngleIndex], TiltAngles[TrackTiltAngleIndex + 1], t);
            else
                TiltAngle = TiltAngles[TrackTiltAngleIndex];

            Quaternion Tilt = Quaternion.AngleAxis(TiltAngle, direction);

            return Quaternion.LookRotation(direction, Tilt * Vector3.up);
        }

        public Quaternion GetRotationAtControlPoint(int index)
        {
            int l_Index = (index + 1) / 3;
            float t = ((float)l_Index / (float)CurveCount);

            Vector3 direction = GetDirection(t);

            t = Mathf.Clamp01(t) * CurveCount;

            int TrackTiltAngleIndex = (int)Mathf.Floor(t);

            TrackTiltAngleIndex = Mathf.Clamp(TrackTiltAngleIndex, 0, CurveCount);

            t = t % 1;

            float TiltAngle = 0f;

            if (TrackTiltAngleIndex + 1 <= TiltAngles.Count - 1)
                TiltAngle = Mathf.LerpAngle(TiltAngles[TrackTiltAngleIndex], TiltAngles[TrackTiltAngleIndex + 1], t);
            else
                TiltAngle = TiltAngles[TrackTiltAngleIndex];

            Quaternion Tilt = Quaternion.AngleAxis(TiltAngle, direction);

            return Quaternion.LookRotation(direction, Tilt * Vector3.up);
        }

        public float GetTrackWidth(float t)
        {
            t = Mathf.Clamp01(t) * CurveCount;

            int TrackWidthIndex = (int)Mathf.Floor(t);

            TrackWidthIndex = Mathf.Clamp(TrackWidthIndex, 0, CurveCount);

            t = t % 1;

            if (TrackWidthIndex + 1 <= Widths.Count - 1)
                return Mathf.Lerp(Widths[TrackWidthIndex], Widths[TrackWidthIndex + 1], t);
            else
                return Widths[TrackWidthIndex];
        }

        internal void ResetTrackTiltAngles()
        {
            TiltAngles.Clear();

            for (int i = 0; i <= CurveCount; i++)
                TiltAngles.Add(0f);

            DrawCurve();
        }

        internal void ResetTrackWidths()
        {
            Widths.Clear();

            for (int i = 0; i <= CurveCount; i++)
                Widths.Add(m_DefaultTrackWidth);

            DrawCurve();
        }

        public void AddPointBeforeSelectedPoint(int SelectedIndex)
        {
            Vector3 SelectedPoint = Points[SelectedIndex];

            Vector3 DirectionAtPoint = GetDirectionAtControlPoint(SelectedIndex);

            Points.Insert(SelectedIndex - 1, SelectedPoint + (-DirectionAtPoint * 4));
            Points.Insert(SelectedIndex - 1, SelectedPoint + (-DirectionAtPoint * 8));
            Points.Insert(SelectedIndex - 1, SelectedPoint + (-DirectionAtPoint * 12));

            int CurveIndex = (SelectedIndex / 3);

            Widths.Insert(CurveIndex, m_DefaultTrackWidth);

            TiltAngles.Insert(CurveIndex, 0f);

            Modes.Insert(CurveIndex, BezierControlPointMode.Free);

            EnforceMode(Points.Count - 4);

            if (Loop)
            {
                Points[Points.Count - 1] = Points[0];
                Modes[Modes.Count - 1] = Modes[0];
                Widths[Widths.Count - 1] = Widths[0];
                TiltAngles[TiltAngles.Count - 1] = TiltAngles[0];
                EnforceMode(0);
            }

            DrawCurve();
        }
        public void AddPointAfterSelectedPoint(int SelectedIndex)
        {
            Vector3 SelectedPoint = Points[SelectedIndex];

            Vector3 DirectionAtPoint = GetDirectionAtControlPoint(SelectedIndex);

            Points.Insert(SelectedIndex + 2, SelectedPoint + (DirectionAtPoint * 12));
            Points.Insert(SelectedIndex + 2, SelectedPoint + (DirectionAtPoint * 8));
            Points.Insert(SelectedIndex + 2, SelectedPoint + (DirectionAtPoint * 4));

            int CurveIndex = SelectedIndex / 3 + 1;

            Widths.Insert(CurveIndex, m_DefaultTrackWidth);

            TiltAngles.Insert(CurveIndex, 0f);

            Modes.Insert(CurveIndex, BezierControlPointMode.Free);

            EnforceMode(Points.Count - 4);

            if (Loop)
            {
                Points[Points.Count - 1] = Points[0];
                Modes[Modes.Count - 1] = Modes[0];
                Widths[Widths.Count - 1] = Widths[0];
                TiltAngles[TiltAngles.Count - 1] = TiltAngles[0];
                EnforceMode(0);
            }

            DrawCurve();
        }

        public void Reset()
        {
            Points.Clear();

            Points.Add(new Vector3(0f, 0f, 0f));
            Points.Add(new Vector3(1f, 0f, 0f));
            Points.Add(new Vector3(2f, 0f, 0f));
            Points.Add(new Vector3(3f, 0f, 0f));

            Modes.Clear();
            Modes.Add(BezierControlPointMode.Free);
            Modes.Add(BezierControlPointMode.Free);

            Widths.Clear();
            Widths.Add(m_DefaultTrackWidth);
            Widths.Add(m_DefaultTrackWidth);

            TiltAngles.Clear();
            TiltAngles.Add(0f);
            TiltAngles.Add(0f);

            DrawCurve();
        }

        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = Modes[modeIndex];

            if (mode == BezierControlPointMode.Free || !Loop && (modeIndex == 0 || modeIndex == Modes.Count - 1))
                return;

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                    fixedIndex = Points.Count - 2;

                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= Points.Count)
                    enforcedIndex = 1;
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= Points.Count)
                    fixedIndex = 1;

                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                    enforcedIndex = Points.Count - 2;
            }

            Vector3 middle = Points[middleIndex];
            Vector3 enforcedTangent = middle - Points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, Points[enforcedIndex]);

            Points[enforcedIndex] = middle + enforcedTangent;

            DrawCurve();
        }

        public void DrawCurve()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            Vector3 Position = GetPoint(0f);
            Quaternion Rotation = GetRotation(0f);
            Vector3 right = Rotation * Vector3.right;
            Vector3 up = Rotation * Vector3.up;

            float trackWidth = GetTrackWidth(0f);
            
            foreach (Vector2 l_Point in MeshVertLayout)
            {
                vertices.Add(Position + (right * (trackWidth * l_Point.x)) + (up * l_Point.y));
                normals.Add(up);
            }

            int triIndex = 0;

            for (int l_Index = 0; l_Index <= MeshDetailLevel; l_Index++)
            {
                float t = (float)l_Index / MeshDetailLevel;

                Position = GetPoint(t);
                Rotation = GetRotation(t);
                trackWidth = GetTrackWidth(t);

                right = Rotation * Vector3.right;
                up = Rotation * Vector3.up;

                foreach (Vector2 l_Point in MeshVertLayout)
                {
                    vertices.Add(Position + (right * (trackWidth * l_Point.x)) + (up * l_Point.y));
                    normals.Add(up);

                }

                for (int l_Index2 = 0; l_Index2 < MeshVertLayout.Count - 1; l_Index2++)
                {
                    triangles.Add(l_Index2 + triIndex);
                    triangles.Add(l_Index2 + 1 + triIndex);
                    triangles.Add(l_Index2 + MeshVertLayout.Count + triIndex);

                    triangles.Add(l_Index2 + MeshVertLayout.Count + triIndex);
                    triangles.Add(l_Index2 + 1 + triIndex);
                    triangles.Add(l_Index2 + MeshVertLayout.Count + triIndex + 1);
                }

                triIndex += MeshVertLayout.Count;
            }
            
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var transform = this.transform;

            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

            if (mesh != null && m_DebugVertices == true)
                foreach(Vector3 vert in mesh.vertices)
                    Gizmos.DrawWireSphere(vert, 0.5f);
                
        }

    }
}