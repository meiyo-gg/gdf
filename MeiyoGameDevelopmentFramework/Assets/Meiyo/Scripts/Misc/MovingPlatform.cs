using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaypointPath _waypointPath;

    [Header("Parameters")]
    [SerializeField] private float _speed;
    [SerializeField] private bool _applySmoothing = false;
    [SerializeField] private bool _waitForTrigger = false;
    [SerializeField] private bool _waitAtFinish = false;
    [SerializeField] private bool _continuousRotation = false;
    [SerializeField] private float _rotationSpeed;

    Dictionary<Transform, float> TransformsOnPlatformAndTime = new Dictionary<Transform, float>();
    [SerializeField] List<Transform> TransformsOnPlatform = new List<Transform>();

    private int _targetWaypointIndex = 0;
    private Transform _previousWaypoint;
    private Transform _targetWaypoint;
    private float _timeToWaypoint;
    private float _elapsedTime;

    private Quaternion _previousRotation;
    private Quaternion _currentRotation;
    private Vector3 _rotationDifference;

    private Vector3 _previousPosition;
    private Vector3 _currentPosition;
    private Vector3 _positionDifference;

    private float defaultPlatformTriggerSizeY = 8;
    private float defaultPlatformTriggerCenterY = 4;


    private void Start()
    {
        _currentPosition = transform.position;
        BoxCollider platformTrigger = transform.GetComponent<BoxCollider>();
        defaultPlatformTriggerSizeY = platformTrigger.size.y;
        defaultPlatformTriggerCenterY = platformTrigger.center.y;
        TargetNextWaypoint();
    }

    private void FixedUpdate()
    {
        _previousRotation = _currentRotation;
        _currentRotation = transform.rotation;

        _previousPosition = _currentPosition;
        _currentPosition = transform.position;

        if (TransformsOnPlatform.Count != TransformsOnPlatformAndTime.Count)
        {
            TransformsOnPlatformAndTime.Clear();
            foreach (Transform t in TransformsOnPlatform)
            {
                TransformsOnPlatformAndTime.Add(t, 1.0f);
            }
        }

        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWaypoint;

        if (_applySmoothing)
        {
            elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        }
        
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);

        if (_continuousRotation)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + _rotationSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);
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
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

    private void RotateAndMoveTransformsOnPlatform(Transform t)
    {
        // Get the difference in platform rotation between frames
        _rotationDifference = _currentRotation.eulerAngles - _previousRotation.eulerAngles; 

        // Apply the change in rotation to the transform of object on the platform
        t.eulerAngles = t.eulerAngles + _rotationDifference;

        /*_distanceFromPivot = new Vector3(t.position.x - transform.position.x, t.position.y - transform.position.y, t.position.z - transform.position.z);
        _newPosition = transform.position + _distanceFromPivot;*/


        // Now we need to update movement
        _positionDifference = _currentPosition - _previousPosition;

        // Apply the change in position to the transform of the object on the platform
        t.position = t.position + _positionDifference;

        // Apply the change in position to the transform of the player controller
        //t.localPosition = t.localPosition + _positionDifference;
        /*if (t.TryGetComponent<StandardHumanoidLandController>(out StandardHumanoidLandController shlc))
        {
            shlc.PlatformMovement = _positionDifference;
        }*/


    }

    private void OnTriggerEnter(Collider other)
    {
        TransformsOnPlatform.Add(other.transform);
        TransformsOnPlatformAndTime.Add(other.transform, 0.0f);
        //AdjustTriggerCollider();
    }

    private void OnTriggerExit(Collider other)
    {
       /* // Remove the force applied to the character controller
        if (other.gameObject.TryGetComponent<StandardHumanoidLandController>(out StandardHumanoidLandController shlc))
        {
            shlc.PlatformMovement = Vector3.zero;
        }*/
        TransformsOnPlatform.Remove(other.transform);
        TransformsOnPlatformAndTime.Remove(other.transform);
        //AdjustTriggerCollider();
    }

    /*private void AdjustTriggerCollider()
    {
        float maxYBound = -Mathf.Infinity;

        foreach (Transform t in TransformsOnPlatform)
        {
            if (t.tag == "PlayerController")
                break;
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

        BoxCollider platformTrigger = transform.GetComponent<BoxCollider>();
        platformTrigger.size = new Vector3(platformTrigger.size.x, defaultPlatformTriggerSizeY + difference, platformTrigger.size.z);

        platformTrigger.center = new Vector3(0, platformTrigger.size.y * 0.5f, 0);
    }*/
}
