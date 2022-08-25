using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Movement))]
class MovementController : MonoBehaviour
{
    [SerializeField]
    private VirtualJoystick _virtualJoystick;
    [SerializeField]
    private Button _jumpButton;
    [SerializeField]
    private float _speedMultiplier;
    [SerializeField]
    private float _rotationMultiplier;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _animationSpeed;

    public Movement Movement { get; private set; }

    public float Speed { get; private set; }
    public float DirectionAngle { get; private set; }
    public float SpeedMultiplier
    {
        get => _speedMultiplier;
        set => _speedMultiplier = OnSpeedMultiplierChange(value);
    }
    public float RotationMultiplier
    {
        get => _rotationMultiplier;
        set => _rotationMultiplier = OnRotationMultiplierChange(value);
    }
    public float JumpForce
    {
        get => _jumpForce;
        set => _jumpForce = OnJumpForceChange(value);
    }
    public float AnimationSpeed
    {
        get => _animationSpeed;
        set => _animationSpeed = OnAnimationSpeedChange(value);
    }

    protected void Awake()
    {
        Movement = GetComponent<Movement>();
    }

    protected void OnEnable()
    {
        _virtualJoystick.RadiusChanged += OnRadiusChanged;
        _virtualJoystick.RelativeAngleChanged += OnAngleChanged;
        _jumpButton.onClick.AddListener(OnJumpClicked);

        Movement.GroundDetector.StateChanged += OnGroundStateChanged;
    }

    protected void OnDisable()
    {
        _virtualJoystick.RadiusChanged -= OnRadiusChanged;
        _virtualJoystick.RelativeAngleChanged -= OnAngleChanged;
        _jumpButton.onClick.RemoveListener(OnJumpClicked);

        Movement.GroundDetector.StateChanged -= OnGroundStateChanged;
    }

    protected void FixedUpdate()
    {
        Movement.Move(Speed, DirectionAngle, _speedMultiplier, _rotationMultiplier);
    }

    protected void Update()
    {
        Movement.Animate(_animationSpeed);
    }

    protected void OnValidate()
    {
        SpeedMultiplier = _speedMultiplier;
        RotationMultiplier = _rotationMultiplier;
        JumpForce = _jumpForce;
        AnimationSpeed = _animationSpeed;
    }

    private void OnRadiusChanged(float radius)
    {
        if (Movement.IsGrounded) Speed = radius;
    }

    private void OnAngleChanged(float angleInRadians, float angleInDegrees)
    {
        if (Movement.IsGrounded) DirectionAngle = angleInDegrees;
    }

    private void OnJumpClicked()
    {
        if (Movement.IsGrounded) Movement.Jump(_jumpForce);
    }

    private void OnGroundStateChanged(GroundState groundState)
    {
        if (groundState == GroundState.Grounded)
        {
            Speed = _virtualJoystick.Radius;
            DirectionAngle = _virtualJoystick.RelativeAngleInDegrees;
        }
    }

    private float OnSpeedMultiplierChange(float speedMultiplier)
    {
        if (speedMultiplier < 0) speedMultiplier = 0;

        return speedMultiplier;
    }

    private float OnRotationMultiplierChange(float rotationMultiplier)
    {
        if (rotationMultiplier < 0) rotationMultiplier = 0;

        return rotationMultiplier;
    }

    private float OnJumpForceChange(float jumpForce)
    {
        if (jumpForce < 0) jumpForce = 0;

        return jumpForce;
    }

    private float OnAnimationSpeedChange(float animationSpeed)
    {
        if (animationSpeed < 0) animationSpeed = 0;

        return animationSpeed;
    }
}
