using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundDetector))]
partial class Movement : MonoBehaviour
{
    private InputState _inputBuffer;
    private float _directionOffset;
    private float _facingDirectionOffset;
    private float _currentSpeed;
    private float _currentDirection;

    [SerializeField]
    private AnimationCurve _speedCurve;
    [SerializeField]
    private AnimationCurve _rotationCurve;

    private float SpeedModifier => SpeedMultiplier * _speedCurve.Evaluate(_currentSpeed);
    private float RotationModifier => RotationMultiplier * _rotationCurve.Evaluate(Mathf.Abs(_directionOffset) / 180);

    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public GroundDetector GroundDetector { get; private set; }
    public IInputState InputBuffer => _inputBuffer;
    public float Speed => InputBuffer.Speed;
    public float DirectionAngle => InputBuffer.DirectionAngle;
    public float FacingDirectionAngle => InputBuffer.FacingDirectionAngle;
    public float SpeedMultiplier => InputBuffer.SpeedMultiplier;
    public float RotationMultiplier => InputBuffer.RotationMultiplier;
    public float DirectionOffset => _directionOffset;
    public float FacingDirectionOffset => _facingDirectionOffset;
    public float VerticalVelocity => Rigidbody.velocity.y;
    public bool IsGrounded => GroundDetector.IsGrounded;
    public bool FacingDirection
    {
        get => _inputBuffer.FacingDirection;
        set => _inputBuffer.FacingDirection = value;
    }

    protected void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        GroundDetector = GetComponent<GroundDetector>();

        Animator.applyRootMotion = false;

        _inputBuffer = new InputState();
        _currentSpeed = 0;
        _currentDirection = Rigidbody.rotation.eulerAngles.y;
    }

    public void Move(float speed, float directionAngle, float speedMultiplier = 1, float rotationMultiplier = 1)
    {
        _inputBuffer.Speed = speed;
        _inputBuffer.DirectionAngle = directionAngle;
        _inputBuffer.SpeedMultiplier = speedMultiplier;
        _inputBuffer.RotationMultiplier = rotationMultiplier;

        _directionOffset = CalculateDirectionOffset();
        _currentSpeed = CalculateCurrentSpeed();
        _currentDirection = CalculateCurrentDirection();
        if (FacingDirection) _facingDirectionOffset = CalculateFacingDirectionOffset();

        float rotationAngle = FacingDirection ? _currentDirection + _facingDirectionOffset : _currentDirection;
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
        Vector3 direction = Quaternion.Euler(0, _currentDirection, 0) * Vector3.forward;
        Vector3 positionOffset = SpeedModifier * _currentSpeed * direction;
        Vector3 position = Vector3.Lerp(Rigidbody.position, Rigidbody.position + positionOffset, Time.fixedDeltaTime);

        Rigidbody.MovePosition(position);
        Rigidbody.MoveRotation(rotation);
    }

    public void FaceTowards(float facingDirectionAngle)
    {
        _inputBuffer.FacingDirectionAngle = facingDirectionAngle;
    }

    public void Animate(float animationSpeed = 1)
    {
        Animator.speed = animationSpeed;

        Animator.SetFloat(nameof(Speed), _currentSpeed);
        Animator.SetFloat(nameof(DirectionOffset), DirectionOffset);
        Animator.SetFloat(nameof(FacingDirectionOffset), FacingDirection ? FacingDirectionOffset : 0);
        Animator.SetFloat(nameof(VerticalVelocity), VerticalVelocity);
        Animator.SetFloat(nameof(GroundDetector.FallingDistanceToGround), GroundDetector.FallingDistanceToGround);
        Animator.SetBool(nameof(IsGrounded), IsGrounded);

        //Change Jump Landing at higher speed (mix landing with strafe)
    }

    public void Jump(float jumpForce = 1)
    {
        Animator.SetTrigger(nameof(Jump));

        Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private float CalculateCurrentSpeed()
    {
        if (_currentSpeed < Speed) _currentSpeed += Time.fixedDeltaTime;
        if (_currentSpeed > Speed) _currentSpeed -= Time.fixedDeltaTime;
        if (_currentSpeed > 1) _currentSpeed = 1;
        if (_currentSpeed < 0) _currentSpeed = 0;

        return _currentSpeed;
    }

    private float CalculateCurrentDirection()
    {
        _currentDirection += RotationModifier * _directionOffset * Time.fixedDeltaTime;

        if (_currentDirection > 360) _currentDirection %= 360;
        if (_currentDirection < 0) _currentDirection = 360 - _currentDirection;

        return _currentDirection;
    }

    private float CalculateDirectionOffset()
    {
        _directionOffset = DirectionAngle - _currentDirection;

        if (_directionOffset < -180) _directionOffset = 360 - _currentDirection + DirectionAngle;
        if (_directionOffset > 180) _directionOffset = -(360 - DirectionAngle + _currentDirection);

        return _directionOffset;
    }

    private float CalculateFacingDirectionOffset()
    {
        _facingDirectionOffset = FacingDirectionAngle - _currentDirection;

        if (_facingDirectionOffset < -180) _facingDirectionOffset = 360 - _currentDirection + FacingDirectionAngle;
        if (_facingDirectionOffset > 180) _facingDirectionOffset = -(360 - FacingDirectionAngle + _currentDirection);

        return _facingDirectionOffset;
    }
}
