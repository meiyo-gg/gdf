using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private bool _reverseAtFinish = false;
    int increment = 1;

    public Transform GetWaypoint(int waypointIndex)
    {
        Debug.Log("Waypoint index: " + waypointIndex);
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        // If currentWaypointIndex is the final waypoint
        if (currentWaypointIndex == transform.childCount - 1)
        {
            // If _reverstAtFinish is set to true
            if (_reverseAtFinish)
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
