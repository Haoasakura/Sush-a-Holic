using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line {

    const float verticalLineGradient =float.MaxValue;

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    float gradientPerpendicular;
    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
            gradientPerpendicular = verticalLineGradient;
        else
            gradientPerpendicular = dy / dx;

        if (gradientPerpendicular == 0)
            gradient = verticalLineGradient;
        else
            gradient = -1 / gradientPerpendicular;

        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = getSide(pointPerpendicularToLine);
    }

    public bool HasCrossedLine(Vector2 point) {
        return getSide(point) != approachSide;
    }

    public float DistanceFromPoint(Vector2 point) {
        float y_interceptPerpendicular = point.y - gradientPerpendicular * point.x;
        float x_intersect = (y_interceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
        float y_intersect = gradient * x_intersect + y_intercept;

        return Vector2.Distance(point, new Vector2(x_intersect, y_intersect));
    }

    bool getSide(Vector2 point) {
        return (point.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (point.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }
    public void DrawWithGizmos(float length) {
        Vector3 lineDirection = new Vector3(1f,gradient,0f).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, pointOnLine_1.y, 0f)+Vector3.forward;
        Gizmos.DrawLine(lineCenter-lineDirection*length*0.5f,lineCenter+lineDirection*length*0.5f);
    }
}
