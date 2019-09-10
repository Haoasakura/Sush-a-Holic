using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPath {

    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;
    
    public SmoothPath (Vector3[] waypoints, Vector3 startPosition, float turnDistance) {
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = startPosition;
        for(int i=0;i<lookPoints.Length;i++) {
            Vector2 currentPoint = lookPoints[i];
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundatyPoint = (i==finishLineIndex)?currentPoint : currentPoint - dirToCurrentPoint * turnDistance;
            turnBoundaries[i] = new Line(turnBoundatyPoint, previousPoint -dirToCurrentPoint*turnDistance);
            previousPoint = turnBoundatyPoint;
        }
    }

    public void DrawWithGizmos() {
        Gizmos.color = Color.black;
        foreach(Vector3 point in lookPoints) {
            Gizmos.DrawCube(point + Vector3.up, Vector3.one);
        }
        Gizmos.color = Color.white;
        foreach (Line line in turnBoundaries) {
            line.DrawWithGizmos(5);
        }
    }
}
