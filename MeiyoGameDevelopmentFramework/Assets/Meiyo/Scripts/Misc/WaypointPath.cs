using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private MovingPlatform _movingPlatform;
    private bool reverseAtFinish = false;

    public bool ReverseAtFinish { set { reverseAtFinish = value; } }

    int increment = 1;

    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        // If currentWaypointIndex is the final waypoint
        if (currentWaypointIndex == transform.childCount - 1)
        {
            // If the moving platform has _waitAtFinish set to true, set its trigger to false
            if (_movingPlatform.WaitAtFinish)
                _movingPlatform.Trigger = false;

            // If _reverstAtFinish is set to true
            if (reverseAtFinish)
            {
                // Apply reverse
                increment = -1;
            }
            else
            {
                // Continue as normal
                increment = 1;
            }
        }

        // If currentWaypointIndex is the frist waypoint
        if (currentWaypointIndex == 0)
        {
            increment = 1;

            if (_movingPlatform.WaitAtStart)
                _movingPlatform.Trigger = false;
        }

        // Set nextWaypointIndex
        int nextWaypointIndex = currentWaypointIndex + increment;

        // If nextWaypointIndex is out of bounds (this will only happen if _reverseAtFinish is false)
        if (nextWaypointIndex == transform.childCount)
        {
            // Go to first waypoint
            nextWaypointIndex = 0;
        }

        return nextWaypointIndex;
    }
}
