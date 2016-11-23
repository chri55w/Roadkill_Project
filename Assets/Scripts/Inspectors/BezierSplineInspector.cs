using UnityEditor;
using UnityEngine;

using Objects;

namespace Inspectors
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : Editor
    {
        private const int stepsPerCurve = 10;
        private const float directionScale = 0.5f;

        private BezierSpline spline;
        private Transform handleTransform;
        private Quaternion handleRotation;

        private const float handleSize = 0.04f;
        private const float pickSize = 0.04f;

        private int selectedIndex = -1;

        private static Color[] modeColors = {
                                                new Color(0.90625f, 0.4921875f, 0.015625f),     // Ecstasy
                                                new Color(0.14453125f, 0.453125f, 0.66045625f), // Seance
                                                new Color(0.6015625f, 0.0703125f, 0.69921875f)  // Jelly Bean
                                            };

        bool ShowMeshProperties = false;
        bool ShowSplineProperties = false;
        bool ShowSelectedPointProperties = false;

        private void OnSceneGUI()
        {
            if (spline == null || spline.Locked)
                return;

            spline = target as BezierSpline;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
            for (int i = 1; i < spline.ControlPointCount; i += 3)
            {
                Vector3 p1 = ShowPoint(i);
                Vector3 p2 = ShowPoint(i + 1);
                Vector3 p3 = ShowPoint(i + 2);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
            Quaternion rotation = spline.GetRotationAtControlPoint(index);
            Vector3 direction = spline.GetDirectionAtControlPoint(index);

            float l_HandleSize = HandleUtility.GetHandleSize(point);

            float l_PickSize = pickSize * HandleUtility.GetHandleSize(point);
            float l_DiscHandleSize = handleSize * HandleUtility.GetHandleSize(point) * 25;

            bool l_IsHandle = (index % 3 != 0);

            Handles.DrawCapFunction HandleCap = Handles.DotCap;

            if (index == 0)
            {
                HandleCap = Handles.SphereCap;
                l_HandleSize = (5 * l_HandleSize) * handleSize;
            }
            else if (index == spline.ControlPointCount - 1)
                l_HandleSize = (2f * l_HandleSize) * handleSize;
            else if (!l_IsHandle)
                l_HandleSize = (1.5f * l_HandleSize) * handleSize;
            else
                l_HandleSize = l_HandleSize * handleSize;

            Handles.color = modeColors[(int)spline.GetControlPointMode(index)];

            if (Handles.Button(point, handleRotation, l_HandleSize, l_PickSize, HandleCap))
            {
                selectedIndex = index;
                Repaint();
            }
            if (selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);

                if (!l_IsHandle)
                    rotation = Handles.Disc(rotation, point, direction, l_DiscHandleSize, false, 0);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move / Tilt Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                    spline.SetControlPointTrackTiltAngle(index, rotation.eulerAngles.z);
                }
            }

            return point;
        }

        public override void OnInspectorGUI()
        {
            spline = target as BezierSpline;

            EditorGUI.BeginChangeCheck();
            bool Locked = EditorGUILayout.Toggle("Lock Spline", spline.Locked);
            if (EditorGUI.EndChangeCheck())
            {
                if ((spline.Locked = true && Locked == false))
                {
                    if (EditorUtility.DisplayDialog("Are You Sure?", "Unlocking the spline allows the mesh to be completely recreated.", "Confirm"))
                    {
                        EditorUtility.SetDirty(spline);
                        spline.Locked = Locked;
                    }
                }
                else
                {
                    EditorUtility.SetDirty(spline);
                    spline.Locked = Locked;
                }
            }

            if (spline.Locked)
                return;

            ShowMeshProperties = EditorGUILayout.Foldout(ShowMeshProperties, "Mesh Properties");

            if (ShowMeshProperties)
            {
                int l_MeshDetailLevel = spline.MeshDetailLevel;
                EditorGUI.BeginChangeCheck();
                l_MeshDetailLevel = EditorGUILayout.IntField("Mesh Size / Detail", l_MeshDetailLevel);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(spline);
                    spline.MeshDetailLevel = l_MeshDetailLevel;
                }


                SerializedProperty meshverts = serializedObject.FindProperty("MeshVertLayout");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(meshverts, true);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(spline);
                    serializedObject.ApplyModifiedProperties();

                    spline.DrawCurve();
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Reset Track Widths"))
                {
                    Undo.RecordObject(spline, "Reset Track Widths");
                    spline.ResetTrackWidths();
                }

                if (GUILayout.Button("Reset Track Tilt Angles"))
                {
                    Undo.RecordObject(spline, "Reset Track Tilt Angles");
                    spline.ResetTrackTiltAngles();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Recalculate UVs"))
                {
                    Undo.RecordObject(spline, "Recalculate UVs");
                    spline.RecalculateTrackUVs();
                }

                if (GUILayout.Button("Recalculate Normals"))
                {
                    Undo.RecordObject(spline, "Recalculate Normals");
                    spline.RecalculateTrackNormals();
                }

                GUILayout.EndHorizontal();

            }

            ShowSplineProperties = EditorGUILayout.Foldout(ShowSplineProperties, "Spline Properties");

            if (ShowSplineProperties)
            {
                EditorGUI.BeginChangeCheck();
                bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Toggle Loop");
                    EditorUtility.SetDirty(spline);
                    spline.Loop = loop;
                }
            }

            if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount && (selectedIndex % 3 == 0))
            {
                ShowSelectedPointProperties = EditorGUILayout.Foldout(ShowSelectedPointProperties, "Selected Point Properties");

                if (ShowSelectedPointProperties)
                {
                    DrawSelectedPointInspector();
                }
            }
            if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed"))
            {
                spline.DrawCurve();
            }
        }

        private void DrawSelectedPointInspector()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(selectedIndex, point);
            }

            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(selectedIndex, mode);
                EditorUtility.SetDirty(spline);
            }

            EditorGUI.BeginChangeCheck();
            float TrackWidth = EditorGUILayout.FloatField("Track Width", spline.GetControlPointTrackWidth(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change Track Width");
                spline.SetControlPointTrackWidth(selectedIndex, TrackWidth);
                EditorUtility.SetDirty(spline);
            }

            EditorGUI.BeginChangeCheck();
            float TrackTiltAngle = EditorGUILayout.FloatField("Track Tilt Angle", spline.GetControlPointTrackTiltAngle(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change Track Tilt Angle");
                spline.SetControlPointTrackTiltAngle(selectedIndex, TrackTiltAngle);
                EditorUtility.SetDirty(spline);
            }

            if (GUILayout.Button("Add Point Before"))
            {
                Undo.RecordObject(spline, "Add Curve Point");
                spline.AddPointBeforeSelectedPoint(selectedIndex);
                EditorUtility.SetDirty(spline);
            }

            if (GUILayout.Button("Add Point After"))
            {
                Undo.RecordObject(spline, "Add Curve Point");
                spline.AddPointAfterSelectedPoint(selectedIndex);
                EditorUtility.SetDirty(spline);
            }

        }
    }
}