using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform[] _patrolPoints;


    public Vector3[] GetPatrolPositions()
    {
        //if (_patrolPoints != null)
        //{
        //    return _patrolPoints.Select(p => p.position).ToArray();
        //}

        return new Vector3[0];

    }

    private void OnDrawGizmos()
    {
        if (_patrolPoints != null)
        {
            foreach (Transform point in _patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(point.position, Vector3.one * 0.2f);
                }
            }
        }
    }

}
