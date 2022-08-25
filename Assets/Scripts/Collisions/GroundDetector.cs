using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class GroundDetector : MonoBehaviour
{
    public event Action<GroundState> StateChanged;

    private List<Collider> _groundColliders;
    private List<Collider> _waterColliders;
    private GroundState _state;
    private float _distanceToGround;
    private float _fallingDistanceToGround;

    public Rigidbody Rigidbody { get; private set; }

    public GroundState State
    {
        get => _state;
        private set
        {
            _state = value;

            StateChanged?.Invoke(_state);
        }
    }
    public float DistanceToGround => _distanceToGround;
    public float FallingDistanceToGround => _fallingDistanceToGround;
    public bool IsGrounded => _state == GroundState.Grounded;
    public bool IsInTheWater => _state == GroundState.InTheWater;
    public bool IsInTheAir => _state == GroundState.InTheAir;

    protected void Awake()
    {
        _groundColliders = new List<Collider>();
        _waterColliders = new List<Collider>();

        Rigidbody = GetComponent<Rigidbody>();

        State = GroundState.InTheAir;
    }

    protected void FixedUpdate()
    {
        Physics.Raycast(transform.position, Rigidbody.velocity, out RaycastHit raycastHit);

        _distanceToGround = raycastHit.distance;

        if (IsInTheAir)
        {
            Physics.Raycast(transform.position, Rigidbody.velocity, out raycastHit);

            _fallingDistanceToGround = raycastHit.distance;
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        _groundColliders.AddRange(GetGroundColliders(collision));
        _waterColliders.AddRange(GetWaterColliders(collision));

        if (IsCollidingWithGround()) State = GroundState.Grounded;
        else if (IsCollidingWithWater()) State = GroundState.InTheWater;
        else State = GroundState.InTheAir;
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (WasItGroundCollider(collision.collider)) _groundColliders.Remove(collision.collider);
        if (WasItWaterCollider(collision.collider)) _waterColliders.Remove(collision.collider);

        if (!IsCollidingWithGround())
        {
            if (IsCollidingWithWater()) State = GroundState.InTheWater;
            else State = GroundState.InTheAir;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Fluid>())
        {
            _waterColliders.Add(other);
        }

        if (IsCollidingWithGround()) State = GroundState.Grounded;
        else if (IsCollidingWithWater()) State = GroundState.InTheWater;
        else State = GroundState.InTheAir;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Fluid>())
        {
            _waterColliders.Remove(other);
        }

        if (!IsCollidingWithGround())
        {
            if (IsCollidingWithWater()) State = GroundState.InTheWater;
            else State = GroundState.InTheAir;
        }
    }

    private List<Collider> GetGroundColliders(Collision collision)
    {
        List<Collider> groundColliders = new List<Collider>();

        for (int index = 0; index < collision.contactCount; index++)
        {
            ContactPoint contactPoint = collision.GetContact(index);

            if (contactPoint.normal.y > 0.5f && !groundColliders.Contains(contactPoint.otherCollider))
            {
                groundColliders.Add(contactPoint.otherCollider);
            }
        }

        return groundColliders;
    }

    private List<Collider> GetWaterColliders(Collision collision)
    {
        List<Collider> waterColliders = new List<Collider>();

        for (int index = 0; index < collision.contactCount; index++)
        {
            ContactPoint contactPoint = collision.GetContact(index);

            if (contactPoint.otherCollider.GetComponent<Fluid>())
            {
                _waterColliders.Add(contactPoint.otherCollider);
            }
        }

        return waterColliders;
    }

    private bool IsCollidingWithGround()
    {
        return _groundColliders.Count != 0;
    }

    private bool IsCollidingWithWater()
    {
        return _waterColliders.Count != 0;
    }

    private bool WasItGroundCollider(Collider collider)
    {
        return _groundColliders.Contains(collider);
    }

    private bool WasItWaterCollider(Collider collider)
    {
        return _waterColliders.Contains(collider);
    }
}