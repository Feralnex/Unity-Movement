using UnityEngine;

class CameraFollow : MonoBehaviour
{
    private Vector3 _currentVelocity;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _smoothTime;
    [SerializeField]
    private Vector3 _offset;

    protected void Reset()
    {
        _currentVelocity = Vector3.zero;
        _smoothTime = 0.125f;
        _offset = Vector3.zero;
    }

    protected void FixedUpdate()
    {
        if (_target == null) return;

        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, _smoothTime);

        transform.position = smoothedPosition;

        transform.LookAt(_target);
    }
}
