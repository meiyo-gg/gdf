using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardHumanoidLandController : MonoBehaviour
{
    [Header("Note: This controller is not based on the physics system.")]

    [Header("References:")]
    // Input
    [SerializeField] HumanoidLandInput _input;
    Vector3 _playerMoveInput = Vector3.zero;
    Vector3 _playerLookInput = Vector3.zero;
    Vector3 _previousPlayerLookInput = Vector3.zero;

    // Character controller
    [SerializeField] CharacterController _characterController;

    // Animator
    [SerializeField] Animator _animator;

    // Camera
    [SerializeField] CameraController _cameraController;
    [SerializeField] Transform _cameraFollow;

    // Rotation and Pitch
    [Tooltip("The sensitivity of the camera rotation along the x-axis.")]
    [HideInInspector, SerializeField] float _cameraYawSensitivity = 180.0f;
    [Tooltip("The sensitivity of the camera rotation along the y-axis.")]
    [HideInInspector, SerializeField] float _cameraPitchSensitivity = 180.0f;
    float _cameraPitch = 0.0f;
    float _playerLookInputLerpTime = 0.35f;

    [Header("Parameters:")]
    // Movement
    [Tooltip("The speed of the player when they are walking.")]
    [SerializeField] float _walkSpeed = 1;
    [Tooltip("The speed of the player when they are running.")]
    [SerializeField] float _runSpeed = 4;
    [Tooltip("The speed of the player when they are crouching.")]
    [SerializeField] float _crouchSpeed = 0.9f;
    [Tooltip("The speed of the player when they are jumping.")]
    [SerializeField] float _jumpSpeed = 6f;
    float _speed = 1;
    Vector3 _direction = Vector3.zero;

    // External forces affecting movement and rotation
    // ...

    // Jumping and Gravity
    [Tooltip("Gravity is applied to the player when they are not grounded - it determines the rate of acceleration of the y-axis velocity when jumping or falling.")]
    [SerializeField, Range(-500, -10)] float _gravity = -80f;
    float defaultGroundedGravity = -0.5f;
    float groundedGravity = 0;
    float previousYVelocity = 0;
    float newYVelocity = 0;
    float nextYVelocity = 0;
    float playerYMovement = 0;
    bool isFalling = false;
    //Vector3 groundNormal = new Vector3(0, 0, 0);
    //float angleDifference = 0;
    Vector3 groundRotation = new Vector3(0, 0, 0);
    float maxGroundAngle = 0;
    float groundedGravitySlopeMultiplier = 1;

    [Tooltip("Initial jump velocity is the y-axis velocity of the player when they press jump. The rate of change of the y-axis velocity (acceleration) is determined by the gravity parameter.")]
    [SerializeField, Range(1, 100)] float _initialJumpVelocity = 10;
    bool isJumping = false;

    // Crouching
    float defaultHeight = 2;
    float crouchHeight = 1.25f;
    float heightDifference = 0;
    float lastStandingYPosition = 0;
    float headroom = 0.1f;
    Transform armature;
    bool isCrouching = false;
    bool colliderIsAboveCharacter = false;

    private void Awake()
    {
        defaultHeight = _characterController.height;
        heightDifference = defaultHeight - crouchHeight;
        armature = transform.GetChild(0);
    }

    private void FixedUpdate()
    {
        // Only perform camera calculations if orbital camera is not being used (pitch and yaw sensitives won't apply to orbit camera)
        if (!_cameraController.UsingOrbitalCamera)
        {
            _playerLookInput = GetLookInput();
            PlayerLook();
            PitchCamera();
        }

        _playerMoveInput = GetMoveInput();

        SetSpeed();
        MovePlayer();
        Gravity();
        Crouch();
        Jump();
    }

    private Vector3 GetLookInput()
    {
        _previousPlayerLookInput = _playerLookInput;
        _playerLookInput = new Vector3(_input.LookInput.x, (_input.InvertMouseY ? -_input.LookInput.y : _input.LookInput.y), 0.0f);
        return Vector3.Lerp(_previousPlayerLookInput, _playerLookInput * Time.deltaTime, _playerLookInputLerpTime);
    }

    private void PlayerLook()
    {
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + (_playerLookInput.x * _cameraYawSensitivity), 0.0f);
    }

    private void PitchCamera()
    {
        _cameraPitch += _playerLookInput.y * _cameraPitchSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -89.9f, 89.9f);

        _cameraFollow.rotation = Quaternion.Euler(_cameraPitch, _cameraFollow.rotation.eulerAngles.y, _cameraFollow.rotation.eulerAngles.z);
    }

    private Vector3 GetMoveInput()
    {
        return new Vector3(_input.MoveInput.x, playerYMovement, _input.MoveInput.y);
    }

    private void SetSpeed()
    {
        if (_input.CrouchIsPressed)
            _speed = _crouchSpeed;
        else if (_input.JumpIsPressed)
            _speed = _jumpSpeed;
        else if (_input.RunIsPressed)
            _speed = _runSpeed;
        else
            _speed = _walkSpeed;
    }

    private void MovePlayer()
    {
        _direction = (transform.TransformDirection(_playerMoveInput));
        _direction = new Vector3(_direction.x * _speed, _direction.y, _direction.z * _speed);
        _characterController.Move((_direction * Time.deltaTime));

        _animator.SetFloat("VelocityX", _playerMoveInput.x * _speed, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityZ", _playerMoveInput.z * _speed, 0.1f, Time.deltaTime);
    }

    private void Gravity()
    {
        if (!_characterController.isGrounded)
        {
            isFalling = true;
            _animator.SetBool("isFalling", isFalling);
            Verlet();
        }
        else
        {
            isFalling = false;
            _animator.SetBool("isFalling", isFalling);

            if (!_input.JumpIsPressed)
            {
                /*The following code sets the groundedGravity value based on the slope of the ground
                This is necessary to prevent bouncing when travelling down a slope*/

                // Cast ray from bottom of the controller to ground
                if (Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out RaycastHit hitInfo, _characterController.height))
                {
#if UNITY_EDITOR
                    Debug.DrawRay(transform.position, Vector3.down * hitInfo.distance, Color.red, 2f, false);
#endif
                    // Get rotation values of the ground
                    groundRotation = hitInfo.transform.rotation.eulerAngles;

                    // If any axis value is greater than 180, subtract 360 to get negative values (-45 is the opposite angle of 45)
                    groundRotation.x = (groundRotation.x > 180) ? groundRotation.x - 360 : groundRotation.x;
                    groundRotation.y = (groundRotation.y > 180) ? groundRotation.y - 360 : groundRotation.y;
                    groundRotation.z = (groundRotation.z > 180) ? groundRotation.z - 360 : groundRotation.z;

                    // Compare the absolute values of each axis to get the max ground angle
                    maxGroundAngle = Mathf.Max(Mathf.Abs(groundRotation.x), Mathf.Abs(groundRotation.y), Mathf.Abs(groundRotation.z));

                    // Adjust groundedGravity value to be proportional to the max ground angle
                    groundedGravity = defaultGroundedGravity - (groundedGravitySlopeMultiplier * maxGroundAngle);
                }

                playerYMovement = groundedGravity;
            }
        }
    }

    private void Verlet()
    {
        // Apply verlet to average the difference between velocity in previous frame and velocity this frame
        // The result reduces errors due to differences in frame rates, making jumps more consistent
        previousYVelocity = playerYMovement;
        newYVelocity = playerYMovement + (_gravity * Time.deltaTime); // Update y velocity by adding (gravity force * deltaTime)
        nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f; // Get the average of the previous and current y velocities
        playerYMovement = nextYVelocity;
    }

    private void Crouch()
    {
        // check if collider is above character and is lower than standing height + buffer
        if (Physics.SphereCast(new Vector3(transform.position.x, lastStandingYPosition, transform.position.z), _characterController.radius, Vector3.up, out RaycastHit hitInfo, (defaultHeight * 0.5f) + headroom))
        {
            colliderIsAboveCharacter = true;
        }
        else
        {
            colliderIsAboveCharacter = false;
        }

        if (_input.CrouchIsPressed && !isCrouching && !isJumping)
        {
            isCrouching = true;
            
            // We need this for the above sphere cast
            lastStandingYPosition = transform.position.y;

            // Adujst the height of the character controller
            _characterController.height = crouchHeight;

            // Adjust the center of the character controller
            _characterController.center = new Vector3(_characterController.center.x, _characterController.center.y + (heightDifference * 0.5f), _characterController.center.z);

            // Adjust the position of the transform
            transform.position = new Vector3(transform.position.x, transform.position.y - heightDifference, transform.position.z);

            // Adjust the position of the armature (character model), which is the first child of this object
            Transform armature = transform.GetChild(0);
            armature.position = new Vector3(armature.position.x, armature.position.y + heightDifference, armature.position.z);

            _animator.SetBool("isCrouching", isCrouching);
        }
        else if (!_input.CrouchIsPressed && isCrouching && !colliderIsAboveCharacter)
        {
            // Do the opposite of the above
            isCrouching = false;

            _characterController.height = defaultHeight;

            _characterController.center = new Vector3(_characterController.center.x, _characterController.center.y - (heightDifference * 0.5f), _characterController.center.z);

            transform.position = new Vector3(transform.position.x, transform.position.y + heightDifference, transform.position.z);

            armature.position = new Vector3(armature.position.x, armature.position.y - heightDifference, armature.position.z);

            _animator.SetBool("isCrouching", isCrouching);
        }
    }

    private void Jump()
    {
        if (!isJumping && _characterController.isGrounded && _input.JumpIsPressed && !_input.CrouchIsPressed)
        {
            isJumping = true;
            playerYMovement = _initialJumpVelocity;
            _animator.SetBool("isJumping", true);
        }
        else if (isJumping && _characterController.isGrounded) //&& !_input.JumpIsPressed)
        {
            isJumping = false;
            _animator.SetBool("isJumping", false);
        }
    }
}
