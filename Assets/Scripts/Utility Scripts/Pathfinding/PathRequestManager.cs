using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public struct PathRequest {
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public struct PathResult {
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult (Vector3[] path,bool success, Action<Vector3[], bool> callback) {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public class PathRequestManager : MonoBehaviour {
  

    Queue<PathResult> results = new Queue<PathResult>();

    Pathfinding pathfinding;

    static PathRequestManager istance;

    private void Awake() {
        istance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update() {
        if(results.Count>0) {
            lock (results) {
                int itemsInQueue = results.Count;
                for (int i = 0; i < itemsInQueue;i++) {
                    PathResult result = results.Dequeue();
                    result.callback(result.path,result.success);
                }
            }
        }        
    }

    public static void RequestPath(PathRequest request) {
        ThreadStart threadStart = delegate {
            istance.pathfinding.FindPath(request, istance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result) {
        lock (results) {
            results.Enqueue(result);
        }
    }

    public static float DistanceFromTarget(Vector3[] waypoints,Vector2 position, int currentWaypoint) {
        float distance = 0;
        if (currentWaypoint < waypoints.Length - 1) {
            for (int i = currentWaypoint; i < waypoints.Length - 1; i++)
                distance += Vector2.Distance(waypoints[i], waypoints[i + 1]);
        }
        else
            distance += Vector2.Distance(position, waypoints[waypoints.Length-1]);

        return distance;
    }
}
