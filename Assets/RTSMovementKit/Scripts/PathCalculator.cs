using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathCalculator : Singleton<PathCalculator> {

    NavMeshPath path;

    float timeAtLastCalculate;
    int calculationsPrFrame = 5;

    private void Awake()
    {
        path = new NavMeshPath();
    }

    private void Update()
    {
        calculationsPrFrame = 5;
    }

    public bool CalculatePath(Vector3 startPos, Vector3 targetPos, out Vector3[] vPath)
    {
        
        vPath = null;
        if (calculationsPrFrame <= 0)
            return false;
        if (NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, path))
        {
            calculationsPrFrame--;
            timeAtLastCalculate = Time.time;
            vPath = path.corners;
            return true;
        }
        return false;
    }

}
