using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public event Action<PointerEventData> DragStart;
    public event Action<PointerEventData> DragStop;
    public event Action<Vector2> InputChanged;
    public event Action<Vector2> RelativeInputChanged;
    public event Action<float> RadiusChanged;
    public event Action<float, float> AngleChanged;
    public event Action<float, float> RelativeAngleChanged;

    private const float MAX_RADIANS = 6.28318531f;

    private Vector2 _input;
    private Vector2 _relativeInput;
    private float _radius;
    private float _angleInRadians;
    private float _angleInDegrees;
    private float _relativeAngleInRadians;
    private float _relativeAngleInDegrees;
    private RectTransform _rectTransform;

    [SerializeField]
    private Image _threshold;
    [SerializeField]
    private Image _thumbstick;
    [SerializeField]
    private Transform _relativeObject;
    [SerializeField]
    [Range(0, 1)]
    private float _inactiveOpacity = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    private float _activeOpacity = 1f;

    private RectTransform ThresholdRect => _threshold.rectTransform;
    private RectTransform ThumbstickRect => _thumbstick.rectTransform;
    private float ThumbstickAngleInRadians
    {
        get
        {
            float angle = Mathf.Atan2(HorizontalAxis, VerticalAxis);

            if (angle < 0) angle += MAX_RADIANS;

            return angle;
        }
    }
    private float RelativeThumbstickAngleInRadians
    {
        get
        {
            float angleInRadians = (ThumbstickAngleInRadians + AngleOffsetInRadians) % MAX_RADIANS;

            return angleInRadians;
        }
    }
    private float AngleOffsetInRadians => _relativeObject.eulerAngles.y * Mathf.Deg2Rad;

    public Transform RelativeObject
    {
        get => _relativeObject;
        set => _relativeObject = value;
    }

    public float Radius => _radius;
    public float AngleInRadians => _angleInRadians;
    public float AngleInDegrees => _angleInDegrees;
    public float RelativeAngleInRadians => _relativeAngleInRadians;
    public float RelativeAngleInDegrees => _relativeAngleInDegrees;
    public float HorizontalAxis => _input.x;
    public float VerticalAxis => _input.y;
    public float RelativeHorizontalAxis => _relativeInput.x;
    public float RelativeVerticalAxis => _relativeInput.y;

    protected void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _input = Vector2.zero;
        _relativeInput = Vector2.zero;
        _radius = 0;
        _angleInRadians = 0;
        _angleInDegrees = 0;
        _relativeAngleInRadians = 0;
        _relativeAngleInDegrees = 0;

        Deactivate();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DragStart?.Invoke(eventData);

        Activate();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            ThresholdRect.anchoredPosition = localPoint;
        }

        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Deactivate();

        _input = Vector2.zero;
        _relativeInput = Vector2.zero;
        ThresholdRect.anchoredPosition = Vector2.zero;
        ThumbstickRect.anchoredPosition = Vector2.zero;
        _radius = 0;

        InputChanged?.Invoke(_input);
        RelativeInputChanged?.Invoke(_relativeInput);
        RadiusChanged?.Invoke(_radius);
        DragStop?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ThresholdRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            localPoint = ClampLocalPoint(localPoint, ThresholdRect.rect);
            _input = CalculateInput(localPoint);
            _radius = Vector2.Distance(Vector2.zero, _input);
            _relativeInput = CalculateRelativeInput(RelativeThumbstickAngleInRadians);
            _angleInRadians = ThumbstickAngleInRadians;
            _angleInDegrees = _angleInRadians * Mathf.Rad2Deg;
            _relativeAngleInRadians = CalculateRotationAngle(ThumbstickAngleInRadians, AngleOffsetInRadians);
            _relativeAngleInDegrees = _relativeAngleInRadians * Mathf.Rad2Deg;
            ThumbstickRect.anchoredPosition = CalculateThumbStickPosition(_input, ThresholdRect.rect);

            InputChanged?.Invoke(_input);
            RelativeInputChanged?.Invoke(_relativeInput);
            RadiusChanged?.Invoke(_radius);
            AngleChanged?.Invoke(_angleInRadians, _angleInDegrees);
            RelativeAngleChanged?.Invoke(_relativeAngleInRadians, _relativeAngleInDegrees);
        }
    }

    private void Activate()
    {
        Color thresholdColor = _threshold.color;
        Color thumbstickColor = _thumbstick.color;

        thresholdColor.a = _activeOpacity;
        thumbstickColor.a = _activeOpacity;

        _threshold.color = thresholdColor;
        _thumbstick.color = thumbstickColor;
    }

    private void Deactivate()
    {
        Color thresholdColor = _threshold.color;
        Color thumbstickColor = _thumbstick.color;

        thresholdColor.a = _inactiveOpacity;
        thumbstickColor.a = _inactiveOpacity;

        _threshold.color = thresholdColor;
        _thumbstick.color = thumbstickColor;
    }

    private Vector2 ClampLocalPoint(Vector2 localPoint, Rect threshold)
    {
        localPoint.x /= threshold.width;
        localPoint.y /= threshold.height;

        return localPoint;
    }

    private Vector2 CalculateInput(Vector2 localPoint)
    {
        float xValue = localPoint.x * 2;
        float yValue = localPoint.y * 2;
        Vector2 input = new Vector2(xValue, yValue);

        if (ShouldNormalizeVector(input)) input = input.normalized;

        return input;
    }

    private Vector2 CalculateRelativeInput(float angleInRadians)
    {
        float xValue = _radius * Mathf.Sin(angleInRadians);
        float yValue = _radius * Mathf.Cos(angleInRadians);
        Vector2 input = new Vector2(xValue, yValue);

        if (ShouldNormalizeVector(input)) input = input.normalized;

        return input;
    }

    private bool ShouldNormalizeVector(Vector3 input) => input.magnitude > 1.0f;

    private Vector3 CalculateThumbStickPosition(Vector2 input, Rect threshold)
    {
        float xPosition = input.x * (threshold.width / 2.5f);
        float yPosition = input.y * (threshold.height / 2.5f);

        return new Vector2(xPosition, yPosition);
    }

    private float CalculateRotationAngle(float angleInRadians, float radiansOffset)
    {
        float threshold = MAX_RADIANS - radiansOffset;

        if (angleInRadians < threshold) return radiansOffset + angleInRadians;
        else return angleInRadians - threshold;
    }
}
