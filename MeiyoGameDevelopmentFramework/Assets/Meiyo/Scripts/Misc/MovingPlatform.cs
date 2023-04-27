using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaypointPath _waypointPath;

    [Header("Parameters")]
    [SerializeField] private Transform _platformCenter;
    [SerializeField] private float _speed;
    [SerializeField] private bool _applySmoothing = false;
    [SerializeField] private bool _waitForTrigger = false;
    [SerializeField] private bool _waitAtStart = false;
    [SerializeField] private bool _waitAtFinish = false;
    [SerializeField] private bool _continuousRotation = false;
    [SerializeField] private float _rotationSpeed;

    public bool WaitAtStart { get { return _waitAtStart; } }
    public bool WaitAtFinish { get { return _waitAtFinish; } }
    private bool trigger = true;
    public bool Trigger { get { return trigger; } set { trigger = value; } }

    Dictionary<Transform, float> TransformsOnPlatformAndTime = new Dictionary<Transform, float>();
    [SerializeField] List<Transform> TransformsOnPlatform = new List<Transform>();

    private int _targetWaypointIndex = 0;
    private Transform _previousWaypoint;
    private Transform _targetWaypoint;
    private float _timeToWaypoint;
    private float _elapsedTime;

    private Quaternion _previousRotation;
    private Vector3 _rotationDifference;

    private Vector3 _previousPosition;
    private Vector3 _positionDifference;

    private float targetPlatformTriggerSizeY = 2;
    
    // The size of the box collider on the platform increases depending on the max height of the objects on the platform
    // The collider's size parameter is relative to the Y scale of the platform
    // Currently, changing the platform's Y scale during runtime does not automatically adjust the collider's size
    // The collider's size is only adjust when OnTriggerEnter() or OnTriggerExit() is called
    // AdjustTriggerCollider() could be called in FixedUpdate, but that is a lot of unneccesarry processing every physics frame
    // An alternative solution should be considered

    private void Start()
    {
        if (_waitForTrigger)
            Trigger = false;

        AdjustTriggerCollider();
        TargetNextWaypoint();
    }

    private void FixedUpdate()
    {
        // If trigger is false, return
        if (!trigger)
            return;

        _previousPosition = transform.position; // _currentPosition;
        _previousRotation = transform.rotation; // _currentRotation;

        /*if (TransformsOnPlatform.Count != TransformsOnPlatformAndTime.Count)
        {
            TransformsOnPlatformAndTime.Clear();
            foreach (Transform t in TransformsOnPlatform)
            {
                TransformsOnPlatformAndTime.Add(t, 1.0f);
            }
        }*/

        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWaypoint;

        if (_applySmoothing)
        {
            elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        }
        
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);

        if (_continuousRotation)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + _rotationSpeed * Time.deltaTime, transform.eulerAngles.z);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(_previousWaypoint.rotation, _targetWaypoint.rotation, elapsedPercentage);
        }

        foreach (Transform t in TransformsOnPlatform)
        {
            TransformsOnPlatformAndTime.TryGetValue(t, out float timer);
            if (timer < 1.0f)
            {
                TransformsOnPlatformAndTime[t] += Time.deltaTime * 4.0f;
            }
            else if (timer > 1.0f)
            {
                TransformsOnPlatformAndTime[t] = 1.0f;
            }
            RotateAndMoveTransformsOnPlatform(t);
        }

        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }

    private void TargetNextWaypoint()
    {
        if (_waitForTrigger)
            Trigger = false;

        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

    private void RotateAndMoveTransformsOnPlatform(Transform t)
    {
        // Get the difference in platform position between frames
        _positionDifference = transform.position - _previousPosition;

        // Apply the change in position to the transform of the object on the platform
        t.position = t.position + _positionDifference;

        // Get the difference in platform rotation between frames
        _rotationDifference = transform.eulerAngles - _previousRotation.eulerAngles;

        // Rotate the object about the y-axis through the center point of the platform
        t.RotateAround(_platformCenter.position, Vector3.up, _rotationDifference.y);  
    }

    private void OnTriggerEnter(Collider other)
    {
        TransformsOnPlatform.Add(other.transform);
        TransformsOnPlatformAndTime.Add(other.transform, 0.0f);

        AdjustTriggerCollider();
    }

    private void OnTriggerExit(Collider other)
    {
        TransformsOnPlatform.Remove(other.transform);
        TransformsOnPlatformAndTime.Remove(other.transform);
        AdjustTriggerCollider();
    }

    private void AdjustTriggerCollider()
    {
        float maxYBound = -Mathf.Infinity;

        foreach (Transform t in TransformsOnPlatform)
        {
            if (t.tag == "PlayerController")
                continue;
            else
            {
                float objectYMax = t.GetComponent<Collider>().bounds.max.y;
                if (objectYMax > maxYBound)
                    maxYBound = objectYMax;
            }
        }

        Collider platformCollider = transform.GetChild(0).GetComponent<Collider>();
        float difference = maxYBound - platformCollider.bounds.max.y;

        if (TransformsOnPlatform.Count == 0)
        {
            difference = 0;
        }
        else if (TransformsOnPlatform.Count == 1 && TransformsOnPlatform[0].tag == "PlayerController")
        {
            difference = 0;
        }

        BoxCollider platformTrigger = transform.GetComponent<BoxCollider>();
        float scaleY = transform.localScale.y;

        platformTrigger.size = new Vector3(platformTrigger.size.x, (targetPlatformTriggerSizeY + difference) / scaleY, platformTrigger.size.z);

        platformTrigger.center = new Vector3(0, (platformTrigger.size.y * 0.5f) + 0.5f, 0);
    }

    public void TriggerEvent(Interactive sender)
    {
        Trigger = true;
        StartCoroutine(WaitForNextStop(sender));
    }

    IEnumerator WaitForNextStop(Interactive sender)
    {
        while (Trigger)
        {
            yield return new WaitForEndOfFrame();
        }
        sender.CanInteract = true;
    }
}
