using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
//этим инструментом можно двигать только те объекты, которые содержат компонент CustomSnap
[EditorTool(displayName:"Custom Snap Move",typeof(CustomSnap))]
public class CustomSnappingTool : EditorTool
{
    public Texture2D ToolIcon;
    private Transform oldTarget;
    private Dictionary<PointBuilder, List<CustomSnapPoint>> allPoints;
    private List<CustomSnapPoint> targetPoints;

    private void OnEnable()
    {
        
    }
    public override GUIContent toolbarIcon
    {
        get
        {
            return new GUIContent(
            image: ToolIcon,
            text: "Custom Snap Move Tool",
            tooltip: "Custom Snap Move Tool - best tool ever"
            );
        }
    }
    public override void OnToolGUI(EditorWindow window)
    {

        Transform targetTransform = ((CustomSnap)target).transform;
        if (targetTransform != oldTarget)
        {
            PointBuilder pointBuilder = target.GetComponent<PointBuilder>();
            allPoints = pointBuilder.GetAllPoints();
            targetPoints = pointBuilder.GetTargetPoints(pointBuilder);
            oldTarget = targetTransform;
        }
        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(targetTransform.position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetTransform, name: "Move with snap tool");
            MoveWithSnapping(targetTransform, newPosition);

        }
    }
    private void MoveWithSnapping(Transform targetTransform, Vector3 newPosition)
    {
        Vector3 bestPosition = newPosition;
        float closestDistance = float.PositiveInfinity;
        foreach (var points in allPoints)
        {
            PointBuilder key = points.Key;
            if ( points.Key!= target.GetComponent<PointBuilder>())
            {
                foreach (var point in allPoints[key])
                {
                    foreach (var ownPoint in targetPoints)
                    {
                        Vector3 targetPos = point.pos - (ownPoint.pos - targetTransform.position);
                        float distance = Vector3.Distance(targetPos, newPosition);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            bestPosition = targetPos;
                        }
                    }
                }
               
            }
        }
        if (closestDistance < 0.5f)
        {
            targetTransform.position = bestPosition;
        }
        else
        {
            targetTransform.position = newPosition;
        }
    }
}
