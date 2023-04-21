using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HumanoidLandInput _input;
    [SerializeField] Transform _playerController;
    [SerializeField] Transform _cameraFollow;
    [SerializeField] Camera MainCamera;
    [SerializeField] CinemachineVirtualCamera cinemachine1stPerson;
    [SerializeField] CinemachineVirtualCamera cinemachine3rdPerson;
    CinemachineFramingTransposer _cinemachineFramingTransposer3rdPerson;
    [SerializeField] CinemachineVirtualCamera cinemachineOrbit;
    CinemachineFramingTransposer _cinemachineFramingTransposerOrbit;

    [Header("Parameters")]
    float _minCameraZoomDistance = 1.0f;
    //float _minOrbitCameraZoomDistance = 1.0f;
    [Range(1.0f, 10.0f), SerializeField] float _maxCameraZoomDistance = 4.0f;
    //float _maxOrbitCameraZoomDistance = 36.0f;

    CinemachineVirtualCamera _activeCamera;
    int _activeCameraPriorityModifier = 3250;
    public bool UsingOrbitalCamera { get; private set; } = false;
    float _cameraZoomModifier = 32.0f;


    private void Awake()
    {
        _cinemachineFramingTransposer3rdPerson = cinemachine3rdPerson.GetCinemachineComponent<CinemachineFramingTransposer>();
        _cinemachineFramingTransposerOrbit = cinemachineOrbit.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        ChangeCamera(); // First time through, set the default camera
    }

    private void Update()
    {
        if (_input.ZoomCameraInput != 0.0f)
            ZoomCamera();
        if (_input.ChangeCameraWasPressedThisFrame)
            ChangeCamera();

            TryOrbitCamera();
    }

    private void ZoomCamera()
    {
        if (_activeCamera == cinemachine3rdPerson || _activeCamera == cinemachineOrbit)
        {
            _cinemachineFramingTransposer3rdPerson.m_CameraDistance = Mathf.Clamp(_cinemachineFramingTransposer3rdPerson.m_CameraDistance + (_input.InvertScroll ? -_input.ZoomCameraInput : _input.ZoomCameraInput) / _cameraZoomModifier, _minCameraZoomDistance, _maxCameraZoomDistance);
            //_cinemachineFramingTransposerOrbit.m_CameraDistance = Mathf.Clamp(_cinemachineFramingTransposerOrbit.m_CameraDistance + (_input.InvertScroll ? -_input.ZoomCameraInput : _input.ZoomCameraInput) / _cameraZoomModifier, _minOrbitCameraZoomDistance, _maxOrbitCameraZoomDistance);
            _cinemachineFramingTransposerOrbit.m_CameraDistance = _cinemachineFramingTransposer3rdPerson.m_CameraDistance;
        }
    }

    private void ChangeCamera()
    {
        if (cinemachine3rdPerson == _activeCamera)
        {
            SetCameraPriorities(cinemachine3rdPerson, cinemachine1stPerson);
            MainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player (Self)"));
        }
        else if (cinemachine1stPerson == _activeCamera)
        {
            SetCameraPriorities(cinemachine1stPerson, cinemachine3rdPerson);
            MainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player (Self)");
        }
        else // for first time through or if there is an error
        {
            cinemachine3rdPerson.Priority += _activeCameraPriorityModifier;
            _activeCamera = cinemachine3rdPerson;
        }
    }

    private void SetCameraPriorities(CinemachineVirtualCamera CurrentCameraMode, CinemachineVirtualCamera NewCameraMode)
    {
        CurrentCameraMode.Priority -= _activeCameraPriorityModifier;
        NewCameraMode.Priority += _activeCameraPriorityModifier;
        _activeCamera = NewCameraMode;
        //Debug.Log(_activeCamera.name);
    }

    private void TryOrbitCamera()
    {
        // Cannot orbit from 1st person perspective
        if (cinemachine1stPerson != _activeCamera && _input.OrbitIsPressed)
        {
            if (cinemachineOrbit == _activeCamera)
                return;
            else
            {
                // There is an issue when moving mouse quickly at the same time as pressing ALT
                // The camera ends up looking in a weird direction as if you were already using the orbit camera and moving the mouse to look around

                // Switch to orbit camera
                CinemachinePOV orbitPOV = cinemachineOrbit.GetCinemachineComponent<CinemachinePOV>();

                if (_cameraFollow.eulerAngles.x > 89.95f)
                    orbitPOV.m_VerticalAxis.Value = _cameraFollow.eulerAngles.x - 360;
                else
                    orbitPOV.m_VerticalAxis.Value = _cameraFollow.eulerAngles.x;

                //Debug.Log("Vertical axis = " + _cameraFollow.eulerAngles.x);
                orbitPOV.m_HorizontalAxis.Value = _playerController.eulerAngles.y;

                SetCameraPriorities(cinemachine3rdPerson, cinemachineOrbit);
                UsingOrbitalCamera = true;
                MainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player (Self)");
            }
        }
        else
        {
            if (cinemachine3rdPerson == _activeCamera || cinemachine1stPerson == _activeCamera)
                return;
            else
            {
                // switch to 3rd person camera
                SetCameraPriorities(cinemachineOrbit, cinemachine3rdPerson);
                _activeCamera = cinemachine3rdPerson;
                UsingOrbitalCamera = false;
            }
        }
    }
}
