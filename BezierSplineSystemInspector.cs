using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline)), CanEditMultipleObjects]
public class BezierSplineSystemInspector : Editor
{

    private const int lineSteps = 10;
    private const float directionScale = 0.5f;

    private BezierSpline[] splineList;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private void OnSceneGUI()
    {
        splineList = targets as BezierSpline[];

		//iterate thru splines
        for (int j = 0; j < splineList.Length; j++)
        {
            handleTransform = splineList[j].transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0, splineList[j]);
            for (int i = 1; i < splineList[j].points.Length; i += 3)
            {
                Vector3 p1 = ShowPoint(i, splineList[j]);
                Vector3 p2 = ShowPoint(i + 1, splineList[j]);
                Vector3 p3 = ShowPoint(i + 2, splineList[j]);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }
            ShowDirections(splineList[j]);
        }


    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        splineList = targets as BezierSpline[];
        for (int i = 0; i < splineList.Length; i++)
        {
            if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(splineList[i], "Add Curve");
                splineList[i].AddCurve();
                EditorUtility.SetDirty(splineList[i]);
            }
            if (GUILayout.Button("New splineList[i]"))
            {
                Undo.RecordObject(splineList[i], "New spline");
                splineList[i].AddSpline();
                EditorUtility.SetDirty(splineList[i]);
            }
        }

    }

    private const int stepsPerCurve = 10;

    private void ShowDirections(BezierSpline spline)
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;

    private Vector3 ShowPoint(int index, BezierSpline spline)
    {
        Vector3 point = handleTransform.TransformPoint(spline.points[index]);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.white;
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
        }

        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.points[index] = handleTransform.InverseTransformPoint(point);
            }
        }
        return point;
    }
}
