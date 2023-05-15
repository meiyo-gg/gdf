using UnityEngine;
using UnityEngine.InputSystem;

public class HumanoidLandInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public bool MoveIsPressed { get; private set; } = false;

    public Vector2 LookInput { get; private set; } = Vector2.zero;
    public bool InvertMouseY { get; private set; } = true;
    public float ZoomCameraInput { get; private set; } = 0.0f;
    public bool InvertScroll { get; private set; } = true;

    public bool RunIsPressed { get; private set; } = false;

    public bool CrouchIsPressed { get; private set; } = false;

    public bool JumpIsPressed { get; private set; } = false;

    public bool OrbitIsPressed { get; private set; } = false;

    public bool ChangeCameraWasPressedThisFrame { get; private set; } = false;

    InputActions _input = null;

    private void OnEnable()
    {
        _input = new InputActions();
        _input.HumanoidLand.Enable();

        _input.HumanoidLand.Move.performed += SetMove;
        _input.HumanoidLand.Move.canceled += SetMove;

        _input.HumanoidLand.Look.performed += SetLook;
        _input.HumanoidLand.Look.canceled += SetLook;

        _input.HumanoidLand.Run.started += SetRun;
        _input.HumanoidLand.Run.canceled += SetRun;

        _input.HumanoidLand.Crouch.started += SetCrouch;
        _input.HumanoidLand.Crouch.canceled += SetCrouch;

        _input.HumanoidLand.Jump.started += SetJump;
        _input.HumanoidLand.Jump.canceled += SetJump;

        _input.HumanoidLand.OrbitCamera.started += SetOrbit;
        _input.HumanoidLand.OrbitCamera.canceled += SetOrbit;

        _input.HumanoidLand.ZoomCamera.started += SetZoomCamera;
        _input.HumanoidLand.ZoomCamera.canceled += SetZoomCamera;
    }

    private void OnDisable()
    {
        _input.HumanoidLand.Move.performed -= SetMove;
        _input.HumanoidLand.Move.canceled -= SetMove;

        _input.HumanoidLand.Look.performed -= SetLook;
        _input.HumanoidLand.Look.canceled -= SetLook;

        _input.HumanoidLand.Run.started -= SetRun;
        _input.HumanoidLand.Run.canceled -= SetRun;

        _input.HumanoidLand.Crouch.started -= SetCrouch;
        _input.HumanoidLand.Crouch.canceled -= SetCrouch;

        _input.HumanoidLand.Jump.started -= SetJump;
        _input.HumanoidLand.Jump.canceled -= SetJump;

        _input.HumanoidLand.OrbitCamera.started -= SetOrbit;
        _input.HumanoidLand.OrbitCamera.canceled -= SetOrbit;

        _input.HumanoidLand.ZoomCamera.started -= SetZoomCamera;
        _input.HumanoidLand.ZoomCamera.canceled -= SetZoomCamera;

        _input.HumanoidLand.Disable();
    }

    private void Update()
    {
        ChangeCameraWasPressedThisFrame = _input.HumanoidLand.ChangeCamera.WasPerformedThisFrame();
    }

    private void SetMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        //Debug.Log(MoveInput.ToString());
        MoveIsPressed = !(MoveInput == Vector2.zero);
    }

    private void SetLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
        //Debug.Log(LookInput.ToString());
    }

    private void SetRun(InputAction.CallbackContext ctx)
    {
        RunIsPressed = ctx.started;
        //Debug.Log("Player is running: " + RunIsPressed.ToString());
    }

    private void SetCrouch(InputAction.CallbackContext ctx)
    {
        CrouchIsPressed = ctx.started;
        //Debug.Log("Player is crouching: " + CrouchIsPressed.ToString());
    }

    private void SetJump(InputAction.CallbackContext ctx)
    {
        JumpIsPressed = ctx.started;
        //Debug.Log("Player is jumping: " + JumpIsPressed.ToString());
    }

    private void SetOrbit(InputAction.CallbackContext ctx)
    {
        OrbitIsPressed = ctx.started;
        //Debug.Log("Player is orbiting the camera: " + OrbitIsPressed.ToString());
    }

    private void SetZoomCamera(InputAction.CallbackContext ctx)
    {
        ZoomCameraInput = ctx.ReadValue<float>();
    }

    public void ResetInputs()
    {
        MoveInput = Vector2.zero;
        LookInput = Vector2.zero;
    }
}
